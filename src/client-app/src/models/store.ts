import StoreUser from './storeUser';
import Tenant from './tenant';

interface Store {
  id: number;
  name: string;
  location: string;
  tenantId: number;
  tenant: Tenant;
  storeUser?: StoreUser[];
}

export default Store;
