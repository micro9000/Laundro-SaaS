import Role from './role';

interface User {
  id: number;
  email: string;
  roleId: number;
  Role: Role;
}

export default User;
