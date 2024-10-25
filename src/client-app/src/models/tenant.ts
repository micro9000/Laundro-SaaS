import User from './user';

interface Tenant {
  id: number;
  user: User;
  tenantGuid: string;
  companyName: string;
}

export default Tenant;
