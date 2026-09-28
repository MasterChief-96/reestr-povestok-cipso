import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { getSummonsById } from '../api';
import { StatusBadge } from '../components/StatusBadge';
import { statusLabels } from '../constants';
import type { Session, SummonsDetail } from '../types';

export function SummonsDetailPage({ session }: { session: Session }) {
  const { id } = useParams();
  const [detail, setDetail] = useState<SummonsDetail | null>(null);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!id) {
      setError('Идентификатор повестки отсутствует.');
      return;
    }

    void getSummonsById(id)
      .then(setDetail)
      .catch(e => setError(e instanceof Error ? e.message : 'Не удалось открыть карточку повестки'));
  }, [id]);

  if (error) {
    return (
      <>
        <div className="error">{error}</div>
        <Link to="/summons" className="secondary button-link">Назад в реестр</Link>
      </>
    );
  }

  if (!detail) return <div className="state">Загрузка карточки…</div>;

  return (
    <>
      <header className="topbar">
        <div>
          <p className="eyebrow">Карточка повестки военного учёта</p>
          <h1>{detail.number}</h1>
        </div>
        <div className="top-actions">
          <StatusBadge status={detail.status} />
          <Link className="secondary button-link" to="/summons">← К реестру</Link>
        </div>
      </header>

      <section className="panel detail-page">
        <div className="detail-grid">
          <div>
            <span>Призывник</span>
            <strong>{detail.citizen.lastName} {detail.citizen.firstName} {detail.citizen.middleName ?? ''}</strong>
            <small>Реестровый № {detail.citizen.registryNumber}</small>
          </div>
          <div>
            <span>Военкомат</span>
            <strong>{detail.authorityOffice.name}</strong>
            <small>{detail.authorityOffice.code}</small>
          </div>
          <div>
            <span>Секретарь</span>
            <strong>{detail.createdByEmployee.fullName}</strong>
            <small>{detail.createdByEmployee.personnelNumber}</small>
          </div>
          <div>
            <span>Дата формирования</span>
            <strong>{new Date(detail.issuedAt).toLocaleDateString('ru-RU')}</strong>
          </div>
          <div>
            <span>Срок явки</span>
            <strong>{new Date(detail.dueAt).toLocaleString('ru-RU')}</strong>
          </div>
          <div>
            <span>Статус</span>
            <StatusBadge status={detail.status} />
          </div>
        </div>

        <section className="detail-section">
          <h3>Основание / причина явки</h3>
          <p>{detail.reason}</p>
          {detail.comment && <p className="muted">{detail.comment}</p>}
        </section>

        {detail.citizen.address && (
          <section className="detail-section">
            <h3>Адрес регистрации</h3>
            <p>
              {detail.citizen.address.postalCode}, {detail.citizen.address.region}, {detail.citizen.address.city},
              {' '}{detail.citizen.address.street}, д. {detail.citizen.address.building}
              {detail.citizen.address.apartment ? `, кв. ${detail.citizen.address.apartment}` : ''}
            </p>
          </section>
        )}

        <section className="detail-section">
          <h3>История статусов повестки</h3>
          <div className="timeline">
            {[...detail.statusHistory]
              .sort((a, b) => +new Date(b.changedAt) - +new Date(a.changedAt))
              .map(item => (
                <div className="timeline-item" key={item.id}>
                  <span className="timeline-dot" />
                  <div>
                    <strong>{statusLabels[item.toStatus]}</strong>
                    <small>{new Date(item.changedAt).toLocaleString('ru-RU')} · {item.changedBy}</small>
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
            <p className="muted">зафиксировано попыток оповещения</p>
          </div>
          <div>
            <h3>Обращения</h3>
            <strong className="metric">{detail.appeals.length}</strong>
            <p className="muted">обращений призывника</p>
          </div>
          <div>
            <h3>Документы</h3>
            <strong className="metric">{detail.documents.length}</strong>
            <p className="muted">сопроводительных записей</p>
          </div>
        </section>

        {!!detail.notifications.length && (
          <section className="detail-section">
            <h3>История оповещения</h3>
            <div className="cards-list">
              {detail.notifications.map(item => (
                <article className="mini-card" key={item.id}>
                  <strong>{item.channel} · {item.status}</strong>
                  <span>{item.destinationMasked}</span>
                  <small>{new Date(item.createdAt).toLocaleString('ru-RU')}</small>
                </article>
              ))}
            </div>
          </section>
        )}

        {session.role === 'Manager' && (
          <section className="detail-section">
            <h3>Журнал аудита · доступ комиссара</h3>
            <div className="cards-list">
              {[...detail.auditEvents]
                .sort((a, b) => +new Date(b.occurredAt) - +new Date(a.occurredAt))
                .map(item => (
                  <article className="mini-card" key={item.id}>
                    <strong>{item.action}</strong>
                    <span>{item.actor}</span>
                    <small>{new Date(item.occurredAt).toLocaleString('ru-RU')}</small>
                    {item.details && <small>{item.details}</small>}
                  </article>
                ))}
            </div>
          </section>
        )}
      </section>
    </>
  );
}
