import { Space, TextInput } from '@mantine/core';
import { UseFormReturnType } from '@mantine/form';

import { nameof } from '@/utilities';

import { CreateNewStoreFileDropzone } from './createNewStoreFileDropzone';
import { CreateNewStoreFormValues } from './createNewStoreFormValues';

export default function CreateNewStoreForm({
  form,
}: {
  form: UseFormReturnType<CreateNewStoreFormValues>;
}) {
  return (
    <>
      <TextInput
        data-autofocus
        withAsterisk
        label="Store Name"
        key={form.key(nameof<CreateNewStoreFormValues>('storeName'))}
        {...form.getInputProps(nameof<CreateNewStoreFormValues>('storeName'))}
      />

      <TextInput
        data-autofocus
        withAsterisk
        label="Store Location"
        key={form.key(nameof<CreateNewStoreFormValues>('storeLocation'))}
        {...form.getInputProps(
          nameof<CreateNewStoreFormValues>('storeLocation')
        )}
      />
      <Space h="lg" />
      <CreateNewStoreFileDropzone />
    </>
  );
}
