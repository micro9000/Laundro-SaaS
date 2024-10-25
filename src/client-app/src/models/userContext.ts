import Role from './role';
import Store from './store';
import Tenant from './tenant';

interface UserContext {
  userId: number;
  email: string;
  tenantName?: string;
  tenantGuid: string;
  isTenantOwner?: boolean;
  roleSystemKey?: string;
  stores?: Store[];
}

export default UserContext;
