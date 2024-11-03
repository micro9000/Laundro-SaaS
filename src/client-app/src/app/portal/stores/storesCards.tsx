'use client';

import React, { useEffect, useState } from 'react';

import { ActionIcon, Badge, Grid, Group, Table } from '@mantine/core';
import { IconAdjustments, IconUsersGroup } from '@tabler/icons-react';
import { AxiosError } from 'axios';

import { RoleEndpoints, StoreEndpoints } from '@/constants/apiEndpoints';
import { hasTenant } from '@/features/userContext/userContextSlice';
import { AppGeneralError } from '@/infrastructure/exceptions';
import { useAppNotification, useAppQuery } from '@/infrastructure/hooks';
import { Role, Store, StoreUser } from '@/models';
import { useAppSelector } from '@/state/hooks';

import StoreCard from './components/storeCard';

export default function StoresCards() {
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
      getStoresData?.stores !== undefined &&
      getStoresData?.stores?.length > 0
    ) {
      setStores(getStoresData.stores);
    }
  }, [getStoresData, getStoresIsLoading]);

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
      notification.notifyError(
        'Unable to load roles',
        generalError.errors?.generalErrors?.join(',')
      );
    }
  }, [getRolesIsError, getRolesError, notification, userHasTenant]);

  useEffect(() => {
    if (
      getRolesData !== null &&
      getRolesData?.roles !== undefined &&
      getRolesData.roles.length > 0
    ) {
      setUserRoles(getRolesData.roles);
    }
  }, [getRolesData, getRolesIsLoading]);

  const employees = (storeUsers?: StoreUser[]) => {
    return storeUsers?.map((su) => (
      <Badge key={su.userId}>{su.user?.name}</Badge>
    ));
  };

  return (
    <>
      <Grid>
        {stores?.map((store) => (
          <Grid.Col span={4} key={store.id}>
            <StoreCard store={store} />
          </Grid.Col>
        ))}
      </Grid>
    </>
  );
}
