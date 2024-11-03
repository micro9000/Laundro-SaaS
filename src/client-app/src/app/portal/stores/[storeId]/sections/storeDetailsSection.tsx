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

import { Store } from '@/models';

export default function StoreDetailsSection({
  store,
}: {
  store?: Store | null;
}) {
  const [storeCreatedAt, setStoreCreatedAt] = useState<Date | null>(null);

  useEffect(() => {
    if (store?.createdAt) {
      setStoreCreatedAt(new Date(store!.createdAt!));
    }
  }, [store, store?.createdAt]);

  return (
    <>
      <Container size="xl">
        <Text fw={700}>Details</Text>
        <Text size="sm">Created at: {storeCreatedAt?.toDateString()}</Text>
        <Space h="lg" />
        <TextInput
          label="Store name"
          description="Name of your store"
          placeholder="store name"
          defaultValue={store?.name}
        />
        <Space h="md" />
        <TextInput
          label="Store location"
          // description="Location of your store"
          // placeholder="store location"
          defaultValue={store?.location}
        />

        <Group justify="right" mt="md">
          <Button type="submit" disabled={false}>
            Update
          </Button>
        </Group>
      </Container>
    </>
  );
}
