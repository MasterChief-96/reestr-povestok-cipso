import type { SummonsListItem } from './types';

export async function getSummons(search = ''): Promise<SummonsListItem[]> {
  const query = search ? `?search=${encodeURIComponent(search)}` : '';
  const response = await fetch(`/api/summons${query}`);
  if (!response.ok) throw new Error('Не удалось загрузить реестр');
  return response.json();
}
