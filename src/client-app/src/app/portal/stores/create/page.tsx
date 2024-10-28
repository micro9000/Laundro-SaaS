'use client';

import Link from 'next/link';
import { useEffect, useState } from 'react';

import {
  Button,
  Container,
  Group,
  LoadingOverlay,
  Paper,
  Space,
  Text,
  TextInput,
} from '@mantine/core';
import { FileRejection } from '@mantine/dropzone';
import { useForm } from '@mantine/form';

import { useAppMutation, useAppNotification } from '@/infrastructure/hooks';
import useAppMultipartMutation from '@/infrastructure/hooks/useAppMultipartMutation';
import { Store } from '@/models';
import { nameof } from '@/utilities';

import { CreateNewStoreFileDropzone } from './createNewStoreFileDropzone';
import { CreateNewStoreFormValues } from './createNewStoreFormValues';

export default function Page() {
  const notification = useAppNotification();
  const [storeImages, setStoreImages] = useState<File[] | null>(null);

  const { mutate, isError, isSuccess, error, isPending } =
    useAppMultipartMutation<{
      store: Store;
    }>({
      path: '/store/create',
      mutationKey: 'create-new-store',
    });

  useEffect(() => {
    console.log(isError, error);
  }, [isError, error]);

  useEffect(() => {
    if (isSuccess) {
      notification.notifySuccess('Successfully created new store');

      setTimeout(() => {
        location.reload();
      }, 500);
    }
  }, [isSuccess, notification]);

  const uploadImages = (images: File[]) => {
    setStoreImages(images);
  };

  const onRejectFiles = (images: FileRejection[]) => {
    var invalidFiles = images.map((i) => i.file.name).join(', ');
    var errors = images.map((i) => i.errors).flat();
    notification.notifyWarning(`Invalid files: [${invalidFiles}]`);
    errors.forEach((e) => {
      notification.notifyError(e.code, e.message);
    });
  };

  const form = useForm<CreateNewStoreFormValues>({
    mode: 'uncontrolled',
    initialValues: {
      name: '',
      location: '',
    },
    validate: {
      name: (value) => (value ? null : 'Store Name is required'),
      location: (value) => (value ? null : 'Store Location is required'),
    },
  });

  const onFormSubmit = (values: CreateNewStoreFormValues) => {
    var formData = new FormData();

    if (storeImages !== null && storeImages.length > 0) {
      storeImages.forEach((f) => formData.append('storeImages', f));
    }

    formData.append(nameof<CreateNewStoreFormValues>('name'), values.name);
    formData.append(
      nameof<CreateNewStoreFormValues>('location'),
      values.location
    );

    mutate(formData);
  };

  return (
    <>
      <LoadingOverlay
        visible={isPending}
        zIndex={1000}
        overlayProps={{ radius: 'sm', blur: 2 }}
      />
      <Container size="lg">
        <Paper shadow="xl" withBorder p="xl">
          <form onSubmit={form.onSubmit(onFormSubmit)}>
            <TextInput
              data-autofocus
              withAsterisk
              label="Store Name"
              key={form.key(nameof<CreateNewStoreFormValues>('name'))}
              {...form.getInputProps(nameof<CreateNewStoreFormValues>('name'))}
            />

            <TextInput
              data-autofocus
              withAsterisk
              label="Store Location"
              key={form.key(nameof<CreateNewStoreFormValues>('location'))}
              {...form.getInputProps(
                nameof<CreateNewStoreFormValues>('location')
              )}
            />
            <Space h="lg" />
            <CreateNewStoreFileDropzone
              onDrop={(files) => uploadImages(files)}
              onReject={onRejectFiles}
            />
            <Text>
              Selected files: {storeImages !== null ? storeImages.length : 0}
            </Text>

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
    </>
  );
}
