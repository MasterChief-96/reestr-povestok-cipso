import type { SummonsStatus, UserRole } from './types';

export const statuses: Array<SummonsStatus | ''> = [
  '', 'Draft', 'Issued', 'Delivered', 'Acknowledged', 'Completed', 'Cancelled'
];

export const statusLabels: Record<SummonsStatus, string> = {
  Draft: 'Черновик',
  Issued: 'Сформирована',
  Delivered: 'Доставлена',
  Acknowledged: 'Получение подтверждено',
  Completed: 'Исполнена',
  Cancelled: 'Отменена'
};

export const roleLabels: Record<UserRole, string> = {
  Operator: 'Секретарь',
  Manager: 'Комиссар',
  Observer: 'Призывник',
  AutomationEngineer: 'Инженер автоматизации'
};
