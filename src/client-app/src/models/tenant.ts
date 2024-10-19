import { User } from './user';

export interface Tenant {
  id: number;
  user: User;
  tenantGuid: string;
  companyName: string;
}
