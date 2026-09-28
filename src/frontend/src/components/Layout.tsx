import { NavLink, Outlet } from 'react-router-dom';
import { roleLabels } from '../constants';
import type { Session } from '../types';

export function Layout({
  session,
  onLogout
}: {
  session: Session;
  onLogout: () => void;
}) {
  const militaryNav = [
    { to: '/dashboard', label: 'Сводка' },
    { to: '/summons', label: 'Реестр повесток' },
    { to: '/citizens', label: 'Призывники' },
    { to: '/appeals', label: 'Обращения' },
    { to: '/documents', label: 'Документы' }
  ];

  const observerNav = [
    { to: '/dashboard', label: 'Моя сводка' },
    { to: '/summons', label: 'Мои повестки' },
    { to: '/citizens', label: 'Мои данные' },
    { to: '/appeals', label: 'Мои обращения' },
    { to: '/documents', label: 'Мои документы' }
  ];

  const navItems = session.role === 'AutomationEngineer'
    ? [{ to: '/accounts', label: 'Учётные записи' }]
    : session.role === 'Observer'
      ? observerNav
      : militaryNav;

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">ЦИПСО</div>
        <div className="brand-subtitle">Повестки военного учёта</div>

        <nav className="sidebar-nav">
          {navItems.map(item => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) => `nav-item${isActive ? ' active' : ''}`}
            >
              {item.label}
            </NavLink>
          ))}
          {session.role !== 'Observer' && (
            <a className="nav-item" href="/swagger" target="_blank" rel="noreferrer">
              Swagger API
            </a>
          )}
        </nav>

        <div className="sidebar-user">
          <strong>{session.displayName}</strong>
          <span>{roleLabels[session.role]}</span>
          <small>Вход: {session.provider === 'MAX' ? 'MAX' : 'Госуслуги / Госключ'} (заглушка)</small>
          <button onClick={onLogout}>Выйти</button>
        </div>
        <div className="demo-note">
          {session.role === 'Observer'
            ? 'Доступ ограничен только вашими данными'
            : 'Учебный проект · только синтетические данные'}
        </div>
      </aside>

      <div className="mobile-bar">
        <strong>ЦИПСО · воинский учёт</strong>
        <nav>
          {navItems.map(item => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) => `mobile-link${isActive ? ' active' : ''}`}
            >
              {item.label}
            </NavLink>
          ))}
        </nav>
      </div>

      <main className="content">
        <Outlet />
      </main>
    </div>
  );
}
