import { Config } from '@/infrastructure/config';

export const RoleEndpoints = {
  getAll: '/role/get-all-roles',
};

export const UserContextEndpoints = {
  get: '/user-context-state/get',
};

export const TenantEndpoints = {
  create: '/tenant/create-new-tenant',
};

export const StoreEndpoints = {
  create: '/store/create-new-store',
  update: '/store/update-store',
  getAll: '/store/get-all-stores',
  get: '/store/get-store',
  assignEmployee: '/store/assign-employee',
  unassignEmployee: '/store/unassign-employee',
};

export const EmployeeEndpoints = {
  register: '/employee/register',
  getAll: '/employee/get-all-employees',
};

export const GenerateStoreImageUrl = (
  storeId?: number,
  imageId?: number,
  tenantGuid?: string
) => {
  const apiUrl = Config.ApiUrl;
  return `${apiUrl}/store/images/content/${tenantGuid}/${storeId}/${imageId}`;
};
