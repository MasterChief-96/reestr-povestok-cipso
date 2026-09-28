import { statusLabels } from '../constants';
import type { SummonsStatus } from '../types';

export function StatusBadge({ status }: { status: SummonsStatus }) {
  return (
    <span className={`badge badge-${status.toLowerCase()}`}>
      {statusLabels[status]}
    </span>
  );
}
