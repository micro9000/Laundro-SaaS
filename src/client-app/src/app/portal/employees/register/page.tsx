'use client';

import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

import {
  Button,
  Container,
  Group,
  LoadingOverlay,
  Paper,
  TextInput,
} from '@mantine/core';
import { useForm } from '@mantine/form';
import { AxiosError } from 'axios';

import { EmployeeEndpoints } from '@/constants/apiEndpoints';
import { AppGeneralError } from '@/infrastructure/exceptions';
import { useAppMutation, useAppNotification } from '@/infrastructure/hooks';
import { User } from '@/models';
import { nameof } from '@/utilities';

import { RegisterNewEmployeeFormValues } from './registerNewEmployeeFormValues';

export default function Page() {
  // const notification = useAppNotification();
  const router = useRouter();

  const { mutate, isPending } = useAppMutation<{
    employee: User;
  }>({
    path: EmployeeEndpoints.register,
    mutationKey: 'register-new-employee',
    enableNotification: true,
    failedCallback: () => {},
    failedMessage: 'Unable to save employee details',
    successCallback: () => {
      setTimeout(() => {
        router.push('/portal/employees');
      }, 500);
    },
    successMessage: 'Successfully register new employee',
  });

  // useEffect(() => {
  //   if (isError && error && error instanceof AxiosError) {
  //     var generalError = (error as AxiosError).response
  //       ?.data as AppGeneralError;

  //     notification.notifyError(
  //       'Unable to save employee details',
  //       generalError.errors?.generalErrors?.join(',')
  //     );
  //   }
  // }, [isError, error, notification]);

  // useEffect(() => {
  //   if (isSuccess) {
  //     notification.notifySuccess('Successfully register new employee');

  //     setTimeout(() => {
  //       router.push('/portal/employees');
  //     }, 500);
  //   }
  // }, [isSuccess, notification, router]);

  const form = useForm<RegisterNewEmployeeFormValues>({
    mode: 'uncontrolled',
    initialValues: {
      name: '',
      email: '',
    },
    validate: {
      name: (value) => (value ? null : 'Store Name is required'),
      email: (value) => (/^\S+@\S+$/.test(value) ? null : 'Invalid email'),
    },
  });

  const onFormSubmit = (values: RegisterNewEmployeeFormValues) => {
    let formData = new FormData();

    formData.append(nameof<RegisterNewEmployeeFormValues>('name'), values.name);
    formData.append(
      nameof<RegisterNewEmployeeFormValues>('email'),
      values.email
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
            <Group grow>
              <TextInput
                data-autofocus
                withAsterisk
                label="Employee name"
                key={form.key(nameof<RegisterNewEmployeeFormValues>('name'))}
                {...form.getInputProps(
                  nameof<RegisterNewEmployeeFormValues>('name')
                )}
              />

              <TextInput
                data-autofocus
                withAsterisk
                label="Employee email"
                key={form.key(nameof<RegisterNewEmployeeFormValues>('email'))}
                {...form.getInputProps(
                  nameof<RegisterNewEmployeeFormValues>('email')
                )}
              />
            </Group>

            <Group justify="right" mt="md">
              <Button
                type="submit"
                variant="subtle"
                component={Link}
                href="/portal/employees"
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
