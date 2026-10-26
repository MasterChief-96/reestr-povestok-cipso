import { FormEvent, useEffect, useMemo, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { createSummons, getCitizens, getOffices } from '../../../shared/api';
import type { Citizen, Office, Session } from '../../../shared/types';
import { errorMessage, getApiFieldErrors } from '../../../shared/validation/errors';
import type { FieldErrors } from '../../../shared/validation/errors';
import { validateSummonsInput } from '../../../shared/validation/validators';
import { FieldError, fieldProps, focusFirstInvalid } from '../../../shared/ui/FieldError';

export function SummonsCreatePage({ session }: { session: Session }) {
  const navigate = useNavigate();
  const canWrite = session.role === 'Operator' || session.role === 'Manager';
  const [citizens, setCitizens] = useState<Citizen[]>([]);
  const [offices, setOffices] = useState<Office[]>([]);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

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

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const problems = validateSummonsInput({
      number, citizenId, officeId, issuedAt, dueAt, reason, comment, hasEmployee: Boolean(employee)
    });
    setFieldErrors(problems);
    if (Object.keys(problems).length) {
      setError('Исправьте отмеченные поля и повторите попытку.');
      focusFirstInvalid(event.currentTarget, problems);
      return;
    }
    if (!employee) return;

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
      const problems = getApiFieldErrors(e);
      setFieldErrors(problems);
      setError(Object.keys(problems).length
        ? 'Сервер обнаружил ошибки в отмеченных полях.'
        : errorMessage(e, 'Не удалось создать повестку'));
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
          <form className="form-grid" onSubmit={submit} noValidate>
            <label className="full">
              Номер
              <input {...fieldProps('number', fieldErrors)} required value={number} onChange={e => { setNumber(e.target.value); setFieldErrors(current => ({ ...current, number: '' })); }} />
              <FieldError name="number" errors={fieldErrors} />
            </label>

            <label>
              Призывник
              <select {...fieldProps('citizenId', fieldErrors)} required value={citizenId} onChange={e => { setCitizenId(e.target.value); setFieldErrors(current => ({ ...current, citizenId: '' })); }}>
                {citizens.map(citizen => (
                  <option key={citizen.id} value={citizen.id}>
                    {citizen.lastName} {citizen.firstName} · {citizen.registryNumber}
                  </option>
                ))}
              </select>
              <FieldError name="citizenId" errors={fieldErrors} />
            </label>

            <label>
              Военкомат
              <select {...fieldProps('officeId', fieldErrors)} required value={officeId} onChange={e => { setOfficeId(e.target.value); setFieldErrors(current => ({ ...current, officeId: '' })); }}>
                {offices.map(item => (
                  <option key={item.id} value={item.id}>{item.name}</option>
                ))}
              </select>
              <FieldError name="officeId" errors={fieldErrors} />
            </label>

            <label>
              Дата формирования
              <input {...fieldProps('issuedAt', fieldErrors)}
                type="date"
                required
                value={issuedAt}
                onChange={e => { setIssuedAt(e.target.value); setFieldErrors(current => ({ ...current, issuedAt: '' })); }}
              />
              <FieldError name="issuedAt" errors={fieldErrors} />
            </label>

            <label>
              Срок явки
              <input {...fieldProps('dueAt', fieldErrors)}
                type="datetime-local"
                required
                value={dueAt}
                onChange={e => { setDueAt(e.target.value); setFieldErrors(current => ({ ...current, dueAt: '' })); }}
              />
              <FieldError name="dueAt" errors={fieldErrors} />
            </label>

            <label className="full">
              Основание / причина явки
              <textarea {...fieldProps('reason', fieldErrors)} required rows={4} value={reason} onChange={e => { setReason(e.target.value); setFieldErrors(current => ({ ...current, reason: '' })); }} />
              <FieldError name="reason" errors={fieldErrors} />
            </label>

            <label className="full">
              Комментарий
              <textarea {...fieldProps('comment', fieldErrors)} rows={3} value={comment} onChange={e => { setComment(e.target.value); setFieldErrors(current => ({ ...current, comment: '' })); }} />
              <FieldError name="comment" errors={fieldErrors} />
            </label>

            {error && <div className="error compact full">{error}</div>}

            <div className="form-actions full">
              <Link className="secondary button-link" to="/summons">Отмена</Link>
              <button className="primary" disabled={busy}>
                {busy ? 'Создание…' : 'Создать повестку'}
              </button>
            </div>
          </form>
        )}
      </section>
    </>
  );
}
