import { FormEvent, useEffect, useMemo, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { createSummons, getCitizens, getOffices } from '../api';
import type { Citizen, Office, Session } from '../types';

export function SummonsCreatePage({ session }: { session: Session }) {
  const navigate = useNavigate();
  const canWrite = session.role === 'Operator' || session.role === 'Manager';
  const [citizens, setCitizens] = useState<Citizen[]>([]);
  const [offices, setOffices] = useState<Office[]>([]);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');

  const today = useMemo(() => new Date().toISOString().slice(0, 10), []);
  const [number, setNumber] = useState(`CIPSO-2026-${String(Date.now()).slice(-6)}`);
  const [citizenId, setCitizenId] = useState('');
  const [officeId, setOfficeId] = useState('');
  const [issuedAt, setIssuedAt] = useState(today);
  const [dueAt, setDueAt] = useState(`${today}T12:00`);
  const [reason, setReason] = useState('Явка в военный комиссариат для уточнения документов воинского учёта');
  const [comment, setComment] = useState('Синтетическая запись учебного реестра повесток военного учёта');

  useEffect(() => {
    void Promise.all([getCitizens(), getOffices()])
      .then(([citizenResult, officeResult]) => {
        setCitizens(citizenResult);
        setOffices(officeResult);
        setCitizenId(citizenResult[0]?.id ?? '');
        setOfficeId(officeResult[0]?.id ?? '');
      })
      .catch(e => setError(e instanceof Error ? e.message : 'Не удалось загрузить справочники'))
      .finally(() => setLoading(false));
  }, []);

  if (!canWrite) {
    return (
      <section className="panel access-panel">
        <h1>Недостаточно прав</h1>
        <p>Призывник может просматривать данные, но не создавать повестки.</p>
        <Link to="/summons" className="secondary button-link">Вернуться в реестр</Link>
      </section>
    );
  }

  const office = offices.find(x => x.id === officeId);
  const employee = office?.employees[0];

  async function submit(event: FormEvent) {
    event.preventDefault();

    if (!employee) {
      setError('У выбранного военкомата нет секретаря для создания записи.');
      return;
    }

    setBusy(true);
    setError('');

    try {
      const created = await createSummons({
        number: number.trim(),
        citizenId,
        authorityOfficeId: officeId,
        createdByEmployeeId: employee.id,
        issuedAt,
        dueAt: new Date(dueAt).toISOString(),
        reason: reason.trim(),
        comment: comment.trim()
      });
      navigate(`/summons/${created.id}`, { replace: true });
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось создать повестку');
    } finally {
      setBusy(false);
    }
  }

  return (
    <>
      <header className="topbar">
        <div>
          <p className="eyebrow">Реестр / новая запись</p>
          <h1>Создание повестки военного учёта</h1>
        </div>
        <Link className="secondary button-link" to="/summons">Отмена</Link>
      </header>

      <section className="panel form-panel">
        {loading ? (
          <div className="state">Загрузка справочников…</div>
        ) : (
          <form className="form-grid" onSubmit={submit}>
            <label className="full">
              Номер
              <input required value={number} onChange={e => setNumber(e.target.value)} />
            </label>

            <label>
              Призывник
              <select required value={citizenId} onChange={e => setCitizenId(e.target.value)}>
                {citizens.map(citizen => (
                  <option key={citizen.id} value={citizen.id}>
                    {citizen.lastName} {citizen.firstName} · {citizen.registryNumber}
                  </option>
                ))}
              </select>
            </label>

            <label>
              Военкомат
              <select required value={officeId} onChange={e => setOfficeId(e.target.value)}>
                {offices.map(item => (
                  <option key={item.id} value={item.id}>{item.name}</option>
                ))}
              </select>
            </label>

            <label>
              Дата формирования
              <input
                type="date"
                required
                value={issuedAt}
                onChange={e => setIssuedAt(e.target.value)}
              />
            </label>

            <label>
              Срок явки
              <input
                type="datetime-local"
                required
                value={dueAt}
                onChange={e => setDueAt(e.target.value)}
              />
            </label>

            <label className="full">
              Основание / причина явки
              <textarea required rows={4} value={reason} onChange={e => setReason(e.target.value)} />
            </label>

            <label className="full">
              Комментарий
              <textarea rows={3} value={comment} onChange={e => setComment(e.target.value)} />
            </label>

            {error && <div className="error compact full">{error}</div>}

            <div className="form-actions full">
              <Link className="secondary button-link" to="/summons">Отмена</Link>
              <button className="primary" disabled={busy || !citizenId || !officeId}>
                {busy ? 'Создание…' : 'Создать повестку'}
              </button>
            </div>
          </form>
        )}
      </section>
    </>
  );
}
