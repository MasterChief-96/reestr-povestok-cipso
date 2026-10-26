import type { DocumentListItem } from '../types';
import { apiFetch } from './client';

export async function getDocuments(): Promise<DocumentListItem[]> {
  return apiFetch<DocumentListItem[]>('/api/documents');
}
