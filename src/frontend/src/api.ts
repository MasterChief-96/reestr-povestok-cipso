import type {
  AppealListItem,
  Citizen,
  CreateSummonsPayload,
  DashboardSummary,
  DocumentListItem,
  Office,
  PagedSummons,
  Session,
  SummonsDetail,
  SummonsFilters
} from './types';

export const SESSION_KEY = 'cipso.session';

export function readSession(): Session | null {
  const raw = localStorage.getItem(SESSION_KEY);
  if (!raw) return null;

  try {
    const session = JSON.parse(raw) as Session;
    if (new Date(session.expiresAt).getTime() <= Date.now()) {
      localStorage.removeItem(SESSION_KEY);
      return null;
    }
    return session;
  } catch {
    return null;
  }
}

async function apiFetch<T>(path: string, init: RequestInit = {}, authenticated = true): Promise<T> {
  const headers = new Headers(init.headers);

  if (init.body && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json');
  }

  if (authenticated) {
    const session = readSession();
    if (session?.token) {
      headers.set('Authorization', `Bearer ${session.token}`);
    }
  }

  const response = await fetch(path, { ...init, headers });

  if (response.status === 401) {
    localStorage.removeItem(SESSION_KEY);
    window.dispatchEvent(new Event('cipso:unauthorized'));
    throw new Error('Сессия истекла. Войдите снова.');
  }

  if (!response.ok) {
    let message = `Ошибка запроса (${response.status})`;
    try {
      const body = await response.json() as { message?: string; title?: string };
      message = body.message ?? body.title ?? message;
    } catch {
      // Response does not contain JSON.
    }
    throw new Error(message);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

export async function login(username: string, password: string): Promise<Session> {
  return apiFetch<Session>('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify({ username, password })
  }, false);
}

export async function getDashboard(): Promise<DashboardSummary> {
  return apiFetch<DashboardSummary>('/api/dashboard');
}

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

export async function getCitizens(): Promise<Citizen[]> {
  return apiFetch<Citizen[]>('/api/citizens');
}

export async function getOffices(): Promise<Office[]> {
  return apiFetch<Office[]>('/api/offices');
}

export async function getAppeals(): Promise<AppealListItem[]> {
  return apiFetch<AppealListItem[]>('/api/appeals');
}

export async function getDocuments(): Promise<DocumentListItem[]> {
  return apiFetch<DocumentListItem[]>('/api/documents');
}

export async function createSummons(
  payload: CreateSummonsPayload
): Promise<{ id: string; number: string; status: string }> {
  return apiFetch<{ id: string; number: string; status: string }>('/api/summons', {
    method: 'POST',
    body: JSON.stringify(payload)
  });
}
