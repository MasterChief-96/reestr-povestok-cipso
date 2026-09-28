export type SummonsStatus = 'Draft' | 'Issued' | 'Delivered' | 'Acknowledged' | 'Completed' | 'Cancelled';

export interface SummonsListItem {
  id: string;
  number: string;
  status: SummonsStatus;
  issuedAt: string;
  dueAt: string;
  reason: string;
  citizen: {
    id: string;
    registryNumber: string;
    lastName: string;
    firstName: string;
  };
  office: {
    id: string;
    code: string;
    name: string;
  };
}
