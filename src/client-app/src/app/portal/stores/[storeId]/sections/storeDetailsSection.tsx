import Link from 'next/link';
import { useEffect, useState } from 'react';

import {
  Button,
  Container,
  Group,
  Space,
  Text,
  TextInput,
} from '@mantine/core';
import { useForm } from '@mantine/form';
import { useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import { StoreEndpoints } from '@/constants/apiEndpoints';
import { AppGeneralError } from '@/infrastructure/exceptions';
import { useAppMutation, useAppNotification } from '@/infrastructure/hooks';
import { Store } from '@/models';
import { nameof } from '@/utilities';

interface UpdateStoreDetailsFormValues {
  storeId: string;
  name: string;
  location: string;
}

export default function StoreDetailsSection({
  store,
}: {
  store?: Store | null;
}) {
  const notification = useAppNotification();
  const queryClient = useQueryClient();
  const [storeCreatedAt, setStoreCreatedAt] = useState<Date | null>(null);

  const form = useForm<UpdateStoreDetailsFormValues>({
    mode: 'controlled',
    initialValues: {
      storeId: '0',
      name: '',
      location: '',
    },
    validate: {
      storeId: (value) => (value ? null : 'Store Id is required'),
      name: (value) => (value ? null : 'Store name is required'),
      location: (value) => (value ? null : 'Store location is required'),
    },
  });

  useEffect(() => {
    if (store?.createdAt) {
      setStoreCreatedAt(new Date(store!.createdAt!));
    }
  }, [store, store?.createdAt]);

  useEffect(() => {
    if (store) {
      form.setValues({
        name: store.name,
        location: store.location,
        storeId: store.id.toString(),
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [store]);

  const { mutate, isError, isSuccess, error, isPending } = useAppMutation({
    path: StoreEndpoints.update,
    mutationKey: 'update-store-details',
  });

  useEffect(() => {
    if (isSuccess && !isPending) {
      notification.notifySuccess('Store details has successful updated');
      queryClient.invalidateQueries({ queryKey: ['get-store-details-by-id'] });
    }
  }, [isSuccess, notification, queryClient, isPending]);

  useEffect(() => {
    if (isError && error && error instanceof AxiosError) {
      var generalError = (error as AxiosError).response
        ?.data as AppGeneralError;

      notification.notifyError(
        'Unable to update store',
        generalError.errors?.generalErrors?.join(',')
      );
    }
  }, [isError, error, notification]);

  const onFormSubmit = (values: UpdateStoreDetailsFormValues) => {
    let formData = new FormData();

    formData.append(
      nameof<UpdateStoreDetailsFormValues>('storeId'),
      values.storeId
    );
    formData.append(nameof<UpdateStoreDetailsFormValues>('name'), values.name);
    formData.append(
      nameof<UpdateStoreDetailsFormValues>('location'),
      values.location
    );

    mutate(formData);
  };

  return (
    <>
      <Container size="xl">
        <Text fw={700}>Details</Text>
        <Text size="sm">Created at: {storeCreatedAt?.toDateString()}</Text>
        <Space h="lg" />

        <form onSubmit={form.onSubmit(onFormSubmit)}>
          <TextInput
            label="Store name"
            description="Name of your store"
            placeholder="store name"
            // defaultValue={store?.name}
            key={form.key(nameof<UpdateStoreDetailsFormValues>('name'))}
            {...form.getInputProps(
              nameof<UpdateStoreDetailsFormValues>('name')
            )}
          />
          <Space h="md" />
          <TextInput
            label="Store location"
            description="Location of your store"
            placeholder="store location"
            key={form.key(nameof<UpdateStoreDetailsFormValues>('location'))}
            {...form.getInputProps(
              nameof<UpdateStoreDetailsFormValues>('location')
            )}
          />

          <Group justify="right" mt="md">
            <Button type="submit" disabled={false}>
              Update
            </Button>
          </Group>
        </form>
      </Container>
    </>
  );
}
