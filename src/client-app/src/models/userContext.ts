import { Role } from './role';
import { Store } from './store';
import { Tenant } from './tenant';

export interface UserContext {
  userId: number;
  email: string;
  tenant?: Tenant;
  isTenantOwner?: boolean;
  role?: Role;
  stores?: Store[];
}
