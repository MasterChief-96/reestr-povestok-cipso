import test from 'node:test';
import assert from 'node:assert/strict';
import { validateAccountInput, validateSummonsInput } from '../src/shared/validation/validators.ts';
import { normalizeApiErrors } from '../src/shared/validation/errors.ts';

const person = {
  registryNumber: 'TEST-0001', lastName: 'Иванов', firstName: 'Иван', middleName: '',
  birthDate: '2000-01-01', email: 'ivan@example.ru', phone: '+79991234567',
  postalCode: '123456', region: 'Регион', city: 'Город', street: 'Улица',
  building: '10', apartment: ''
};
test('ошибки призывника привязаны к конкретным полям', () => {
  const errors = validateAccountInput(true, '', { ...person, lastName: ' ', email: 'не email', postalCode: '123' });
  assert.match(errors.lastName, /Фамилия/);
  assert.match(errors.email, /e-mail/);
  assert.match(errors.postalCode, /6 цифр/);
  assert.deepEqual(validateAccountInput(true, '', person), {});
});
test('дата явки раньше даты формирования запрещена', () => {
  const errors = validateSummonsInput({
    number: 'TEST-0001', citizenId: 'citizen', officeId: 'office', issuedAt: '2026-10-26',
    dueAt: '2026-10-25T12:00', reason: 'Причина', comment: '', hasEmployee: true
  });
  assert.match(errors.dueAt, /раньше даты/);
});
test('вложенные ошибки API попадают в нужное поле формы', () => {
  assert.deepEqual(normalizeApiErrors({ 'request.Citizen.Address.PostalCode': ['Некорректный индекс'] }),
    { postalCode: 'Некорректный индекс' });
});
