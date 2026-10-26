import { FormEvent, useEffect, useMemo, useState } from 'react';
import { createAccount, getAccounts, getCitizens, writeOffCitizen } from '../../../shared/api';
import { roleLabels } from '../../../shared/constants';
import type { Citizen, SystemAccount, UserRole } from '../../../shared/types';
import { errorMessage, getApiFieldErrors } from '../../../shared/validation/errors';
import type { FieldErrors } from '../../../shared/validation/errors';
import { validateAccountInput } from '../../../shared/validation/validators';
import { FieldError, fieldProps, focusFirstInvalid } from '../../../shared/ui/FieldError';

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
  const [citizens, setCitizens] = useState<Citizen[]>([]);
  const [displayName, setDisplayName] = useState('');
  const [role, setRole] = useState<UserRole>('Observer');
  const [conscript, setConscript] = useState(emptyConscript);
  const [error, setError] = useState('');
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [busy, setBusy] = useState(false);
  const [writeOffBusy, setWriteOffBusy] = useState<string | null>(null);

  const isConscript = role === 'Observer';

  const conscriptDisplayName = useMemo(
    () => [conscript.lastName, conscript.firstName, conscript.middleName]
      .map(x => x.trim())
      .filter(Boolean)
      .join(' '),
    [conscript.lastName, conscript.firstName, conscript.middleName]
  );

  const citizensByRegistry = useMemo(
    () => new Map(citizens.map(citizen => [citizen.registryNumber, citizen])),
    [citizens]
  );

  async function load() {
    const [accountItems, citizenItems] = await Promise.all([getAccounts(), getCitizens()]);
    setAccounts(accountItems);
    setCitizens(citizenItems);
  }

  useEffect(() => {
    void load().catch(e => setError(e instanceof Error ? e.message : 'Не удалось загрузить учётные записи'));
  }, []);

  function setField(field: keyof typeof emptyConscript, value: string) {
    setConscript(current => ({ ...current, [field]: value }));
    setFieldErrors(current => ({ ...current, [field]: '' }));
  }

  async function submit(event: FormEvent) {
    event.preventDefault();
    const problems = validateAccountInput(isConscript, displayName, conscript);
    setFieldErrors(problems);
    if (Object.keys(problems).length) {
      setError('Исправьте отмеченные поля и повторите попытку.');
      focusFirstInvalid(event.currentTarget, problems);
      return;
    }
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
      setFieldErrors({});
      await load();
    } catch (e) {
      const problems = getApiFieldErrors(e);
      setFieldErrors(problems);
      setError(Object.keys(problems).length
        ? 'Сервер обнаружил ошибки в отмеченных полях.'
        : errorMessage(e, 'Не удалось создать учётную запись'));
    } finally {
      setBusy(false);
    }
  }

  async function handleWriteOff(account: SystemAccount) {
    if (!account.citizenRegistryNumber) return;

    const citizen = citizensByRegistry.get(account.citizenRegistryNumber);
    if (!citizen) return;

    if (!window.confirm(
      `Списать призывника ${account.displayName} (${account.citizenRegistryNumber})? Его учётная запись будет отключена.`
    )) return;

    setWriteOffBusy(citizen.id);
    setError('');

    try {
      await writeOffCitizen(citizen.id);
      await load();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось списать призывника');
    } finally {
      setWriteOffBusy(null);
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

          <form className="form-grid" onSubmit={submit} noValidate>
            <label className="full">
              Роль
              <select {...fieldProps('role', fieldErrors)} value={role} onChange={e => { setRole(e.target.value as UserRole); setFieldErrors({}); setError(''); }}>
                {assignableRoles.map(item => (
                  <option key={item} value={item}>{roleLabels[item]}</option>
                ))}
              </select>
              <FieldError name="role" errors={fieldErrors} />
            </label>

            {isConscript ? (
              <>
                <label>
                  Фамилия
                  <input {...fieldProps('lastName', fieldErrors)} required value={conscript.lastName} onChange={e => setField('lastName', e.target.value)} />
                  <FieldError name="lastName" errors={fieldErrors} />
                </label>
                <label>
                  Имя
                  <input {...fieldProps('firstName', fieldErrors)} required value={conscript.firstName} onChange={e => setField('firstName', e.target.value)} />
                  <FieldError name="firstName" errors={fieldErrors} />
                </label>
                <label>
                  Отчество
                  <input {...fieldProps('middleName', fieldErrors)} value={conscript.middleName} onChange={e => setField('middleName', e.target.value)} />
                  <FieldError name="middleName" errors={fieldErrors} />
                </label>
                <label>
                  Реестровый номер
                  <input {...fieldProps('registryNumber', fieldErrors)} required value={conscript.registryNumber} onChange={e => setField('registryNumber', e.target.value)} placeholder="например, TEST-0013" />
                  <FieldError name="registryNumber" errors={fieldErrors} />
                </label>
                <label>
                  Дата рождения
                  <input {...fieldProps('birthDate', fieldErrors)} type="date" required value={conscript.birthDate} onChange={e => setField('birthDate', e.target.value)} />
                  <FieldError name="birthDate" errors={fieldErrors} />
                </label>
                <label>
                  Телефон
                  <input {...fieldProps('phone', fieldErrors)} value={conscript.phone} onChange={e => setField('phone', e.target.value)} />
                  <FieldError name="phone" errors={fieldErrors} />
                </label>
                <label className="full">
                  E-mail
                  <input {...fieldProps('email', fieldErrors)} type="email" value={conscript.email} onChange={e => setField('email', e.target.value)} />
                  <FieldError name="email" errors={fieldErrors} />
                </label>

                <div className="full form-subtitle">Адрес регистрации</div>

                <label>
                  Индекс
                  <input {...fieldProps('postalCode', fieldErrors)} required value={conscript.postalCode} onChange={e => setField('postalCode', e.target.value)} />
                  <FieldError name="postalCode" errors={fieldErrors} />
                </label>
                <label>
                  Регион
                  <input {...fieldProps('region', fieldErrors)} required value={conscript.region} onChange={e => setField('region', e.target.value)} />
                  <FieldError name="region" errors={fieldErrors} />
                </label>
                <label>
                  Город
                  <input {...fieldProps('city', fieldErrors)} required value={conscript.city} onChange={e => setField('city', e.target.value)} />
                  <FieldError name="city" errors={fieldErrors} />
                </label>
                <label>
                  Улица
                  <input {...fieldProps('street', fieldErrors)} required value={conscript.street} onChange={e => setField('street', e.target.value)} />
                  <FieldError name="street" errors={fieldErrors} />
                </label>
                <label>
                  Дом
                  <input {...fieldProps('building', fieldErrors)} required value={conscript.building} onChange={e => setField('building', e.target.value)} />
                  <FieldError name="building" errors={fieldErrors} />
                </label>
                <label>
                  Квартира
                  <input {...fieldProps('apartment', fieldErrors)} value={conscript.apartment} onChange={e => setField('apartment', e.target.value)} />
                  <FieldError name="apartment" errors={fieldErrors} />
                </label>
              </>
            ) : (
              <label className="full">
                Отображаемое имя / ФИО
                <input {...fieldProps('displayName', fieldErrors)} required value={displayName} onChange={e => { setDisplayName(e.target.value); setFieldErrors(current => ({ ...current, displayName: '' })); }} />
                <FieldError name="displayName" errors={fieldErrors} />
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
              <p>Инженер может списать активного призывника. Исторические повестки при этом сохраняются.</p>
            </div>
          </div>
          <div className="compact-list">
            {accounts.map(account => {
              const linkedCitizen = account.citizenRegistryNumber
                ? citizensByRegistry.get(account.citizenRegistryNumber)
                : undefined;

              return (
                <div className="account-admin-row" key={account.id}>
                  <div>
                    <strong>{account.displayName}</strong>
                    <small>{roleLabels[account.role]} · {account.externalSubject}</small>
                    {account.citizenRegistryNumber && (
                      <small>Призывник: {account.citizenRegistryNumber}</small>
                    )}
                  </div>
                  <div className="account-actions">
                    <span className="text-badge">{account.isActive ? 'Активна' : 'Отключена'}</span>
                    {account.role === 'Observer' && linkedCitizen && (
                      <button
                        className="danger"
                        disabled={writeOffBusy === linkedCitizen.id}
                        onClick={() => void handleWriteOff(account)}
                      >
                        {writeOffBusy === linkedCitizen.id ? 'Списание…' : 'Списать'}
                      </button>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        </section>
      </div>
    </>
  );
}
