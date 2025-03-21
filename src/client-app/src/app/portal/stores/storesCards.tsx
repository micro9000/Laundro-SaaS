'use client';

import React, { useEffect, useState } from 'react';

import { Grid } from '@mantine/core';
import { AxiosError } from 'axios';

import { StoreEndpoints } from '@/constants/apiEndpoints';
import { hasTenant } from '@/features/userContext/userContextSlice';
import { AppGeneralError } from '@/infrastructure/exceptions';
import { useAppNotification, useAppQuery } from '@/infrastructure/hooks';
import { Store } from '@/models';
import { useAppSelector } from '@/state/hooks';

import StoreCard from './_components/storeCard';

export default function StoresCards() {
  const userHasTenant = useAppSelector(hasTenant);
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
