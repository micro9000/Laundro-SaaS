import Link from 'next/link';
import { useEffect, useState } from 'react';

import {
  ActionIcon,
  Button,
  Card,
  Group,
  Image,
  Menu,
  Rating,
  SimpleGrid,
  Space,
  Text,
  Title,
  rem,
} from '@mantine/core';
import {
  IconDots,
  IconEye,
  IconFileZip,
  IconSettingsFilled,
  IconStars,
  IconTrash,
} from '@tabler/icons-react';

import { GenerateStoreImageUrl } from '@/constants/apiEndpoints';
import { selectUserTenantGuid } from '@/features/userContext/userContextSlice';
import { Store } from '@/models';
import { useAppSelector } from '@/state/hooks';

import StoreImageCarousel from './storeImagesCarousel';

interface StoreCardPromps {
  store: Store;
}

// TODO: use NextJS Image component to enable image caching: https://nextjs.org/docs/pages/api-reference/components/image#loader

export default function StoreCard({ store }: StoreCardPromps) {
  const tenantGuid = useAppSelector(selectUserTenantGuid);

  const [images, setImages] = useState<string[]>();

  useEffect(() => {
    setImages(
      store?.images.map((img) =>
        GenerateStoreImageUrl(img?.storeId, img?.id, tenantGuid)
      )
    );
  }, [store, store?.images.length]);

  return (
    <>
      <Card withBorder shadow="sm" radius="md">
        <Card.Section withBorder inheritPadding py="xs">
          <Group justify="space-between">
            <Text fw={500}>{store.name}</Text>
            <Menu withinPortal position="bottom-end" shadow="sm">
              <Menu.Target>
                <ActionIcon variant="subtle" color="gray">
                  <IconDots style={{ width: rem(16), height: rem(16) }} />
                </ActionIcon>
              </Menu.Target>

              <Menu.Dropdown>
                <Menu.Item
                  leftSection={
                    <IconSettingsFilled
                      style={{ width: rem(14), height: rem(14) }}
                    />
                  }
                  component={Link}
                  href={{
                    pathname: `/portal/stores/${store.obfuscatedId}`,
                  }}
                >
                  Manage
                </Menu.Item>
                <Menu.Item
                  leftSection={
                    <IconStars style={{ width: rem(14), height: rem(14) }} />
                  }
                >
                  Reviews
                </Menu.Item>
                <Menu.Item
                  leftSection={
                    <IconTrash style={{ width: rem(14), height: rem(14) }} />
                  }
                  color="red"
                >
                  Delete all images
                </Menu.Item>
              </Menu.Dropdown>
            </Menu>
          </Group>
        </Card.Section>

        <Text mt="sm" c="dimmed" size="sm">
          Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
          eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad
          minim veniam, quis nostrud exercitation ullamco laboris nisi ut
        </Text>
        <Space h="md" />

        <Text mt="sm" c="dimmed" size="sm">
          {store.location}
        </Text>
        <Space h="md" />

        <Group>
          <Title order={6}>Ratings:</Title>
          <Rating value={4.5} />
        </Group>

        <Card.Section mt="sm">
          <Image
            src={images?.at(0)}
            radius="sm"
            height={200}
            fit="cover"
            width={undefined}
            fallbackSrc="https://placehold.co/600x400?text=Placeholder"
          />
        </Card.Section>

        {images && images?.length > 1 ? (
          <Card.Section inheritPadding mt="sm" pb="md">
            <SimpleGrid cols={3}>
              {images
                ?.slice(1)
                .map((image) => (
                  <Image
                    src={image}
                    key={image}
                    radius="sm"
                    width="auto"
                    fit="contain"
                  />
                ))}
            </SimpleGrid>
          </Card.Section>
        ) : null}
      </Card>
    </>
  );
}
