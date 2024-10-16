import { User } from './user';

export interface Tenant {
  id: number;
  user: User;
  companyName: string;
}
