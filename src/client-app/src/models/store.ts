import Tenant from './tenant';

interface Store {
  id: number;
  name: string;
  tenantId: number;
  tenant: Tenant;
}

export default Store;
