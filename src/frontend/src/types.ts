export type UserRole = 'Operator' | 'Manager' | 'Observer';
export type SummonsStatus = 'Draft' | 'Issued' | 'Delivered' | 'Acknowledged' | 'Completed' | 'Cancelled';

export interface Session {
  token: string;
  username: string;
  displayName: string;
  role: UserRole;
  expiresAt: string;
}

export interface Citizen {
  id: string;
  registryNumber: string;
  lastName: string;
  firstName: string;
  middleName?: string | null;
  birthDate: string;
  email?: string | null;
  phone?: string | null;
  address?: {
    postalCode: string;
    region: string;
    city: string;
    street: string;
    building: string;
    apartment?: string | null;
  } | null;
}

export interface Employee {
  id: string;
  personnelNumber: string;
  fullName: string;
  role: string;
}

export interface Office {
  id: string;
  code: string;
  name: string;
  region: string;
  employees: Employee[];
}

export interface SummonsListItem {
  id: string;
  number: string;
  status: SummonsStatus;
  issuedAt: string;
  dueAt: string;
  reason?: string;
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

export interface PagedSummons {
  items: SummonsListItem[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
}

export interface SummonsDetail {
  id: string;
  number: string;
  issuedAt: string;
  dueAt: string;
  reason: string;
  status: SummonsStatus;
  comment?: string | null;
  citizen: Citizen;
  authorityOffice: Office;
  createdByEmployee: Employee;
  statusHistory: Array<{
    id: string;
    fromStatus?: SummonsStatus | null;
    toStatus: SummonsStatus;
    changedAt: string;
    changedBy: string;
    comment?: string | null;
  }>;
  notifications: Array<{
    id: string;
    channel: string;
    destinationMasked: string;
    status: string;
    createdAt: string;
    deliveryAttempts: Array<{
      id: string;
      attemptNumber: number;
      result: string;
      attemptedAt: string;
      providerMessage?: string | null;
    }>;
  }>;
  appeals: Array<{
    id: string;
    type: string;
    text: string;
    status: string;
    submittedAt: string;
  }>;
  documents: Array<{
    id: string;
    fileName: string;
    mimeType: string;
    storageUri: string;
    createdAt: string;
  }>;
  auditEvents: Array<{
    id: string;
    action: string;
    actor: string;
    occurredAt: string;
    details?: string | null;
  }>;
}

export interface SummonsFilters {
  search?: string;
  status?: SummonsStatus | '';
  officeId?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateSummonsPayload {
  number: string;
  citizenId: string;
  authorityOfficeId: string;
  createdByEmployeeId: string;
  issuedAt: string;
  dueAt: string;
  reason: string;
  comment?: string;
}

export interface DashboardSummary {
  total: number;
  active: number;
  completed: number;
  cancelled: number;
  citizens: number;
  appeals: number;
  documents: number;
  byStatus: Partial<Record<SummonsStatus, number>>;
  recent: SummonsListItem[];
}

export interface AppealListItem {
  id: string;
  type: string;
  text: string;
  status: string;
  submittedAt: string;
  summons: {
    id: string;
    number: string;
    status: SummonsStatus;
  };
  citizen: {
    id: string;
    registryNumber: string;
    lastName: string;
    firstName: string;
  };
}

export interface DocumentListItem {
  id: string;
  fileName: string;
  mimeType: string;
  storageUri: string;
  createdAt: string;
  summons: {
    id: string;
    number: string;
    status: SummonsStatus;
  };
  citizen: {
    id: string;
    registryNumber: string;
    lastName: string;
    firstName: string;
  };
}
