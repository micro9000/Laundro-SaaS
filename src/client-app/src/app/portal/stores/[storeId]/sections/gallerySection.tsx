import { useEffect, useState } from 'react';

import {
  Button,
  Card,
  Container,
  Group,
  Image,
  SimpleGrid,
  Text,
} from '@mantine/core';
import { IconTrash } from '@tabler/icons-react';

import { selectUserTenantGuid } from '@/features/userContext/userContextSlice';
import { Store } from '@/models';
import { useAppSelector } from '@/state/hooks';

export default function GallerySection({ store }: { store?: Store | null }) {
  const tenantGuid = useAppSelector(selectUserTenantGuid);

  const [images, setImages] = useState<string[]>();

  useEffect(() => {
    setImages(
      store?.images.map(
        (img) =>
          `https://localhost:7177/api/store/get-image-content/${tenantGuid}/${img?.storeId}/${img?.id}`
      )
    );
  }, [store, store?.images.length]);

  return (
    <>
      <Container size="xl">
        <Text fw={700}>Gallery</Text>

        {images && images?.length > 1 ? (
          <SimpleGrid cols={4}>
            {images.map((image) => (
              <>
                <Card shadow="sm" padding="lg" radius="md" withBorder>
                  <Card.Section>
                    <Image
                      src={image}
                      key={image}
                      radius="sm"
                      width="auto"
                      fit="contain"
                    />
                  </Card.Section>
                  <Group>
                    <Button
                      color="blue"
                      mt="md"
                      variant="subtle"
                      radius="md"
                      size="sm"
                      leftSection={
                        <IconTrash
                          size="0.8rem"
                          stroke={1.5}
                          className="mantine-rotate-rtl"
                        />
                      }
                    >
                      Delete
                    </Button>

                    <Button
                      color="blue"
                      mt="md"
                      variant="subtle"
                      radius="md"
                      size="sm"
                      leftSection={
                        <IconTrash
                          size="0.8rem"
                          stroke={1.5}
                          className="mantine-rotate-rtl"
                        />
                      }
                    >
                      Replace
                    </Button>
                  </Group>
                </Card>
              </>
            ))}
          </SimpleGrid>
        ) : null}
      </Container>
    </>
  );
}
