import type { StubAccount, AuthProvider, Session } from '../types';
import { apiFetch } from './client';

export async function getStubAccounts(): Promise<StubAccount[]> {
  return apiFetch<StubAccount[]>('/api/auth/stub/accounts', {}, false);
}

export async function externalStubLogin(provider: AuthProvider, accountId: string): Promise<Session> {
  return apiFetch<Session>('/api/auth/stub/external', {
    method: 'POST',
    body: JSON.stringify({ provider, accountId })
  }, false);
}
