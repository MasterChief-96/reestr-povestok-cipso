import { FormEvent, useEffect, useMemo, useState } from 'react';
import {
  SESSION_KEY,
  createSummons,
  getCitizens,
  getOffices,
  getSummons,
  getSummonsById,
  login
} from './api';
import type {
  Citizen,
  Office,
  PagedSummons,
  Session,
  SummonsDetail,
  SummonsFilters,
  SummonsListItem,
  SummonsStatus
} from './types';

const statuses: Array<SummonsStatus | ''> = [
  '',
  'Draft',
  'Issued',
  'Delivered',
  'Acknowledged',
  'Completed',
  'Cancelled'
];

const statusLabels: Record<SummonsStatus, string> = {
  Draft: 'Черновик',
  Issued: 'Выпущена',
  Delivered: 'Доставлена',
  Acknowledged: 'Подтверждена',
  Completed: 'Завершена',
  Cancelled: 'Отменена'
};

function readStoredSession(): Session | null {
  const raw = localStorage.getItem(SESSION_KEY);
  if (!raw) return null;

  try {
    const session = JSON.parse(raw) as Session;
    if (new Date(session.expiresAt).getTime() <= Date.now()) {
      localStorage.removeItem(SESSION_KEY);
      return null;
    }
    return session;
  } catch {
    return null;
  }
}

function StatusBadge({ status }: { status: SummonsStatus }) {
  return <span className={`badge badge-${status.toLowerCase()}`}>{statusLabels[status]}</span>;
}

function LoginScreen({ onLogin }: { onLogin: (session: Session) => void }) {
  const [username, setUsername] = useState('operator');
  const [password, setPassword] = useState('demo123!');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  async function submit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError('');

    try {
      const session = await login(username, password);
      localStorage.setItem(SESSION_KEY, JSON.stringify(session));
      onLogin(session);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось войти');
    } finally {
      setBusy(false);
    }
  }

  return (
    <main className="login-page">
      <section className="login-card">
        <div className="brand brand-dark">ЦИПСО</div>
        <p className="eyebrow">Учебный демонстрационный контур</p>
        <h1>Реестр электронных повесток</h1>
        <p className="muted">
          Авторизация реализована через JWT и роли. В проекте используются только синтетические данные.
        </p>

        <form onSubmit={submit} className="login-form">
          <label>
            Логин
            <input
              value={username}
              onChange={e => setUsername(e.target.value)}
              autoComplete="username"
            />
          </label>
          <label>
            Пароль
            <input
              type="password"
              value={password}
              onChange={e => setPassword(e.target.value)}
              autoComplete="current-password"
            />
          </label>
          {error && <div className="error compact">{error}</div>}
          <button className="primary" disabled={busy}>
            {busy ? 'Вход…' : 'Войти'}
          </button>
        </form>

        <div className="demo-credentials">
          <strong>Демо-аккаунты</strong>
          <span>operator / demo123! — запись</span>
          <span>manager / demo123! — запись и контроль</span>
          <span>observer / demo123! — только чтение</span>
        </div>
      </section>
    </main>
  );
}

function DetailDrawer({
  detail,
  loading,
  onClose
}: {
  detail: SummonsDetail | null;
  loading: boolean;
  onClose: () => void;
}) {
  if (!detail && !loading) return null;

  return (
    <div className="drawer-backdrop" onMouseDown={onClose}>
      <aside className="drawer" onMouseDown={e => e.stopPropagation()}>
        <div className="drawer-head">
          <div>
            <p className="eyebrow">Карточка повестки</p>
            <h2>{detail?.number ?? 'Загрузка…'}</h2>
          </div>
          <button className="icon-button" onClick={onClose}>×</button>
        </div>

        {loading || !detail ? (
          <div className="state">Загрузка карточки…</div>
        ) : (
          <div className="drawer-body">
            <section className="detail-grid">
              <div>
                <span>Статус</span>
                <StatusBadge status={detail.status} />
              </div>
              <div>
                <span>Адресат</span>
                <strong>{detail.citizen.lastName} {detail.citizen.firstName}</strong>
                <small>{detail.citizen.registryNumber}</small>
              </div>
              <div>
                <span>Подразделение</span>
                <strong>{detail.authorityOffice.name}</strong>
                <small>{detail.authorityOffice.code}</small>
              </div>
              <div>
                <span>Оператор</span>
                <strong>{detail.createdByEmployee.fullName}</strong>
                <small>{detail.createdByEmployee.personnelNumber}</small>
              </div>
              <div>
                <span>Дата выпуска</span>
                <strong>{new Date(detail.issuedAt).toLocaleDateString('ru-RU')}</strong>
              </div>
              <div>
                <span>Срок</span>
                <strong>{new Date(detail.dueAt).toLocaleString('ru-RU')}</strong>
              </div>
            </section>

            <section className="detail-section">
              <h3>Основание</h3>
              <p>{detail.reason}</p>
              {detail.comment && <p className="muted">{detail.comment}</p>}
            </section>

            <section className="detail-section">
              <h3>История статусов</h3>
              <div className="timeline">
                {[...detail.statusHistory]
                  .sort((a, b) => +new Date(b.changedAt) - +new Date(a.changedAt))
                  .map(item => (
                    <div className="timeline-item" key={item.id}>
                      <span className="timeline-dot" />
                      <div>
                        <strong>{statusLabels[item.toStatus]}</strong>
                        <small>
                          {new Date(item.changedAt).toLocaleString('ru-RU')} · {item.changedBy}
                        </small>
                        {item.comment && <p>{item.comment}</p>}
                      </div>
                    </div>
                  ))}
              </div>
            </section>

            <section className="detail-section detail-columns">
              <div>
                <h3>Уведомления</h3>
                <strong className="metric">{detail.notifications.length}</strong>
                <p className="muted">зафиксировано отправок</p>
              </div>
              <div>
                <h3>Обращения</h3>
                <strong className="metric">{detail.appeals.length}</strong>
                <p className="muted">связано с записью</p>
              </div>
              <div>
                <h3>Документы</h3>
                <strong className="metric">{detail.documents.length}</strong>
                <p className="muted">метаданных файлов</p>
              </div>
            </section>
          </div>
        )}
      </aside>
    </div>
  );
}

function CreateDialog({
  citizens,
  offices,
  onClose,
  onCreated
}: {
  citizens: Citizen[];
  offices: Office[];
  onClose: () => void;
  onCreated: () => Promise<void>;
}) {
  const today = new Date().toISOString().slice(0, 10);
  const defaultOffice = offices[0];

  const [number, setNumber] = useState(`CIPSO-${new Date().getFullYear()}-`);
  const [citizenId, setCitizenId] = useState(citizens[0]?.id ?? '');
  const [officeId, setOfficeId] = useState(defaultOffice?.id ?? '');
  const [issuedAt, setIssuedAt] = useState(today);
  const [dueAt, setDueAt] = useState(`${today}T12:00`);
  const [reason, setReason] = useState('Учебное оповещение');
  const [comment, setComment] = useState('Синтетические данные');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  const office = offices.find(x => x.id === officeId);
  const employee = office?.employees[0];

  async function submit(event: FormEvent) {
    event.preventDefault();

    if (!employee) {
      setError('У выбранного подразделения нет сотрудника для создания записи.');
      return;
    }

    setBusy(true);
    setError('');

    try {
      await createSummons({
        number: number.trim(),
        citizenId,
        authorityOfficeId: officeId,
        createdByEmployeeId: employee.id,
        issuedAt,
        dueAt: new Date(dueAt).toISOString(),
        reason: reason.trim(),
        comment: comment.trim()
      });

      await onCreated();
      onClose();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось создать запись');
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="modal-backdrop" onMouseDown={onClose}>
      <section className="modal" onMouseDown={e => e.stopPropagation()}>
        <div className="drawer-head">
          <div>
            <p className="eyebrow">Новая запись</p>
            <h2>Создать повестку</h2>
          </div>
          <button className="icon-button" onClick={onClose}>×</button>
        </div>

        <form className="form-grid" onSubmit={submit}>
          <label className="full">
            Номер
            <input required value={number} onChange={e => setNumber(e.target.value)} />
          </label>

          <label>
            Адресат
            <select required value={citizenId} onChange={e => setCitizenId(e.target.value)}>
              {citizens.map(c => (
                <option key={c.id} value={c.id}>
                  {c.lastName} {c.firstName} · {c.registryNumber}
                </option>
              ))}
            </select>
          </label>

          <label>
            Подразделение
            <select required value={officeId} onChange={e => setOfficeId(e.target.value)}>
              {offices.map(o => <option key={o.id} value={o.id}>{o.name}</option>)}
            </select>
          </label>

          <label>
            Дата выпуска
            <input
              type="date"
              required
              value={issuedAt}
              onChange={e => setIssuedAt(e.target.value)}
            />
          </label>

          <label>
            Срок
            <input
              type="datetime-local"
              required
              value={dueAt}
              onChange={e => setDueAt(e.target.value)}
            />
          </label>

          <label className="full">
            Основание
            <textarea
              required
              rows={3}
              value={reason}
              onChange={e => setReason(e.target.value)}
            />
          </label>

          <label className="full">
            Комментарий
            <textarea rows={2} value={comment} onChange={e => setComment(e.target.value)} />
          </label>

          {error && <div className="error compact full">{error}</div>}

          <div className="form-actions full">
            <button type="button" className="secondary" onClick={onClose}>Отмена</button>
            <button className="primary" disabled={busy}>
              {busy ? 'Создание…' : 'Создать'}
            </button>
          </div>
        </form>
      </section>
    </div>
  );
}

export default function App() {
  const [session, setSession] = useState<Session | null>(() => readStoredSession());
  const [data, setData] = useState<PagedSummons>({
    items: [],
    page: 1,
    pageSize: 10,
    total: 0,
    totalPages: 0
  });
  const [filters, setFilters] = useState<SummonsFilters>({
    search: '',
    status: '',
    officeId: '',
    page: 1,
    pageSize: 10
  });
  const [citizens, setCitizens] = useState<Citizen[]>([]);
  const [offices, setOffices] = useState<Office[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [detail, setDetail] = useState<SummonsDetail | null>(null);
  const [detailLoading, setDetailLoading] = useState(false);
  const [showCreate, setShowCreate] = useState(false);

  const canWrite = session?.role === 'Operator' || session?.role === 'Manager';

  async function loadSummons(next: SummonsFilters = filters) {
    setLoading(true);
    setError('');

    try {
      const result = await getSummons(next);
      setData(result);
      setFilters(prev => ({ ...prev, ...next, page: result.page }));
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Ошибка загрузки');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    const reset = () => setSession(null);
    window.addEventListener('cipso:unauthorized', reset);
    return () => window.removeEventListener('cipso:unauthorized', reset);
  }, []);

  useEffect(() => {
    if (!session) return;

    void Promise.all([getCitizens(), getOffices(), getSummons(filters)])
      .then(([citizenResult, officeResult, summonsResult]) => {
        setCitizens(citizenResult);
        setOffices(officeResult);
        setData(summonsResult);
      })
      .catch(e => setError(e instanceof Error ? e.message : 'Ошибка загрузки'));
  }, [session]);

  const visibleStatuses = useMemo(() => {
    const counts = new Map<SummonsStatus, number>();
    data.items.forEach(item => counts.set(item.status, (counts.get(item.status) ?? 0) + 1));
    return counts;
  }, [data.items]);

  if (!session) {
    return <LoginScreen onLogin={setSession} />;
  }

  async function openDetail(item: SummonsListItem) {
    setDetail(null);
    setDetailLoading(true);

    try {
      setDetail(await getSummonsById(item.id));
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось открыть карточку');
    } finally {
      setDetailLoading(false);
    }
  }

  function logout() {
    localStorage.removeItem(SESSION_KEY);
    setSession(null);
  }

  function submitSearch(event: FormEvent) {
    event.preventDefault();
    void loadSummons({ ...filters, page: 1 });
  }

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">ЦИПСО</div>
        <div className="brand-subtitle">Реестр повесток</div>

        <nav>
          <a className="nav-item active" href="#registry">Реестр</a>
          <a className="nav-item" href="#overview">Сводка</a>
          <a className="nav-item" href="/swagger" target="_blank">Swagger</a>
        </nav>

        <div className="sidebar-user">
          <strong>{session.displayName}</strong>
          <span>{session.role}</span>
          <button onClick={logout}>Выйти</button>
        </div>
        <div className="demo-note">Учебный контур · синтетические данные</div>
      </aside>

      <main className="content">
        <header className="topbar">
          <div>
            <p className="eyebrow">
              Централизованная информационная платформа списков оповещения
            </p>
            <h1>Реестр электронных повесток</h1>
          </div>
          <div className="top-actions">
            <span className="environment">DEMO · {session.role}</span>
            {canWrite && (
              <button className="primary" onClick={() => setShowCreate(true)}>
                + Новая повестка
              </button>
            )}
          </div>
        </header>

        <section className="stats" id="overview">
          <article><span>Всего записей</span><strong>{data.total}</strong></article>
          <article><span>На странице</span><strong>{data.items.length}</strong></article>
          <article>
            <span>Выпущено на странице</span>
            <strong>{visibleStatuses.get('Issued') ?? 0}</strong>
          </article>
          <article>
            <span>Страница</span>
            <strong>{data.totalPages ? `${data.page}/${data.totalPages}` : '0/0'}</strong>
          </article>
        </section>

        <section className="panel" id="registry">
          <div className="panel-head">
            <div>
              <h2>Реестр</h2>
              <p>Фильтры выполняются на сервере; результаты отдаются постранично.</p>
            </div>
          </div>

          <form className="filters" onSubmit={submitSearch}>
            <input
              value={filters.search ?? ''}
              onChange={e => setFilters({ ...filters, search: e.target.value })}
              placeholder="Номер, фамилия или реестровый номер"
            />
            <select
              value={filters.status ?? ''}
              onChange={e => setFilters({
                ...filters,
                status: e.target.value as SummonsStatus | ''
              })}
            >
              {statuses.map(status => (
                <option key={status || 'all'} value={status}>
                  {status ? statusLabels[status] : 'Все статусы'}
                </option>
              ))}
            </select>
            <select
              value={filters.officeId ?? ''}
              onChange={e => setFilters({ ...filters, officeId: e.target.value })}
            >
              <option value="">Все подразделения</option>
              {offices.map(office => (
                <option key={office.id} value={office.id}>{office.name}</option>
              ))}
            </select>
            <button className="secondary" type="submit">Применить</button>
          </form>

          {error && <div className="error">{error}</div>}

          {loading ? (
            <div className="state">Загрузка…</div>
          ) : (
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Номер</th>
                    <th>Адресат</th>
                    <th>Подразделение</th>
                    <th>Дата выпуска</th>
                    <th>Срок</th>
                    <th>Статус</th>
                  </tr>
                </thead>
                <tbody>
                  {data.items.map(item => (
                    <tr
                      key={item.id}
                      className="clickable-row"
                      onClick={() => void openDetail(item)}
                    >
                      <td><strong>{item.number}</strong><small>{item.reason}</small></td>
                      <td>
                        {item.citizen.lastName} {item.citizen.firstName}
                        <small>{item.citizen.registryNumber}</small>
                      </td>
                      <td>{item.office.name}<small>{item.office.code}</small></td>
                      <td>{new Date(item.issuedAt).toLocaleDateString('ru-RU')}</td>
                      <td>{new Date(item.dueAt).toLocaleString('ru-RU')}</td>
                      <td><StatusBadge status={item.status} /></td>
                    </tr>
                  ))}
                  {!data.items.length && (
                    <tr><td colSpan={6} className="state">Ничего не найдено</td></tr>
                  )}
                </tbody>
              </table>
            </div>
          )}

          <div className="pagination">
            <span>Всего: {data.total}</span>
            <div>
              <button
                className="secondary"
                disabled={data.page <= 1 || loading}
                onClick={() => void loadSummons({ ...filters, page: data.page - 1 })}
              >
                ←
              </button>
              <span>{data.page} / {Math.max(data.totalPages, 1)}</span>
              <button
                className="secondary"
                disabled={data.page >= data.totalPages || loading}
                onClick={() => void loadSummons({ ...filters, page: data.page + 1 })}
              >
                →
              </button>
            </div>
          </div>
        </section>
      </main>

      <DetailDrawer
        detail={detail}
        loading={detailLoading}
        onClose={() => {
          setDetail(null);
          setDetailLoading(false);
        }}
      />

      {showCreate && (
        <CreateDialog
          citizens={citizens}
          offices={offices}
          onClose={() => setShowCreate(false)}
          onCreated={() => loadSummons({ ...filters, page: 1 })}
        />
      )}
    </div>
  );
}
