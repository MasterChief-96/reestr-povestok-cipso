import type { FieldErrors } from './errors';

export type CitizenInput = Record<
  'registryNumber' | 'lastName' | 'firstName' | 'middleName' | 'birthDate' |
  'email' | 'phone' | 'postalCode' | 'region' | 'city' | 'street' |
  'building' | 'apartment', string
>;

function required(errors: FieldErrors, field: string, value: string, label: string, max: number) {
  if (!value.trim()) errors[field] = `Заполните поле «${label}».`;
  else if (value.trim().length > max) errors[field] = `Поле «${label}»: не более ${max} символов.`;
}
function optional(errors: FieldErrors, field: string, value: string, label: string, max: number) {
  if (value.trim().length > max) errors[field] = `Поле «${label}»: не более ${max} символов.`;
}

/** Проверки перед созданием учётной записи и карточки призывника. */
export function validateAccountInput(isConscript: boolean, displayName: string, citizen: CitizenInput): FieldErrors {
  const errors: FieldErrors = {};
  if (!isConscript) {
    required(errors, 'displayName', displayName, 'Отображаемое имя / ФИО', 180);
    return errors;
  }
  for (const [key, label, max] of [
    ['registryNumber', 'Реестровый номер', 64], ['lastName', 'Фамилия', 100],
    ['firstName', 'Имя', 100], ['postalCode', 'Индекс', 16],
    ['region', 'Регион', 120], ['city', 'Город', 120],
    ['street', 'Улица', 120], ['building', 'Дом', 32]
  ] as const) required(errors, key, citizen[key], label, max);
  for (const [key, label, max] of [
    ['middleName', 'Отчество', 100], ['phone', 'Телефон', 32],
    ['email', 'E-mail', 255], ['apartment', 'Квартира', 32]
  ] as const) optional(errors, key, citizen[key], label, max);

  const birth = new Date(citizen.birthDate + 'T00:00:00');
  if (!citizen.birthDate || Number.isNaN(birth.getTime()) ||
      birth.getFullYear() + '-' + String(birth.getMonth()+1).padStart(2,'0') + '-' +
      String(birth.getDate()).padStart(2,'0') !== citizen.birthDate)
    errors.birthDate = 'Укажите корректную дату рождения.';
  else if (birth.getTime() > Date.now())
    errors.birthDate = 'Дата рождения не может быть в будущем.';

  if (citizen.email.trim() && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(citizen.email.trim()))
    errors.email = 'Укажите корректный e-mail, например name@example.ru.';
  if (citizen.phone.trim() && citizen.phone.replace(/\D/g, '').length < 10)
    errors.phone = 'Телефон должен содержать не менее 10 цифр.';
  if (citizen.postalCode.trim() && !/^\d{6}$/.test(citizen.postalCode.trim()))
    errors.postalCode = 'Для адреса в России укажите индекс из 6 цифр.';
  return errors;
}

export interface SummonsInput {
  number: string;
  citizenId: string;
  officeId: string;
  issuedAt: string;
  dueAt: string;
  reason: string;
  comment: string;
  hasEmployee: boolean;
}

export function validateSummonsInput(values: SummonsInput): FieldErrors {
  const errors: FieldErrors = {};
  required(errors, 'number', values.number, 'Номер', 64);
  required(errors, 'reason', values.reason, 'Основание / причина явки', 500);
  optional(errors, 'comment', values.comment, 'Комментарий', 1000);
  if (!values.citizenId) errors.citizenId = 'Выберите призывника.';
  if (!values.officeId) errors.officeId = 'Выберите военкомат.';
  else if (!values.hasEmployee) errors.officeId = 'У выбранного военкомата нет секретаря.';
  if (!values.issuedAt || Number.isNaN(Date.parse(values.issuedAt)))
    errors.issuedAt = 'Укажите корректную дату формирования.';
  if (!values.dueAt || Number.isNaN(Date.parse(values.dueAt)))
    errors.dueAt = 'Укажите корректные дату и время явки.';
  else if (!errors.issuedAt && values.dueAt.slice(0, 10) < values.issuedAt)
    errors.dueAt = 'Срок явки не может быть раньше даты формирования.';
  return errors;
}
