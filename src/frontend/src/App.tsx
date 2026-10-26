import { useEffect, useState } from 'react';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { readSession, SESSION_KEY } from './shared/api';
import { Layout } from './shared/layout/Layout';
import { AccountsPage } from './features/accounts/pages/AccountsPage';
import { AppealsPage } from './features/appeals/pages/AppealsPage';
import { CitizensPage } from './features/citizens/pages/CitizensPage';
import { DashboardPage } from './features/dashboard/pages/DashboardPage';
import { DocumentsPage } from './features/documents/pages/DocumentsPage';
import { LoginPage } from './features/auth/pages/LoginPage';
import { SummonsCreatePage } from './features/summons/pages/SummonsCreatePage';
import { SummonsDetailPage } from './features/summons/pages/SummonsDetailPage';
import { SummonsListPage } from './features/summons/pages/SummonsListPage';
import type { Session } from './shared/types';

export default function App() {
  const [session, setSession] = useState<Session | null>(() => readSession());

  useEffect(() => {
    const reset = () => setSession(null);
    window.addEventListener('cipso:unauthorized', reset);
    return () => window.removeEventListener('cipso:unauthorized', reset);
  }, []);

  function logout() {
    localStorage.removeItem(SESSION_KEY);
    setSession(null);
  }

  const home = session?.role === 'AutomationEngineer' ? '/accounts' : '/dashboard';

  return (
    <BrowserRouter>
      <Routes>
        <Route
          path="/login"
          element={session ? <Navigate to={home} replace /> : <LoginPage onLogin={setSession} />}
        />

        {session ? (
          <Route element={<Layout session={session} onLogout={logout} />}>
            <Route index element={<Navigate to={home} replace />} />

            {session.role === 'AutomationEngineer' ? (
              <Route path="/accounts" element={<AccountsPage />} />
            ) : (
              <>
                <Route path="/dashboard" element={<DashboardPage session={session} />} />
                <Route path="/summons" element={<SummonsListPage session={session} />} />
                <Route path="/summons/new" element={<SummonsCreatePage session={session} />} />
                <Route path="/summons/:id" element={<SummonsDetailPage session={session} />} />
                <Route path="/citizens" element={<CitizensPage session={session} />} />
                <Route path="/appeals" element={<AppealsPage />} />
                <Route path="/documents" element={<DocumentsPage />} />
              </>
            )}

            <Route path="*" element={<Navigate to={home} replace />} />
          </Route>
        ) : (
          <Route path="*" element={<Navigate to="/login" replace />} />
        )}
      </Routes>
    </BrowserRouter>
  );
}
