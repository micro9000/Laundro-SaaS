import { useEffect, useState } from 'react';

import {
  Button,
  Container,
  Drawer,
  Group,
  Space,
  Table,
  Text,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { modals } from '@mantine/modals';
import { IconPlus, IconTrashX } from '@tabler/icons-react';
import { useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import { StoreEndpoints } from '@/constants/apiEndpoints';
import { AppGeneralError } from '@/infrastructure/exceptions';
import { useAppMutation, useAppNotification } from '@/infrastructure/hooks';
import { Store, StoreUser, User } from '@/models';

import AssignNewEmployeeToStoreForm from './_components/AssignNewEmployeeToStoreForm';

export default function EmployeesSection({ store }: { store?: Store | null }) {
  const notification = useAppNotification();
  const queryClient = useQueryClient();
  const [employees, setEmployees] = useState<StoreUser[]>([]);

  const [opened, { open, close }] = useDisclosure(false);

  useEffect(() => {
    if (store && store?.storeUser && store?.storeUser?.length > 0) {
      setEmployees(store?.storeUser);
    }
  }, [store, store?.storeUser]);

  const {
    mutate: unassignEmployeeMutate,
    isError: isUnassignEmployeeError,
    isSuccess: isUnassignEmployeeSuccess,
    error: unAssignEmployeeError,
    isPending: isUnassignEmployeePending,
  } = useAppMutation({
    path: StoreEndpoints.unassignEmployee,
    mutationKey: 'un-assign-employee-to-store',
  });

  useEffect(() => {
    if (isUnassignEmployeeSuccess && !isUnassignEmployeePending) {
      notification.notifySuccess('Successfully un-assign employee');
      queryClient.invalidateQueries({ queryKey: ['get-store-details-by-id'] });
    }
  }, [isUnassignEmployeeSuccess, isUnassignEmployeePending, queryClient, notification]);

  useEffect(() => {
    if (
      isUnassignEmployeeError &&
      unAssignEmployeeError &&
      unAssignEmployeeError instanceof AxiosError
    ) {
      var generalError = (unAssignEmployeeError as AxiosError).response
        ?.data as AppGeneralError;

      notification.notifyError(
        'Unable to update store',
        generalError.errors?.generalErrors?.join(',')
      );
    }
  }, [isUnassignEmployeeError, unAssignEmployeeError, notification]);

  const onConfirmUnassignEmployee = (userId?: number, roleId?: number) => {
    let formData = new FormData();

    formData.append('userId', userId?.toString() ?? '0');
    formData.append('roleId', roleId?.toString() ?? '0');
    formData.append('storeId', store?.id?.toString() ?? '0');

    unassignEmployeeMutate(formData);
  };

  const openDeleteModal = (userId?: number, roleId?: number) =>
    modals.openConfirmModal({
      title: 'Unassign employee',
      centered: true,
      children: (
        <Text size="sm">Are you sure you want to unassign this employee?</Text>
      ),
      labels: { confirm: 'Continue', cancel: 'Cancel' },
      confirmProps: { color: 'red' },
      // onCancel: () => console.log('Cancel', userId),
      onConfirm: () => onConfirmUnassignEmployee(userId, roleId),
    });

  const rows = employees?.map((emp) => (
    <Table.Tr key={emp.userId}>
      <Table.Td>{emp.user?.email}</Table.Td>
      <Table.Td>{emp.user?.name}</Table.Td>
      <Table.Td>{emp.role?.name}</Table.Td>
      <Table.Td>
        <Button
          leftSection={<IconTrashX size={14} />}
          onClick={() => openDeleteModal(emp.userId, emp.roleId)}
          variant="subtle"
        >
          Unassign
        </Button>
      </Table.Td>
    </Table.Tr>
  ));

  return (
    <>
      <Container size="xl">
        <Drawer
          offset={4}
          // radius="md"
          size="lg"
          opened={opened}
          onClose={close}
          title="Assign New Employee"
          position="right"
        >
          <AssignNewEmployeeToStoreForm store={store!} onSuccess={close} />
        </Drawer>
        <Text fw={700}>Employees</Text>
        <Space h="lg" />
        <Group justify="right">
          <Button
            leftSection={<IconPlus size={14} />}
            variant="subtle"
            onClick={open}
          >
            Assign New Employee On This Store
          </Button>
        </Group>
        <Space h="md" />

        <Table withRowBorders>
          <Table.Thead>
            <Table.Tr>
              <Table.Th>Email</Table.Th>
              <Table.Th>Name</Table.Th>
              <Table.Th>Role on this store</Table.Th>
              <Table.Th>Actions</Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>{rows}</Table.Tbody>
        </Table>
      </Container>
    </>
  );
}
