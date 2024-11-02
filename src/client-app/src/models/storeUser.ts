import Role from './role';
import Store from './store';
import User from './user';

interface StoreUser {
  storeId?: number;
  store?: Store;
  roleId?: number;
  role?: Role;
  userId?: number;
  user?: User;
}

export default StoreUser;
