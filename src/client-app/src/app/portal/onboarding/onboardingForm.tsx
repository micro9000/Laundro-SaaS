import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

import { Button, Checkbox, Group, TextInput } from '@mantine/core';
import { useForm } from '@mantine/form';

import { useAppMutation, useAppNotification } from '@/infrastructure/hooks';
import { Tenant } from '@/models/tenant';

interface OnboardingFormValues {
  tenantName: string;
}

export default function OnboardingForm() {
  var router = useRouter();
  var notification = useAppNotification();
  const { mutate, isError, isSuccess, error, isPending, data } =
    useAppMutation<{
      tenant: Tenant;
    }>({
      path: '/tenant/create',
      mutationKey: 'tenant/create',
    });

  useEffect(() => {
    if (isError) {
      notification.notifyError(error.name, error.message);
    }
  }, [isError, error, notification]);

  useEffect(() => {
    if (isSuccess) {
      notification.notifySuccess('Successfully submit onboarding form');

      setTimeout(() => {
        router.push(`/portal/${data.tenant.tenantGuid}`);
      }, 500);
    }
  }, [isSuccess, notification]);

  const form = useForm<OnboardingFormValues>({
    mode: 'uncontrolled',
    initialValues: {
      tenantName: '',
    },

    validate: {
      tenantName: (value) => (value ? null : 'Tenant name is required'),
    },
  });

  const onFormSubmit = (values: OnboardingFormValues) => {
    var formData = new FormData();
    formData.append('name', values.tenantName);
    mutate(formData);
  };

  return (
    <form onSubmit={form.onSubmit(onFormSubmit)}>
      <TextInput
        data-autofocus
        withAsterisk
        label="Tenant Name"
        key={form.key('tenantName')}
        {...form.getInputProps('tenantName')}
      />

      <Group justify="flex-end" mt="md">
        <Button type="submit" disabled={isPending}>
          Submit
        </Button>
      </Group>
    </form>
  );
}
