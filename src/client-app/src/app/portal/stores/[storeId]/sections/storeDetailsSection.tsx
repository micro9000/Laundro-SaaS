import { useEffect, useRef, useState } from 'react';

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

import { StoreEndpoints } from '@/constants/apiEndpoints';
import { useAppMutation } from '@/infrastructure/hooks';
import { Store } from '@/models';
import { nameof } from '@/utilities';

import { getStoreDetailsById } from '../sharedApiRequestKeys';

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
  const queryClient = useQueryClient();
  const queryClientRef = useRef(queryClient);

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

  const { mutate } = useAppMutation({
    path: StoreEndpoints.update,
    mutationKey: 'update-store-details',
    httpVerb: 'put',
    enableNotification: true,
    successCallback: () => {
      queryClientRef.current.invalidateQueries({
        queryKey: [getStoreDetailsById],
      });
    },
    successMessage: 'Store details has successful updated',
    failedCallback: () => {},
    failedMessage: 'Unable to update store',
  });

  // useEffect(() => {
  //   if (isSuccess && !isPending) {
  //     notificationRef.current.notifySuccess(
  //       'Store details has successful updated'
  //     );
  //     queryClientRef.current.invalidateQueries({
  //       queryKey: [getStoreDetailsById],
  //     });
  //   }
  // }, [isSuccess, isPending]);

  // useEffect(() => {
  //   if (isError && error && error instanceof AxiosError) {
  //     var generalError = (error as AxiosError).response
  //       ?.data as AppGeneralError;

  //     notificationRef.current.notifyError(
  //       'Unable to update store',
  //       generalError.errors?.generalErrors?.join(',')
  //     );
  //   }
  // }, [isError, error]);

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
