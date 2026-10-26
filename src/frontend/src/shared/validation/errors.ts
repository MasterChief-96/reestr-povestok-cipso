export type FieldErrors = Record<string, string>;

export function normalizeApiErrors(raw: unknown): FieldErrors {
  if (!raw || typeof raw !== 'object' || Array.isArray(raw)) return {};
  const result: FieldErrors = {};
  for (const [path, messages] of Object.entries(raw)) {
    const message = Array.isArray(messages) ? messages.find(value => typeof value === 'string') : messages;
    if (typeof message !== 'string' || !message.trim()) continue;
    const last = path.replace(/^\$\./, '').split('.').at(-1)?.replace(/\[\d+\]/g, '');
    if (last) result[last[0].toLowerCase() + last.slice(1)] = message;
  }
  return result;
}

export class ApiError extends Error {
  constructor(message: string, readonly fieldErrors: FieldErrors = {}) {
    super(message);
    this.name = 'ApiError';
  }
}
export function getApiFieldErrors(error: unknown): FieldErrors {
  return error instanceof ApiError ? error.fieldErrors : {};
}
export function errorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback;
}
