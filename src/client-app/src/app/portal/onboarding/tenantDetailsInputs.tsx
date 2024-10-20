import { Group, Space, TextInput, Textarea, rem } from '@mantine/core';
import { UseFormReturnType } from '@mantine/form';
import { IconAt } from '@tabler/icons-react';

import { nameof } from '@/utilities';

import { OnboardingFormValues } from './onboardingFormValues';

export default function TenantDetailsInputs({
  form,
}: {
  form: UseFormReturnType<OnboardingFormValues>;
}) {
  const icon = <IconAt style={{ width: rem(16), height: rem(16) }} />;

  return (
    <>
      <TextInput
        data-autofocus
        withAsterisk
        label="Tenant Name"
        description="Your company name"
        key={form.key(nameof<OnboardingFormValues>('tenantName'))}
        {...form.getInputProps(nameof<OnboardingFormValues>('tenantName'))}
      />
      <Space h="md" />

      <Textarea
        label="Company Address"
        key={form.key(nameof<OnboardingFormValues>('companyAddress'))}
        {...form.getInputProps(nameof<OnboardingFormValues>('companyAddress'))}
      />

      <Space h="md" />

      <TextInput
        data-autofocus
        label="Website Url"
        key={form.key(nameof<OnboardingFormValues>('websiteUrl'))}
        {...form.getInputProps(nameof<OnboardingFormValues>('websiteUrl'))}
      />
      <Space h="md" />

      <Group grow justify="flex-start">
        <TextInput
          data-autofocus
          label="Business Registration Number"
          key={form.key(
            nameof<OnboardingFormValues>('businessRegistrationNumber')
          )}
          {...form.getInputProps(
            nameof<OnboardingFormValues>('businessRegistrationNumber')
          )}
        />
        <TextInput
          data-autofocus
          withAsterisk
          label="Primary Contact Name"
          key={form.key(nameof<OnboardingFormValues>('primaryContactName'))}
          {...form.getInputProps(
            nameof<OnboardingFormValues>('primaryContactName')
          )}
        />
      </Group>

      <Space h="md" />

      <Group grow justify="flex-start">
        <TextInput
          data-autofocus
          withAsterisk
          label="Contact Email"
          rightSection={icon}
          key={form.key(nameof<OnboardingFormValues>('contactEmail'))}
          {...form.getInputProps(nameof<OnboardingFormValues>('contactEmail'))}
        />

        <TextInput
          data-autofocus
          withAsterisk
          label="Phone Number"
          key={form.key(nameof<OnboardingFormValues>('phoneNumber'))}
          {...form.getInputProps(nameof<OnboardingFormValues>('phoneNumber'))}
        />
      </Group>
    </>
  );
}
