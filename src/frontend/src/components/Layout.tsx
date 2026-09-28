import { NavLink, Outlet } from 'react-router-dom';
import type { Session } from '../types';

const navItems = [
  { to: '/dashboard', label: 'Сводка' },
  { to: '/summons', label: 'Реестр повесток' },
  { to: '/citizens', label: 'Граждане' },
  { to: '/appeals', label: 'Обращения' },
  { to: '/documents', label: 'Документы' }
];

export function Layout({
  session,
  onLogout
}: {
  session: Session;
  onLogout: () => void;
}) {
  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">ЦИПСО</div>
        <div className="brand-subtitle">Реестр повесток</div>

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
          <a className="nav-item" href="/swagger" target="_blank" rel="noreferrer">
            Swagger API
          </a>
        </nav>

        <div className="sidebar-user">
          <strong>{session.displayName}</strong>
          <span>{session.role}</span>
          <button onClick={onLogout}>Выйти</button>
        </div>
        <div className="demo-note">Учебный контур · синтетические данные</div>
      </aside>

      <div className="mobile-bar">
        <strong>ЦИПСО</strong>
        <nav>
          {navItems.slice(0, 5).map(item => (
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
