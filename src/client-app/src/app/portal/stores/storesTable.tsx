'use client';

import React, { useEffect, useState } from 'react';

import { ActionIcon, Group, Table } from '@mantine/core';
import { IconAdjustments } from '@tabler/icons-react';
import { AxiosError } from 'axios';

import { StoreEndpoints } from '@/constants/apiEndpoints';
import { AppGeneralError } from '@/infrastructure/exceptions';
import { useAppNotification, useAppQuery } from '@/infrastructure/hooks';
import { Store } from '@/models';

export default function StoresTable() {
  const [stores, setStores] = useState<Store[]>();
  const notification = useAppNotification();

  const { data, isLoading, isError, error } = useAppQuery<{ stores: Store[] }>({
    path: StoreEndpoints.getAll,
    queryOptions: {
      queryKey: ['get-all-stores'],
    },
  });

  useEffect(() => {
    if (isError && error) {
      var generalError = (error as AxiosError).response
        ?.data as AppGeneralError;
      notification.notifyError(
        'Unable to load stores',
        generalError.errors?.generalErrors?.join(',')
      );
    }
  }, [isError, error, notification]);

  useEffect(() => {
    if (data !== null && data?.stores != null && data?.stores.length > 0) {
      setStores(data.stores);
    }
  }, [data, isLoading]);

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
        </Group>
      </Table.Td>
      <Table.Td>{store.name}</Table.Td>
      <Table.Td>{store.location}</Table.Td>
    </Table.Tr>
  ));

  const ths = (
    <Table.Tr>
      <Table.Th>Actions</Table.Th>
      <Table.Th>Name</Table.Th>
      <Table.Th>Location</Table.Th>
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
