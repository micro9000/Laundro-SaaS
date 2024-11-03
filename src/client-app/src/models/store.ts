import StoreImage from './storeImage';
import StoreUser from './storeUser';
import Tenant from './tenant';

interface Store {
  id: number;
  name: string;
  location: string;
  tenantId: number;
  tenant: Tenant;
  storeUser?: StoreUser[];
  images: StoreImage[];
}

export default Store;
