import Link from 'next/link';

import { Button, Card, Group, Rating, Space, Text } from '@mantine/core';
import { IconSettingsFilled } from '@tabler/icons-react';

import { Store } from '@/models';

import StoreImageCarousel from './storeImagesCarousel';

interface StoreCardPromps {
  store: Store;
}

// TODO: use NextJS Image component to enable image caching: https://nextjs.org/docs/pages/api-reference/components/image#loader

export default function StoreCard({ store }: StoreCardPromps) {
  return (
    <>
      <Card shadow="sm" padding="lg" radius="md" withBorder>
        <Card.Section>
          <StoreImageCarousel storeImages={store.images} />
        </Card.Section>

        <Group justify="space-between" mt="md" mb="xs">
          <Text fw={500}>{store.name}</Text>
        </Group>

        <Text size="lg" c="dimmed">
          {store.location}
        </Text>
        <Space h="md" />
        <Rating value={4.5} />

        <Button
          color="blue"
          fullWidth
          mt="md"
          radius="md"
          leftSection={<IconSettingsFilled size={14} />}
          component={Link}
          href={{
            pathname: `/portal/stores/${store.obfuscatedId}`,
          }}
        >
          Manage
        </Button>
      </Card>
    </>
  );
}
