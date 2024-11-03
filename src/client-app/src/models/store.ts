import StoreImage from './storeImage';
import StoreUser from './storeUser';
import Tenant from './tenant';

interface Store {
  id: number;
  createdAt?: string;
  name: string;
  location: string;
  tenantId: number;
  tenant: Tenant;
  storeUser?: StoreUser[];
  images: StoreImage[];
  obfuscatedId?: string;
}

export default Store;
