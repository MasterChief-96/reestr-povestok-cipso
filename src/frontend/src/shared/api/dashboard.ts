import type { DashboardSummary } from '../types';
import { apiFetch } from './client';

export async function getDashboard(): Promise<DashboardSummary> {
  return apiFetch<DashboardSummary>('/api/dashboard');
}
