import { FormEvent, useEffect, useMemo, useState } from 'react';
import { createAccount, getAccounts } from '../api';
import { roleLabels } from '../constants';
import type { SystemAccount, UserRole } from '../types';

const assignableRoles: UserRole[] = ['Operator', 'Manager', 'Observer', 'AutomationEngineer'];

const emptyConscript = {
  registryNumber: '',
  lastName: '',
  firstName: '',
  middleName: '',
  birthDate: '2000-01-01',
  email: '',
  phone: '',
  postalCode: '',
  region: 'Тестовый регион',
  city: '',
  street: '',
  building: '',
  apartment: ''
};

export function AccountsPage() {
  const [accounts, setAccounts] = useState<SystemAccount[]>([]);
  const [displayName, setDisplayName] = useState('');
  const [role, setRole] = useState<UserRole>('Observer');
  const [conscript, setConscript] = useState(emptyConscript);
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  const isConscript = role === 'Observer';

  const conscriptDisplayName = useMemo(
    () => [conscript.lastName, conscript.firstName, conscript.middleName]
      .map(x => x.trim())
      .filter(Boolean)
      .join(' '),
    [conscript.lastName, conscript.firstName, conscript.middleName]
  );

  async function load() {
    setAccounts(await getAccounts());
  }

  useEffect(() => {
    void load().catch(e => setError(e instanceof Error ? e.message : 'Не удалось загрузить учётные записи'));
  }, []);

  function setField(field: keyof typeof emptyConscript, value: string) {
    setConscript(current => ({ ...current, [field]: value }));
  }

  async function submit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError('');

    try {
      const effectiveDisplayName = isConscript ? conscriptDisplayName : displayName.trim();

      await createAccount({
        displayName: effectiveDisplayName,
        role,
        citizen: isConscript ? {
          registryNumber: conscript.registryNumber.trim(),
          lastName: conscript.lastName.trim(),
          firstName: conscript.firstName.trim(),
          middleName: conscript.middleName.trim() || undefined,
          birthDate: conscript.birthDate,
          email: conscript.email.trim() || undefined,
          phone: conscript.phone.trim() || undefined,
          address: {
            postalCode: conscript.postalCode.trim(),
            region: conscript.region.trim(),
            city: conscript.city.trim(),
            street: conscript.street.trim(),
            building: conscript.building.trim(),
            apartment: conscript.apartment.trim() || undefined
          }
        } : undefined
      });

      setDisplayName('');
      setConscript(emptyConscript);
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
              <h2>{isConscript ? 'Создать призывника и учётную запись' : 'Создать учётную запись'}</h2>
              <p>
                Для призывника одновременно создаются карточка Citizen и SystemAccount.
                После сохранения секретарь сразу сможет выбрать его при создании повестки.
              </p>
            </div>
          </div>

          <form className="form-grid" onSubmit={submit}>
            <label className="full">
              Роль
              <select value={role} onChange={e => setRole(e.target.value as UserRole)}>
                {assignableRoles.map(item => (
                  <option key={item} value={item}>{roleLabels[item]}</option>
                ))}
              </select>
            </label>

            {isConscript ? (
              <>
                <label>
                  Фамилия
                  <input required value={conscript.lastName} onChange={e => setField('lastName', e.target.value)} />
                </label>
                <label>
                  Имя
                  <input required value={conscript.firstName} onChange={e => setField('firstName', e.target.value)} />
                </label>
                <label>
                  Отчество
                  <input value={conscript.middleName} onChange={e => setField('middleName', e.target.value)} />
                </label>
                <label>
                  Реестровый номер
                  <input required value={conscript.registryNumber} onChange={e => setField('registryNumber', e.target.value)} placeholder="например, TEST-0013" />
                </label>
                <label>
                  Дата рождения
                  <input type="date" required value={conscript.birthDate} onChange={e => setField('birthDate', e.target.value)} />
                </label>
                <label>
                  Телефон
                  <input value={conscript.phone} onChange={e => setField('phone', e.target.value)} />
                </label>
                <label className="full">
                  E-mail
                  <input type="email" value={conscript.email} onChange={e => setField('email', e.target.value)} />
                </label>

                <div className="full form-subtitle">Адрес регистрации</div>

                <label>
                  Индекс
                  <input required value={conscript.postalCode} onChange={e => setField('postalCode', e.target.value)} />
                </label>
                <label>
                  Регион
                  <input required value={conscript.region} onChange={e => setField('region', e.target.value)} />
                </label>
                <label>
                  Город
                  <input required value={conscript.city} onChange={e => setField('city', e.target.value)} />
                </label>
                <label>
                  Улица
                  <input required value={conscript.street} onChange={e => setField('street', e.target.value)} />
                </label>
                <label>
                  Дом
                  <input required value={conscript.building} onChange={e => setField('building', e.target.value)} />
                </label>
                <label>
                  Квартира
                  <input value={conscript.apartment} onChange={e => setField('apartment', e.target.value)} />
                </label>
              </>
            ) : (
              <label className="full">
                Отображаемое имя / ФИО
                <input required value={displayName} onChange={e => setDisplayName(e.target.value)} />
              </label>
            )}

            {error && <div className="error compact full">{error}</div>}

            <div className="form-actions full">
              <button
                className="primary"
                disabled={busy || (isConscript && !conscriptDisplayName)}
              >
                {busy ? 'Создание…' : isConscript ? 'Создать призывника' : 'Создать учётную запись'}
              </button>
            </div>
          </form>
        </section>

        <section className="panel">
          <div className="panel-head">
            <div>
              <h2>Созданные учётные записи</h2>
              <p>Призывники связаны с карточками по реестровому номеру.</p>
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
