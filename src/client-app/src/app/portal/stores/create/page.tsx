'use client';

import Link from 'next/link';

import { Button, Container, Group, Paper } from '@mantine/core';
import { useForm } from '@mantine/form';

import { useAppMutation } from '@/infrastructure/hooks';
import { Store } from '@/models';

import CreateNewStoreForm from './createNewStoreForm';
import { CreateNewStoreFormValues } from './createNewStoreFormValues';

export default function Page() {
  const { mutate, isError, isSuccess, error, isPending } = useAppMutation<{
    store: Store;
  }>({
    path: '/store/create',
    mutationKey: 'create-new-store',
  });

  const form = useForm<CreateNewStoreFormValues>({
    mode: 'uncontrolled',
    initialValues: {
      storeName: '',
      storeLocation: '',
    },
    validate: {
      storeName: (value) => (value ? null : 'Store Name is required'),
      storeLocation: (value) => (value ? null : 'Store Location is required'),
    },
  });

  const onFormSubmit = (values: CreateNewStoreFormValues) => {
    var formData = new FormData();

    mutate(formData);
  };

  return (
    <Container size="lg">
      <Paper shadow="xl" withBorder p="xl">
        <form onSubmit={form.onSubmit(onFormSubmit)}>
          <CreateNewStoreForm form={form} />

          <Group justify="right" mt="md">
            <Button
              type="submit"
              variant="subtle"
              component={Link}
              href="/portal/stores"
            >
              Cancel
            </Button>
            <Button type="submit" disabled={isPending}>
              Submit
            </Button>
          </Group>
        </form>
      </Paper>
    </Container>
  );
}
