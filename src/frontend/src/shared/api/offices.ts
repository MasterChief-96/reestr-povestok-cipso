import type { Office } from '../types';
import { apiFetch } from './client';

export async function getOffices(): Promise<Office[]> {
  return apiFetch<Office[]>('/api/offices');
}
