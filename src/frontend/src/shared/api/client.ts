import type { Session } from '../types';
import { ApiError, normalizeApiErrors } from '../validation/errors';
import type { FieldErrors } from '../validation/errors';

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

export async function apiFetch<T>(path: string, init: RequestInit = {}, authenticated = true): Promise<T> {
  const headers = new Headers(init.headers);
  if (init.body && !headers.has('Content-Type')) headers.set('Content-Type', 'application/json');

  if (authenticated) {
    const session = readSession();
    if (session?.token) headers.set('Authorization', `Bearer ${session.token}`);
  }

  const response = await fetch(path, { ...init, headers });

  if (response.status === 401) {
    localStorage.removeItem(SESSION_KEY);
    window.dispatchEvent(new Event('cipso:unauthorized'));
    throw new Error('Сессия истекла. Выполните вход через MAX или Госуслуги снова.');
  }

  if (!response.ok) {
    let message = `Ошибка запроса (${response.status})`;
    let fieldErrors: FieldErrors = {};
    try {
      const body = await response.json() as { message?: string; title?: string; errors?: Record<string, unknown> };
      message = body.message ?? body.title ?? message;
      fieldErrors = normalizeApiErrors(body.errors);
    } catch {
      // Ответ сервера может не содержать JSON.
    }
    throw new ApiError(message, fieldErrors);
  }

  if (response.status === 204) return undefined as T;
  return response.json() as Promise<T>;
}
