import type { SummonsStatus } from './types';

export const statuses: Array<SummonsStatus | ''> = [
  '',
  'Draft',
  'Issued',
  'Delivered',
  'Acknowledged',
  'Completed',
  'Cancelled'
];

export const statusLabels: Record<SummonsStatus, string> = {
  Draft: 'Черновик',
  Issued: 'Выпущена',
  Delivered: 'Доставлена',
  Acknowledged: 'Подтверждена',
  Completed: 'Завершена',
  Cancelled: 'Отменена'
};
