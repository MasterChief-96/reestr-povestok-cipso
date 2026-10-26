import type { Citizen } from '../types';
import { apiFetch } from './client';

export async function getCitizens(): Promise<Citizen[]> {
  return apiFetch<Citizen[]>('/api/citizens');
}

export async function writeOffCitizen(id: string): Promise<void> {
  return apiFetch<void>(`/api/citizens/${id}`, {
    method: 'DELETE'
  });
}
