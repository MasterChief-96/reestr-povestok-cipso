import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getDocuments } from '../api';
import { StatusBadge } from '../components/StatusBadge';
import type { DocumentListItem } from '../types';

export function DocumentsPage() {
  const [items, setItems] = useState<DocumentListItem[]>([]);
  const [error, setError] = useState('');

  useEffect(() => {
    void getDocuments()
      .then(setItems)
      .catch(e => setError(e instanceof Error ? e.message : 'Не удалось загрузить документы'));
  }, []);

  return (
    <>
      <header className="topbar">
        <div>
          <p className="eyebrow">Связанные данные</p>
          <h1>Документы призывного учёта</h1>
        </div>
        <span className="environment">{items.length} записей</span>
      </header>

      <section className="panel">
        <div className="panel-head">
          <div>
            <h2>Сопроводительные документы</h2>
            <p>В MVP хранятся только метаданные документов воинского учёта; реальные файлы не принимаются.</p>
          </div>
        </div>

        {error && <div className="error">{error}</div>}

        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Создан</th>
                <th>Имя файла</th>
                <th>MIME type</th>
                <th>Призывник</th>
                <th>Повестка</th>
                <th>Storage URI</th>
              </tr>
            </thead>
            <tbody>
              {items.map(item => (
                <tr key={item.id}>
                  <td>{new Date(item.createdAt).toLocaleString('ru-RU')}</td>
                  <td><strong>{item.fileName}</strong></td>
                  <td>{item.mimeType}</td>
                  <td>
                    {item.citizen.lastName} {item.citizen.firstName}
                    <small>{item.citizen.registryNumber}</small>
                  </td>
                  <td>
                    <Link to={`/summons/${item.summons.id}`}>{item.summons.number}</Link>
                    <small><StatusBadge status={item.summons.status} /></small>
                  </td>
                  <td><code>{item.storageUri}</code></td>
                </tr>
              ))}
              {!items.length && !error && (
                <tr><td colSpan={6} className="state">Документов пока нет</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </section>
    </>
  );
}
