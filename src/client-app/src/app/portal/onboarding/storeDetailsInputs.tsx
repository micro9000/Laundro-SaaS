import { TextInput } from '@mantine/core';
import { UseFormReturnType } from '@mantine/form';

import { nameof } from '@/utilities';

import { OnboardingFormValues } from './onboardingFormValues';

export default function StoreDetailsInputs({
  form,
}: {
  form: UseFormReturnType<OnboardingFormValues>;
}) {
  return (
    <>
      <TextInput
        data-autofocus
        withAsterisk
        label="Store Name"
        key={form.key(nameof<OnboardingFormValues>('storeName'))}
        {...form.getInputProps(nameof<OnboardingFormValues>('storeName'))}
      />

      <TextInput
        data-autofocus
        withAsterisk
        label="Store Location"
        key={form.key(nameof<OnboardingFormValues>('storeLocation'))}
        {...form.getInputProps(nameof<OnboardingFormValues>('storeLocation'))}
      />
    </>
  );
}
