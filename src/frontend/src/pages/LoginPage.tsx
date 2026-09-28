import { FormEvent, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { login, SESSION_KEY } from '../api';
import type { Session } from '../types';

export function LoginPage({ onLogin }: { onLogin: (session: Session) => void }) {
  const navigate = useNavigate();
  const [username, setUsername] = useState('operator');
  const [password, setPassword] = useState('demo123!');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  async function submit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError('');

    try {
      const session = await login(username, password);
      localStorage.setItem(SESSION_KEY, JSON.stringify(session));
      onLogin(session);
      navigate('/dashboard', { replace: true });
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось войти');
    } finally {
      setBusy(false);
    }
  }

  return (
    <main className="login-page">
      <section className="login-card">
        <div className="brand brand-dark">ЦИПСО</div>
        <p className="eyebrow">Учебный демонстрационный контур</p>
        <h1>Реестр электронных повесток</h1>
        <p className="muted">
          Войдите под одной из демонстрационных ролей. Используются только синтетические данные.
        </p>

        <form onSubmit={submit} className="login-form">
          <label>
            Логин
            <input
              value={username}
              onChange={e => setUsername(e.target.value)}
              autoComplete="username"
            />
          </label>
          <label>
            Пароль
            <input
              type="password"
              value={password}
              onChange={e => setPassword(e.target.value)}
              autoComplete="current-password"
            />
          </label>
          {error && <div className="error compact">{error}</div>}
          <button className="primary" disabled={busy}>
            {busy ? 'Вход…' : 'Войти'}
          </button>
        </form>

        <div className="demo-credentials">
          <strong>Демо-аккаунты</strong>
          <span>operator / demo123! — чтение и изменение</span>
          <span>manager / demo123! — чтение и изменение</span>
          <span>observer / demo123! — только чтение</span>
        </div>
      </section>
    </main>
  );
}
