import type { FieldErrors } from '../validation/errors';

export function fieldProps(name: string, errors: FieldErrors) {
  return {
    id: name,
    'aria-invalid': Boolean(errors[name]),
    'aria-describedby': errors[name] ? `${name}-error` : undefined
  };
}

export function FieldError({ name, errors }: { name: string; errors: FieldErrors }) {
  const message = errors[name];
  return message ? <span id={`${name}-error`} className="field-error" role="alert">{message}</span> : null;
}

export function focusFirstInvalid(form: HTMLFormElement, errors: FieldErrors) {
  const first = Object.keys(errors)[0];
  if (first) form.querySelector<HTMLElement>(`[id="${first}"]`)?.focus();
}
