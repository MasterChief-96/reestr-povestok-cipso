import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getAppeals } from '../api';
import { StatusBadge } from '../components/StatusBadge';
import type { AppealListItem } from '../types';

export function AppealsPage() {
  const [items, setItems] = useState<AppealListItem[]>([]);
  const [error, setError] = useState('');

  useEffect(() => {
    void getAppeals()
      .then(setItems)
      .catch(e => setError(e instanceof Error ? e.message : 'Не удалось загрузить обращения'));
  }, []);

  return (
    <>
      <header className="topbar">
        <div>
          <p className="eyebrow">Связанные данные</p>
          <h1>Обращения призывников</h1>
        </div>
        <span className="environment">{items.length} записей</span>
      </header>

      <section className="panel">
        <div className="panel-head">
          <div>
            <h2>Обращения и уточнения по повесткам</h2>
            <p>Учебные обращения по срокам явки, сведениям воинского учёта и содержанию повестки.</p>
          </div>
        </div>

        {error && <div className="error">{error}</div>}

        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Дата</th>
                <th>Тип</th>
                <th>Призывник</th>
                <th>Повестка</th>
                <th>Текст</th>
                <th>Статус обращения</th>
              </tr>
            </thead>
            <tbody>
              {items.map(item => (
                <tr key={item.id}>
                  <td>{new Date(item.submittedAt).toLocaleString('ru-RU')}</td>
                  <td><strong>{item.type}</strong></td>
                  <td>
                    {item.citizen.lastName} {item.citizen.firstName}
                    <small>{item.citizen.registryNumber}</small>
                  </td>
                  <td>
                    <Link to={`/summons/${item.summons.id}`}>{item.summons.number}</Link>
                    <small><StatusBadge status={item.summons.status} /></small>
                  </td>
                  <td>{item.text}</td>
                  <td><span className="text-badge">{item.status}</span></td>
                </tr>
              ))}
              {!items.length && !error && (
                <tr><td colSpan={6} className="state">Обращений пока нет</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </section>
    </>
  );
}
