import { useEffect, useMemo, useState } from 'react';
import { getCitizens, writeOffCitizen } from '../api';
import type { Citizen, Session } from '../types';

export function CitizensPage({ session }: { session: Session }) {
  const [citizens, setCitizens] = useState<Citizen[]>([]);
  const [search, setSearch] = useState('');
  const [error, setError] = useState('');
  const [busyId, setBusyId] = useState<string | null>(null);

  async function load() {
    setCitizens(await getCitizens());
  }

  useEffect(() => {
    void load()
      .catch(e => setError(e instanceof Error ? e.message : 'Не удалось загрузить данные призывника'));
  }, []);

  const filtered = useMemo(() => {
    const value = search.trim().toLowerCase();
    if (!value) return citizens;

    return citizens.filter(citizen =>
      citizen.registryNumber.toLowerCase().includes(value) ||
      citizen.lastName.toLowerCase().includes(value) ||
      citizen.firstName.toLowerCase().includes(value)
    );
  }, [citizens, search]);

  const observer = session.role === 'Observer';
  const canWriteOff = session.role === 'Operator' || session.role === 'Manager';

  async function handleWriteOff(citizen: Citizen) {
    const fullName = [citizen.lastName, citizen.firstName, citizen.middleName]
      .filter(Boolean)
      .join(' ');

    if (!window.confirm(
      `Списать призывника ${fullName} (${citizen.registryNumber})? История повесток сохранится, но новые повестки выписать будет нельзя.`
    )) return;

    setBusyId(citizen.id);
    setError('');

    try {
      await writeOffCitizen(citizen.id);
      await load();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось списать призывника');
    } finally {
      setBusyId(null);
    }
  }

  return (
    <>
      <header className="topbar">
        <div>
          <p className="eyebrow">{observer ? 'Личный кабинет' : 'Справочник'}</p>
          <h1>{observer ? 'Мои данные' : 'Призывники'}</h1>
        </div>
        <span className="environment">{observer ? 'Личная карточка' : `${citizens.length} записей`}</span>
      </header>

      <section className="panel">
        <div className="panel-head">
          <div>
            <h2>{observer ? 'Моя учётная карточка' : 'Учётные карточки призывников'}</h2>
            <p>
              {observer
                ? 'Сервер возвращает только данные, связанные с вашей учётной записью.'
                : 'Списание сохраняет исторические повестки, но исключает призывника из активного учёта.'}
            </p>
          </div>
        </div>

        {!observer && (
          <div className="single-filter">
            <input
              value={search}
              onChange={e => setSearch(e.target.value)}
              placeholder="Поиск по ФИО или реестровому номеру"
            />
          </div>
        )}

        {error && <div className="error">{error}</div>}

        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Реестровый номер</th>
                <th>ФИО</th>
                <th>Дата рождения</th>
                <th>Контакты</th>
                <th>Адрес регистрации</th>
                {canWriteOff && <th>Действия</th>}
              </tr>
            </thead>
            <tbody>
              {filtered.map(citizen => (
                <tr key={citizen.id}>
                  <td><strong>{citizen.registryNumber}</strong></td>
                  <td>{citizen.lastName} {citizen.firstName} {citizen.middleName ?? ''}</td>
                  <td>{new Date(citizen.birthDate).toLocaleDateString('ru-RU')}</td>
                  <td>
                    {citizen.email ?? '—'}
                    <small>{citizen.phone ?? '—'}</small>
                  </td>
                  <td>
                    {citizen.address
                      ? `${citizen.address.city}, ${citizen.address.street}, д. ${citizen.address.building}`
                      : '—'}
                  </td>
                  {canWriteOff && (
                    <td>
                      <button
                        className="danger"
                        disabled={busyId === citizen.id}
                        onClick={() => void handleWriteOff(citizen)}
                      >
                        {busyId === citizen.id ? 'Списание…' : 'Списать'}
                      </button>
                    </td>
                  )}
                </tr>
              ))}
              {!filtered.length && (
                <tr>
                  <td colSpan={canWriteOff ? 6 : 5} className="state">Данные не найдены</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>
    </>
  );
}
