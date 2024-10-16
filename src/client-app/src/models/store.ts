import { Tenant } from './tenant';

export interface Store {
  id: number;
  name: string;
  tenantId: number;
  tenant: Tenant;
}
