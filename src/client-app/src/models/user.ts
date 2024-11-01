import Role from './role';
import StoreUser from './storeUser';

interface User {
  id: number;
  email: string;
  name: string;
  roleId: number;
  role?: Role;
  storeUser?: StoreUser[];
}

export default User;
