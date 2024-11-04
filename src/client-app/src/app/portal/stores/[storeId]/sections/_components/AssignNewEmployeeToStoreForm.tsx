import { useEffect, useRef, useState } from 'react';

import {
  Box,
  Button,
  Group,
  LoadingOverlay,
  Select,
  Space,
} from '@mantine/core';
import { useForm } from '@mantine/form';
import { useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import {
  EmployeeEndpoints,
  RoleEndpoints,
  StoreEndpoints,
} from '@/constants/apiEndpoints';
import { hasTenant } from '@/features/userContext/userContextSlice';
import { AppGeneralError } from '@/infrastructure/exceptions';
import {
  useAppMutation,
  useAppNotification,
  useAppQuery,
} from '@/infrastructure/hooks';
import { Role, Store, User } from '@/models';
import { useAppSelector } from '@/state/hooks';
import { nameof } from '@/utilities';

interface AssignNewEmployeeFormValues {
  userId: number;
  roleId: number;
}

interface AssignNewEmployeeToStoreFormParams {
  store: Store;
  onSuccess: () => void;
}

export default function AssignNewEmployeeToStoreForm({
  store,
  onSuccess,
}: AssignNewEmployeeToStoreFormParams) {
  const userHasTenant = useAppSelector(hasTenant);

  const notification = useAppNotification();
  const notificationRef = useRef(notification); // to resolve React Hook useEffect has a missing dependency:
  const queryClient = useQueryClient();
  const queryClientRef = useRef(queryClient);

  const [employees, setEmployees] = useState<User[]>();
  const [userRoles, setUserRoles] = useState<Role[]>();

  const { data, isLoading, isError, error } = useAppQuery<{
    employees: User[];
  }>({
    path: EmployeeEndpoints.getAll,
    queryOptions: {
      queryKey: ['get-all-employees'],
      enabled: userHasTenant,
    },
  });

  useEffect(() => {
    if (isError && error && userHasTenant) {
      var generalError = (error as AxiosError).response
        ?.data as AppGeneralError;
      notificationRef.current.notifyError(
        'Unable to load employees',
        generalError.errors?.generalErrors?.join(',')
      );
    }
  }, [isError, error, userHasTenant]);

  useEffect(() => {
    if (
      data !== null &&
      data?.employees != null &&
      data?.employees.length > 0
    ) {
      setEmployees(data.employees);
    }
  }, [data, isLoading]);

  const {
    data: getRolesData,
    isLoading: getRolesIsLoading,
    isError: getRolesIsError,
    error: getRolesError,
  } = useAppQuery<{ roles: Role[] }>({
    path: RoleEndpoints.getAll,
    queryOptions: {
      queryKey: ['get-all-roles'],
      enabled: userHasTenant,
    },
  });

  useEffect(() => {
    if (getRolesIsError && getRolesError && userHasTenant) {
      var generalError = (getRolesError as AxiosError).response
        ?.data as AppGeneralError;
      notificationRef.current.notifyError(
        'Unable to load roles',
        generalError.errors?.generalErrors?.join(',')
      );
    }
  }, [getRolesIsError, getRolesError, userHasTenant]);

  useEffect(() => {
    if (
      getRolesData !== null &&
      getRolesData?.roles !== undefined &&
      getRolesData.roles.length > 0
    ) {
      setUserRoles(getRolesData.roles);
    }
  }, [getRolesData, getRolesIsLoading]);

  const {
    mutate: assignEmployeeMutate,
    isError: isAssignEmployeeError,
    isSuccess: isAssignEmployeeSuccess,
    error: assignEmployeeError,
    isPending: isAssignEmployeePending,
  } = useAppMutation({
    path: StoreEndpoints.assignEmployee,
    mutationKey: 'assign-employee-to-store',
  });

  useEffect(() => {
    if (isAssignEmployeeSuccess) {
      onSuccess();
      notificationRef.current.notifySuccess('Successfully assign new employee');
      queryClientRef.current.invalidateQueries({
        queryKey: ['get-store-details-by-id'],
      });
    }
  }, [isAssignEmployeeSuccess, onSuccess]);

  useEffect(() => {
    if (
      isAssignEmployeeError &&
      assignEmployeeError &&
      assignEmployeeError instanceof AxiosError
    ) {
      var generalError = (assignEmployeeError as AxiosError).response
        ?.data as AppGeneralError;

      notificationRef.current.notifyError(
        'Unable to assign employee',
        generalError.errors?.generalErrors?.join(',')
      );
    }
  }, [isAssignEmployeeError, assignEmployeeError]);

  const form = useForm<AssignNewEmployeeFormValues>({
    mode: 'uncontrolled',
    initialValues: {
      userId: 0,
      roleId: 0,
    },
    validate: {
      userId: (value) => (value ? null : 'Employee name is required'),
      roleId: (value) => (value ? null : 'Role is required'),
    },
  });

  const onFormSubmit = (values: AssignNewEmployeeFormValues) => {
    let formData = new FormData();

    formData.append('storeId', store.id.toString());
    formData.append(
      nameof<AssignNewEmployeeFormValues>('userId'),
      values.userId.toString()
    );
    formData.append(
      nameof<AssignNewEmployeeFormValues>('roleId'),
      values.roleId.toString()
    );

    assignEmployeeMutate(formData);
  };

  const employeeOptions = employees?.map((emp) => {
    return {
      label: emp.name,
      value: emp.id.toString(),
    };
  });

  const roleOptions = userRoles
    ?.filter((role) => role.roleLevel === 'store')
    .map((role) => {
      return {
        label: role.name,
        value: role.id.toString(),
      };
    });

  return (
    <>
      <Box pos="relative">
        <LoadingOverlay
          visible={isLoading}
          zIndex={1000}
          overlayProps={{ radius: 'sm', blur: 2 }}
        />
        <form onSubmit={form.onSubmit(onFormSubmit)}>
          <Select
            label="Select Employee"
            placeholder="Pick employee name"
            data={employeeOptions}
            searchable
            key={form.key(nameof<AssignNewEmployeeFormValues>('userId'))}
            {...form.getInputProps(
              nameof<AssignNewEmployeeFormValues>('userId')
            )}
          />
          <Space h="lg" />
          <Select
            label="Select Role"
            description="Employee's role on the selected store"
            placeholder="Pick role"
            data={roleOptions}
            searchable
            key={form.key(nameof<AssignNewEmployeeFormValues>('roleId'))}
            {...form.getInputProps(
              nameof<AssignNewEmployeeFormValues>('roleId')
            )}
          />

          <Group justify="right" mt="md">
            <Button type="submit" disabled={isAssignEmployeePending}>
              Submit
            </Button>
          </Group>
        </form>
      </Box>
    </>
  );
}
