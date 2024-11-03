'use client';

import { use, useEffect, useState } from 'react';

import {
  Card,
  Container,
  LoadingOverlay,
  Paper,
  Space,
  Tabs,
  Text,
  rem,
} from '@mantine/core';
import {
  IconLogs,
  IconPhoto,
  IconSettings,
  IconStars,
  IconUsersGroup,
} from '@tabler/icons-react';

import { StoreEndpoints } from '@/constants/apiEndpoints';
import { useAppQuery } from '@/infrastructure/hooks';
import { Store } from '@/models';

import StoreImageCarousel from '../_components/storeImagesCarousel';
import ReviewsSection from './sections/ReviewsSection';
import EmployeesSection from './sections/employeesSection';
import GallerySection from './sections/gallerySection';
import StoreDetailsSection from './sections/storeDetailsSection';

export default function Page({
  params,
}: {
  params: Promise<{ storeId: string }>;
}) {
  const [store, setStore] = useState<Store | null>(null);
  const { storeId: obfuscatedId } = use(params);

  const { data, isLoading, isError, error } = useAppQuery<{ store: Store }>({
    path: StoreEndpoints.get,
    params: {
      ObfuscatedStoreId: obfuscatedId,
    },
    queryOptions: {
      queryKey: ['get-stores', obfuscatedId],
      enabled: obfuscatedId !== null,
    },
  });

  useEffect(() => {
    if (!isLoading && data?.store) {
      setStore(data.store);
    }
  }, [data, isLoading]);

  return (
    <>
      <LoadingOverlay
        visible={isLoading}
        zIndex={1000}
        overlayProps={{ radius: 'sm', blur: 2 }}
      />
      <Container size="xl">
        <Card shadow="sm" padding="xl">
          <Card.Section>
            <StoreImageCarousel
              storeImages={store?.images}
              height={250}
              fit="cover"
              width={undefined}
            />
          </Card.Section>

          <Text fw={500} size="lg" mt="md">
            {store?.name}
          </Text>

          <Text mt="xs" c="dimmed" size="sm">
            {store?.location}
          </Text>
        </Card>
        <Space h="lg" />
        <Paper shadow="xs" p="sm" withBorder>
          <Text fw={700}>Store settings</Text>
          <Space h="lg" />
          <Tabs
            defaultValue="details"
            orientation="vertical"
            variant="pills"
            color="cyan"
          >
            <Tabs.List>
              <Tabs.Tab
                value="details"
                leftSection={
                  <IconLogs style={{ width: rem(16), height: rem(16) }} />
                }
              >
                Details
              </Tabs.Tab>
              <Tabs.Tab
                value="employees"
                leftSection={
                  <IconUsersGroup style={{ width: rem(16), height: rem(16) }} />
                }
              >
                Employees
              </Tabs.Tab>
              <Tabs.Tab
                value="gallery"
                leftSection={
                  <IconPhoto style={{ width: rem(16), height: rem(16) }} />
                }
              >
                Gallery
              </Tabs.Tab>
              <Tabs.Tab
                value="reviews"
                leftSection={
                  <IconStars style={{ width: rem(16), height: rem(16) }} />
                }
              >
                Reviews
              </Tabs.Tab>
            </Tabs.List>
            <Tabs.Panel value="details">
              <StoreDetailsSection store={store} />
            </Tabs.Panel>
            <Tabs.Panel value="employees">
              <EmployeesSection store={store} />
            </Tabs.Panel>

            <Tabs.Panel value="gallery">
              <GallerySection store={store} />
            </Tabs.Panel>
            <Tabs.Panel value="reviews">
              <ReviewsSection store={store} />
            </Tabs.Panel>
          </Tabs>
        </Paper>
      </Container>
    </>
  );
}
