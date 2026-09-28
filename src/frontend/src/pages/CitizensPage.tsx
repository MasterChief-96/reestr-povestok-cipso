import { useEffect, useMemo, useState } from 'react';
import { getCitizens } from '../api';
import type { Citizen } from '../types';

export function CitizensPage() {
  const [citizens, setCitizens] = useState<Citizen[]>([]);
  const [search, setSearch] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    void getCitizens()
      .then(setCitizens)
      .catch(e => setError(e instanceof Error ? e.message : 'Не удалось загрузить призывников'));
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

  return (
    <>
      <header className="topbar">
        <div>
          <p className="eyebrow">Справочник</p>
          <h1>Призывники</h1>
        </div>
        <span className="environment">{citizens.length} записей</span>
      </header>

      <section className="panel">
        <div className="panel-head">
          <div>
            <h2>Учётные карточки призывников</h2>
            <p>Все персональные данные синтетические и используются только в учебном контуре.</p>
          </div>
        </div>

        <div className="single-filter">
          <input
            value={search}
            onChange={e => setSearch(e.target.value)}
            placeholder="Поиск по ФИО или реестровому номеру"
          />
        </div>

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
              </tr>
            </thead>
            <tbody>
              {filtered.map(citizen => (
                <tr key={citizen.id}>
                  <td><strong>{citizen.registryNumber}</strong></td>
                  <td>
                    {citizen.lastName} {citizen.firstName} {citizen.middleName ?? ''}
                  </td>
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
                </tr>
              ))}
              {!filtered.length && (
                <tr><td colSpan={5} className="state">Ничего не найдено</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </section>
    </>
  );
}
