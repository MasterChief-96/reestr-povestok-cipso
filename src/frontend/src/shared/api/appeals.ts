import type { AppealListItem } from '../types';
import { apiFetch } from './client';

export async function getAppeals(): Promise<AppealListItem[]> {
  return apiFetch<AppealListItem[]>('/api/appeals');
}
