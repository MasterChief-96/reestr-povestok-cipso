import { FormEvent, useEffect, useState } from 'react';
import { createAccount, getAccounts } from '../api';
import { roleLabels } from '../constants';
import type { SystemAccount, UserRole } from '../types';

const assignableRoles: UserRole[] = ['Operator', 'Manager', 'Observer', 'AutomationEngineer'];

export function AccountsPage() {
  const [accounts, setAccounts] = useState<SystemAccount[]>([]);
  const [displayName, setDisplayName] = useState('');
  const [role, setRole] = useState<UserRole>('Observer');
  const [registryNumber, setRegistryNumber] = useState('');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  async function load() {
    setAccounts(await getAccounts());
  }

  useEffect(() => {
    void load().catch(e => setError(e instanceof Error ? e.message : 'Не удалось загрузить учётные записи'));
  }, []);

  async function submit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError('');

    try {
      await createAccount({
        displayName: displayName.trim(),
        role,
        citizenRegistryNumber: role === 'Observer' && registryNumber.trim()
          ? registryNumber.trim()
          : undefined
      });
      setDisplayName('');
      setRegistryNumber('');
      await load();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось создать учётную запись');
    } finally {
      setBusy(false);
    }
  }

  return (
    <>
      <header className="topbar">
        <div>
          <p className="eyebrow">Служебная функция · UC-11</p>
          <h1>Учётные записи пользователей</h1>
        </div>
        <span className="environment">Инженер автоматизации</span>
      </header>

      <div className="accounts-grid">
        <section className="panel form-panel">
          <div className="panel-head">
            <div>
              <h2>Создать учётную запись</h2>
              <p>
                Самостоятельной регистрации нет. Для учебной версии создаётся локальная запись,
                которую затем можно выбрать в заглушке MAX или Госуслуг.
              </p>
            </div>
          </div>

          <form className="form-grid" onSubmit={submit}>
            <label className="full">
              Отображаемое имя / ФИО
              <input required value={displayName} onChange={e => setDisplayName(e.target.value)} />
            </label>

            <label>
              Роль
              <select value={role} onChange={e => setRole(e.target.value as UserRole)}>
                {assignableRoles.map(item => (
                  <option key={item} value={item}>{roleLabels[item]}</option>
                ))}
              </select>
            </label>

            <label>
              Реестровый номер призывника
              <input
                disabled={role !== 'Observer'}
                value={registryNumber}
                onChange={e => setRegistryNumber(e.target.value)}
                placeholder={role === 'Observer' ? 'например, TEST-0001' : 'не требуется'}
              />
            </label>

            {error && <div className="error compact full">{error}</div>}

            <div className="form-actions full">
              <button className="primary" disabled={busy}>
                {busy ? 'Создание…' : 'Создать учётную запись'}
              </button>
            </div>
          </form>
        </section>

        <section className="panel">
          <div className="panel-head">
            <div>
              <h2>Созданные учётные записи</h2>
              <p>Используются только в демонстрационном контуре внешней аутентификации.</p>
            </div>
          </div>
          <div className="compact-list">
            {accounts.map(account => (
              <div className="account-admin-row" key={account.id}>
                <div>
                  <strong>{account.displayName}</strong>
                  <small>{roleLabels[account.role]} · {account.externalSubject}</small>
                  {account.citizenRegistryNumber && (
                    <small>Призывник: {account.citizenRegistryNumber}</small>
                  )}
                </div>
                <span className="text-badge">{account.isActive ? 'Активна' : 'Отключена'}</span>
              </div>
            ))}
          </div>
        </section>
      </div>
    </>
  );
}
