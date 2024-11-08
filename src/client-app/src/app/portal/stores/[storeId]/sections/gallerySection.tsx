import { useCallback, useEffect, useState } from 'react';

import {
  Button,
  Card,
  Container,
  Group,
  Image,
  SimpleGrid,
  Text,
} from '@mantine/core';
import { modals } from '@mantine/modals';
import { IconTrash } from '@tabler/icons-react';

import { GenerateStoreImageUrl } from '@/constants/apiEndpoints';
import { selectUserTenantGuid } from '@/features/userContext/userContextSlice';
import { Store, StoreImage } from '@/models';
import { useAppSelector } from '@/state/hooks';

export default function GallerySection({ store }: { store?: Store | null }) {
  const tenantGuid = useAppSelector(selectUserTenantGuid);

  const [images, setImages] = useState<StoreImage[]>();

  useEffect(() => {
    setImages(store?.images);
    //GenerateStoreImageUrl(img?.storeId, img?.id, tenantGuid)
  }, [store, store?.images.length]);

  const onConfirmDeleteImage = useCallback((imageId?: number) => {}, []);

  const openDeleteModal = useCallback(
    (imageId?: number) =>
      modals.openConfirmModal({
        title: 'Delete image',
        centered: true,
        children: (
          <Text size="sm">Are you sure you want to delete this image?</Text>
        ),
        labels: { confirm: 'Continue', cancel: 'Cancel' },
        confirmProps: { color: 'red' },
        // onCancel: () => console.log('Cancel', userId),
        onConfirm: () => onConfirmDeleteImage(imageId),
      }),
    [onConfirmDeleteImage]
  );

  return (
    <>
      <Container size="xl">
        <Text fw={700}>Gallery</Text>

        <SimpleGrid cols={4}>
          {images && images?.length > 1
            ? images.map((image) => (
                <>
                  <Card shadow="sm" padding="lg" radius="md" withBorder>
                    <Card.Section>
                      <Image
                        src={GenerateStoreImageUrl(
                          image?.storeId,
                          image?.id,
                          tenantGuid
                        )}
                        key={image?.id}
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
                        onClick={() => openDeleteModal(image?.id)}
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
              ))
            : null}
        </SimpleGrid>
      </Container>
    </>
  );
}
