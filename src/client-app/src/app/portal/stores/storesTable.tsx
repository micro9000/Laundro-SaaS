'use client';

import React, { useEffect, useState } from 'react';

import { ActionIcon, Badge, Group, Table } from '@mantine/core';
import { IconAdjustments, IconUsersGroup } from '@tabler/icons-react';
import { AxiosError } from 'axios';

import { RoleEndpoints, StoreEndpoints } from '@/constants/apiEndpoints';
import { hasTenant } from '@/features/userContext/userContextSlice';
import { AppGeneralError } from '@/infrastructure/exceptions';
import { useAppNotification, useAppQuery } from '@/infrastructure/hooks';
import { Role, Store, StoreUser } from '@/models';
import { useAppSelector } from '@/state/hooks';

export default function StoresTable() {
  const userHasTenant = useAppSelector(hasTenant);
  const [userRoles, setUserRoles] = useState<Role[]>();
  const [stores, setStores] = useState<Store[]>();
  const notification = useAppNotification();

  const {
    data: getStoresData,
    isLoading: getStoresIsLoading,
    isError: getStoresIsError,
    error: getStoresError,
  } = useAppQuery<{ stores: Store[] }>({
    path: StoreEndpoints.getAll,
    queryOptions: {
      queryKey: ['get-all-stores'],
      enabled: userHasTenant,
    },
  });

  const {
    data: getRolesData,
    isLoading: getRolesIsLoading,
    isError: getRolesIsError,
    error: getRolesError,
  } = useAppQuery<{ stores: Store[] }>({
    path: RoleEndpoints.getAll,
    queryOptions: {
      queryKey: ['get-all-roles'],
      enabled: userHasTenant,
    },
  });

  useEffect(() => {
    if (getStoresIsError && getStoresError && userHasTenant) {
      var generalError = (getStoresError as AxiosError).response
        ?.data as AppGeneralError;
      notification.notifyError(
        'Unable to load stores',
        generalError.errors?.generalErrors?.join(',')
      );
    }
  }, [getStoresIsError, getStoresError, notification, userHasTenant]);

  useEffect(() => {
    if (
      getStoresData !== null &&
      getStoresData?.stores != null &&
      getStoresData?.stores.length > 0
    ) {
      setStores(getStoresData.stores);
    }
  }, [getStoresData, getStoresIsLoading]);

  const employees = (storeUsers?: StoreUser[]) => {
    return storeUsers?.map((su) => (
      <Badge key={su.userId}>{su.user?.name}</Badge>
    ));
  };

  const rows = stores?.map((store) => (
    <Table.Tr key={store.id}>
      <Table.Td>
        <Group justify="center">
          <ActionIcon variant="default" aria-label="Settings">
            <IconAdjustments
              style={{ width: '70%', height: '70%' }}
              stroke={1.5}
            />
          </ActionIcon>
          <ActionIcon variant="default" aria-label="Employees">
            <IconUsersGroup
              style={{ width: '70%', height: '70%' }}
              stroke={1.5}
            />
          </ActionIcon>
        </Group>
      </Table.Td>
      <Table.Td>{store.name}</Table.Td>
      <Table.Td>{store.location}</Table.Td>
      <Table.Td>{employees(store.storeUser)}</Table.Td>
    </Table.Tr>
  ));

  const ths = (
    <Table.Tr>
      <Table.Th>Actions</Table.Th>
      <Table.Th>Name</Table.Th>
      <Table.Th>Location</Table.Th>
      <Table.Th>Employees</Table.Th>
    </Table.Tr>
  );

  return (
    <Table
      captionSide="bottom"
      striped
      highlightOnHover
      withTableBorder
      withColumnBorders
      stickyHeader
      stickyHeaderOffset={60}
    >
      <Table.Caption>Stores in your tenant</Table.Caption>
      <Table.Thead>{ths}</Table.Thead>
      <Table.Tbody>{rows}</Table.Tbody>
    </Table>
  );
}
