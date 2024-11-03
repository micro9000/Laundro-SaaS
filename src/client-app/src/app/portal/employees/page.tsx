'use client';

import Link from 'next/link';
import React, { useEffect, useState } from 'react';

import {
  ActionIcon,
  Badge,
  Button,
  Container,
  Group,
  Space,
  Table,
} from '@mantine/core';
import { IconAdjustments, IconLogs, IconPlus } from '@tabler/icons-react';
import { AxiosError } from 'axios';

import { EmployeeEndpoints } from '@/constants/apiEndpoints';
import { hasTenant } from '@/features/userContext/userContextSlice';
import { AppGeneralError } from '@/infrastructure/exceptions';
import { useAppNotification, useAppQuery } from '@/infrastructure/hooks';
import { StoreUser, User } from '@/models';
import { useAppSelector } from '@/state/hooks';

export default function Page() {
  const userHasTenant = useAppSelector(hasTenant);
  const notification = useAppNotification();

  const [employees, setEmployees] = useState<User[]>();

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
      notification.notifyError(
        'Unable to load employees',
        generalError.errors?.generalErrors?.join(',')
      );
    }
  }, [isError, error, notification, userHasTenant]);

  useEffect(() => {
    if (
      data !== null &&
      data?.employees != null &&
      data?.employees.length > 0
    ) {
      setEmployees(data.employees);
    }
  }, [data, isLoading]);

  const assignedStores = (storeUsers?: StoreUser[]) => {
    return storeUsers?.map((su) => (
      <Badge key={su.store?.id}>
        [{su.store?.name}] - [{su.role?.name}]
      </Badge>
    ));
  };

  const rows = employees?.map((emp) => (
    <Table.Tr key={emp.id}>
      <Table.Td>
        <ActionIcon.Group>
          <ActionIcon variant="default" aria-label="Settings">
            <IconAdjustments
              style={{ width: '70%', height: '70%' }}
              stroke={1.5}
            />
          </ActionIcon>
          <ActionIcon variant="default" aria-label="Settings">
            <IconLogs style={{ width: '70%', height: '70%' }} stroke={1.5} />
          </ActionIcon>
        </ActionIcon.Group>
      </Table.Td>
      <Table.Td>{emp.name}</Table.Td>
      <Table.Td>{emp.email}</Table.Td>
      <Table.Td>{assignedStores(emp.storeUser)}</Table.Td>
    </Table.Tr>
  ));

  const ths = (
    <Table.Tr>
      <Table.Th>Actions</Table.Th>
      <Table.Th>Name</Table.Th>
      <Table.Th>Email</Table.Th>
      <Table.Th>Assigned Stores</Table.Th>
    </Table.Tr>
  );

  return (
    <Container size="lg">
      <Group justify="right">
        <Button
          leftSection={<IconPlus size={14} />}
          variant="subtle"
          component={Link}
          href="/portal/employees/register"
        >
          Add New Employee
        </Button>
      </Group>
      <Space h="md" />
      <Table
        captionSide="bottom"
        striped
        highlightOnHover
        withTableBorder
        withColumnBorders
        stickyHeader
        stickyHeaderOffset={60}
      >
        <Table.Caption>Your Employees</Table.Caption>
        <Table.Thead>{ths}</Table.Thead>
        <Table.Tbody>{rows}</Table.Tbody>
      </Table>
    </Container>
  );
}
