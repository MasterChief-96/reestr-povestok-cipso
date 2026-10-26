import type { SummonsFilters, PagedSummons, SummonsDetail, CreateSummonsPayload } from '../types';
import { apiFetch } from './client';

export async function getSummons(filters: SummonsFilters): Promise<PagedSummons> {
  const params = new URLSearchParams();
  if (filters.search) params.set('search', filters.search);
  if (filters.status) params.set('status', filters.status);
  if (filters.officeId) params.set('officeId', filters.officeId);
  params.set('page', String(filters.page ?? 1));
  params.set('pageSize', String(filters.pageSize ?? 10));
  return apiFetch<PagedSummons>(`/api/summons?${params.toString()}`);
}

export async function getSummonsById(id: string): Promise<SummonsDetail> {
  return apiFetch<SummonsDetail>(`/api/summons/${id}`);
}

export async function createSummons(
  payload: CreateSummonsPayload
): Promise<{ id: string; number: string; status: string }> {
  return apiFetch<{ id: string; number: string; status: string }>('/api/summons', {
    method: 'POST',
    body: JSON.stringify(payload)
  });
}
