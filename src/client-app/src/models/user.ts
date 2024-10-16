import { Role } from './role';

export interface User {
  id: number;
  email: string;
  roleId: number;
  Role: Role;
}
