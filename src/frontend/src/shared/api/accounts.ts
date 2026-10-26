import type { SystemAccount, UserRole } from '../types';
import { apiFetch } from './client';

export async function getAccounts(): Promise<SystemAccount[]> {
  return apiFetch<SystemAccount[]>('/api/accounts');
}

export async function createAccount(payload: {
  displayName: string;
  role: UserRole;
  citizen?: {
    registryNumber: string;
    lastName: string;
    firstName: string;
    middleName?: string;
    birthDate: string;
    email?: string;
    phone?: string;
    address: {
      postalCode: string;
      region: string;
      city: string;
      street: string;
      building: string;
      apartment?: string;
    };
  };
}): Promise<SystemAccount> {
  return apiFetch<SystemAccount>('/api/accounts', {
    method: 'POST',
    body: JSON.stringify(payload)
  });
}
