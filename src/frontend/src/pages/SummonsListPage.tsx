import { FormEvent, useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { getOffices, getSummons } from '../api';
import { StatusBadge } from '../components/StatusBadge';
import { statusLabels, statuses } from '../constants';
import type {
  Office,
  PagedSummons,
  Session,
  SummonsFilters,
  SummonsStatus
} from '../types';

const emptyPage: PagedSummons = {
  items: [],
  page: 1,
  pageSize: 10,
  total: 0,
  totalPages: 0
};

export function SummonsListPage({ session }: { session: Session }) {
  const navigate = useNavigate();
  const [offices, setOffices] = useState<Office[]>([]);
  const [data, setData] = useState<PagedSummons>(emptyPage);
  const [filters, setFilters] = useState<SummonsFilters>({
    search: '',
    status: '',
    officeId: '',
    page: 1,
    pageSize: 10
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const canWrite = session.role === 'Operator' || session.role === 'Manager';

  async function load(next: SummonsFilters) {
    setLoading(true);
    setError('');

    try {
      const result = await getSummons(next);
      setData(result);
      setFilters(prev => ({ ...prev, ...next, page: result.page }));
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось загрузить реестр');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void Promise.all([getOffices(), getSummons(filters)])
      .then(([officeResult, summonsResult]) => {
        setOffices(officeResult);
        setData(summonsResult);
      })
      .catch(e => setError(e instanceof Error ? e.message : 'Не удалось загрузить реестр'))
      .finally(() => setLoading(false));
  }, []);

  function submit(event: FormEvent) {
    event.preventDefault();
    void load({ ...filters, page: 1 });
  }

  return (
    <>
      <header className="topbar">
        <div>
          <p className="eyebrow">Реестр</p>
          <h1>Повестки военного учёта</h1>
        </div>
        <div className="top-actions">
          <span className="environment">{data.total} записей</span>
          {canWrite && (
            <Link className="primary button-link" to="/summons/new">+ Новая повестка</Link>
          )}
        </div>
      </header>

      <section className="panel">
        <div className="panel-head">
          <div>
            <h2>Реестр повесток</h2>
            <p>Поиск и фильтры выполняются на сервере. На странице показывается по 10 записей.</p>
          </div>
        </div>

        <form className="filters" onSubmit={submit}>
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
            <option value="">Все военкоматы</option>
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
                  <th>Призывник</th>
                  <th>Военкомат</th>
                  <th>Дата формирования</th>
                  <th>Срок явки</th>
                  <th>Статус</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map(item => (
                  <tr
                    key={item.id}
                    className="clickable-row"
                    onClick={() => navigate(`/summons/${item.id}`)}
                  >
                    <td>
                      <strong>{item.number}</strong>
                      {item.reason && <small>{item.reason}</small>}
                    </td>
                    <td>
                      {item.citizen.lastName} {item.citizen.firstName}
                      <small>{item.citizen.registryNumber}</small>
                    </td>
                    <td>
                      {item.office.name}
                      <small>{item.office.code}</small>
                    </td>
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
          <span>Всего: {data.total} · страница {data.page} из {Math.max(data.totalPages, 1)}</span>
          <div>
            <button
              className="secondary"
              disabled={data.page <= 1 || loading}
              onClick={() => void load({ ...filters, page: data.page - 1 })}
            >
              ← Назад
            </button>
            <button
              className="secondary"
              disabled={data.page >= data.totalPages || loading}
              onClick={() => void load({ ...filters, page: data.page + 1 })}
            >
              Вперед →
            </button>
          </div>
        </div>
      </section>
    </>
  );
}
