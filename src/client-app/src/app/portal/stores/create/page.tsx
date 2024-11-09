'use client';

import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useCallback, useEffect, useRef, useState } from 'react';

import {
  Button,
  Container,
  Group,
  Image,
  LoadingOverlay,
  Paper,
  SimpleGrid,
  Space,
  Text,
  TextInput,
} from '@mantine/core';
import { FileRejection } from '@mantine/dropzone';
import { useForm } from '@mantine/form';
import { isArray, isObject } from 'lodash';

import { StoreEndpoints } from '@/constants/apiEndpoints';
import { useAppMutation, useAppNotification } from '@/infrastructure/hooks';
import { Store } from '@/models';
import { ExtractErrorMessages, nameof } from '@/utilities';

import { FileDropzone } from '../../_components/fileDropzone/FileDropzone';
import ImagePreviews from '../_components/imagePreviews';
import { maximumStoreImages } from '../storeConfigs';
import { CreateNewStoreFormValues } from './createNewStoreFormValues';

export default function Page() {
  const router = useRouter();
  const routerRef = useRef(router);
  const notification = useAppNotification();
  const notificationRef = useRef(notification);
  const [storeImages, setStoreImages] = useState<File[] | null>(null);

  const { mutate, isPending } = useAppMutation<{
    store: Store;
  }>({
    path: StoreEndpoints.create,
    mutationKey: 'create-new-store',
    enableMultipartForm: true,
    enableNotification: true,
    successCallback: () => {
      setTimeout(() => {
        routerRef.current.push('/portal/stores');
      }, 500);
    },
    successMessage: 'Successfully created new store',
    failedCallback: () => {},
    failedMessage: 'Unable to save new store details',
  });

  // useEffect(() => {
  //   if (isError) {
  //     var errorsToDisplay = ExtractErrorMessages(error);

  //     if (isArray(errorsToDisplay)) {
  //       errorsToDisplay.forEach((err) => {
  //         notificationRef.current.notifyError(
  //           'Unable to save new store details',
  //           err
  //         );
  //       });
  //     }
  //   }
  // }, [isError, error]);

  // useEffect(() => {
  //   if (isSuccess) {
  //     notificationRef.current.notifySuccess('Successfully created new store');

  //     setTimeout(() => {
  //       routerRef.current.push('/portal/stores');
  //     }, 500);
  //   }
  // }, [isSuccess]);

  const uploadImages = (images: File[]) => {
    setStoreImages(images);
  };

  const onRejectFiles = useCallback((images: FileRejection[]) => {
    var invalidFiles = images.map((i) => i.file.name).join(', ');
    var errors = images.map((i) => i.errors).flat();
    notificationRef.current.notifyWarning(`Invalid files: [${invalidFiles}]`);
    errors.forEach((e) => {
      notificationRef.current.notifyError(e.code, e.message);
    });
  }, []);

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

  const onFormSubmit = useCallback(
    (values: CreateNewStoreFormValues) => {
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
    },
    [storeImages]
  );

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
            <FileDropzone
              dropzoneProps={{
                onDrop: (files) => uploadImages(files),
                onReject: onRejectFiles,
                maxFiles: maximumStoreImages,
              }}
              title="Drag images here or click to select files"
              description="Attach as at least 4 images, each file should not exceed 5mb"
            />
            <ImagePreviews
              images={storeImages}
              removeImage={(file: File) => {
                setStoreImages(storeImages?.filter((f) => f !== file) ?? null);
              }}
            />

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
