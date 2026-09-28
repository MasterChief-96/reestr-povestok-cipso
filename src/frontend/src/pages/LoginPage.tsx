import { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { externalStubLogin, getStubAccounts, SESSION_KEY } from '../api';
import { roleLabels } from '../constants';
import type { AuthProvider, Session, StubAccount } from '../types';

export function LoginPage({ onLogin }: { onLogin: (session: Session) => void }) {
  const navigate = useNavigate();
  const [accounts, setAccounts] = useState<StubAccount[]>([]);
  const [accountId, setAccountId] = useState('');
  const [error, setError] = useState('');
  const [busyProvider, setBusyProvider] = useState<AuthProvider | null>(null);

  useEffect(() => {
    void getStubAccounts()
      .then(items => {
        setAccounts(items);
        setAccountId(items[0]?.id ?? '');
      })
      .catch(e => setError(e instanceof Error ? e.message : 'Не удалось загрузить учебные учётные записи'));
  }, []);

  const selected = useMemo(
    () => accounts.find(account => account.id === accountId),
    [accounts, accountId]
  );

  async function signIn(provider: AuthProvider) {
    if (!accountId) return;
    setBusyProvider(provider);
    setError('');

    try {
      const session = await externalStubLogin(provider, accountId);
      localStorage.setItem(SESSION_KEY, JSON.stringify(session));
      onLogin(session);
      navigate(session.role === 'AutomationEngineer' ? '/accounts' : '/dashboard', { replace: true });
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось выполнить учебный вход');
    } finally {
      setBusyProvider(null);
    }
  }

  return (
    <main className="login-page">
      <section className="login-card login-card-wide">
        <div className="brand brand-dark">ЦИПСО</div>
        <p className="eyebrow">Реестр повесток военного учёта · учебный контур</p>
        <h1>Вход в систему</h1>
        <p className="muted">
          По проектному ТЗ локального логина и пароля нет. В рабочем варианте аутентификация
          выполняется через MAX либо Госуслуги с использованием «Госключа». Сейчас обе
          интеграции представлены безопасными демонстрационными заглушками.
        </p>

        <div className="stub-warning">
          <strong>Заглушка внешней аутентификации</strong>
          <span>Выберите заранее созданную учебную учётную запись, затем провайдера входа.</span>
        </div>

        <div className="account-choice">
          {accounts.map(account => (
            <button
              type="button"
              key={account.id}
              className={`account-option${account.id === accountId ? ' selected' : ''}`}
              onClick={() => setAccountId(account.id)}
            >
              <strong>{account.displayName}</strong>
              <span>{roleLabels[account.role]}</span>
              {account.citizenRegistryNumber && (
                <small>Реестровый № {account.citizenRegistryNumber}</small>
              )}
            </button>
          ))}
        </div>

        {selected && (
          <div className="selected-account">
            Вход как: <strong>{selected.displayName}</strong> · {roleLabels[selected.role]}
          </div>
        )}

        {error && <div className="error compact">{error}</div>}

        <div className="provider-actions">
          <button
            className="provider-button provider-max"
            disabled={!accountId || busyProvider !== null}
            onClick={() => void signIn('MAX')}
          >
            {busyProvider === 'MAX' ? 'Вход…' : 'Войти через MAX'}
            <small>демо-заглушка</small>
          </button>
          <button
            className="provider-button provider-gosuslugi"
            disabled={!accountId || busyProvider !== null}
            onClick={() => void signIn('Gosuslugi')}
          >
            {busyProvider === 'Gosuslugi' ? 'Вход…' : 'Войти через Госуслуги'}
            <small>«Госключ» · демо-заглушка</small>
          </button>
        </div>

        <p className="login-footnote">
          Самостоятельная регистрация отсутствует. Учётные записи создаёт инженер автоматизации.
        </p>
      </section>
    </main>
  );
}
