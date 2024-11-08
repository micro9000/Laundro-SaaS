import { useCallback, useEffect, useRef, useState } from 'react';

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
import { useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import {
  GenerateStoreImageUrl,
  StoreImageEndpoints,
} from '@/constants/apiEndpoints';
import { selectUserTenantGuid } from '@/features/userContext/userContextSlice';
import { AppGeneralError } from '@/infrastructure/exceptions';
import { useAppMutation, useAppNotification } from '@/infrastructure/hooks';
import { Store, StoreImage } from '@/models';
import { useAppSelector } from '@/state/hooks';

import { getStoreDetailsById } from '../sharedApiRequestKeys';

export default function GallerySection({ store }: { store?: Store | null }) {
  const tenantGuid = useAppSelector(selectUserTenantGuid);
  const notification = useAppNotification();
  const notificationRef = useRef(notification); // to resolve React Hook useEffect has a missing dependency:
  const queryClient = useQueryClient();
  const queryClientRef = useRef(queryClient);

  const [images, setImages] = useState<StoreImage[]>();

  useEffect(() => {
    setImages(store?.images);
  }, [store, store?.images.length]);

  const {
    mutate: deleteStoreImageMutate,
    isError: isDeleteStoreImageError,
    isSuccess: isDeleteStoreImageSuccess,
    error: deleteStoreImageError,
    isPending: isDeleteStoreImagePending,
  } = useAppMutation({
    path: StoreImageEndpoints.delete,
    mutationKey: 'delete-store-image',
    httpVerb: 'delete',
  });

  useEffect(() => {
    if (isDeleteStoreImageSuccess && !isDeleteStoreImagePending) {
      notificationRef?.current.notifySuccess('Successfully deleted an image');
      queryClientRef.current.invalidateQueries({
        queryKey: [getStoreDetailsById],
      });
    }
  }, [isDeleteStoreImageSuccess, isDeleteStoreImagePending]);

  useEffect(() => {
    if (
      isDeleteStoreImageError &&
      deleteStoreImageError &&
      deleteStoreImageError instanceof AxiosError
    ) {
      var generalError = (deleteStoreImageError as AxiosError).response
        ?.data as AppGeneralError;

      notificationRef.current.notifyError(
        'Unable to un-assign employee',
        generalError.errors?.generalErrors?.join(',')
      );
    }
  }, [isDeleteStoreImageError, deleteStoreImageError]);

  const onConfirmDeleteImage = useCallback(
    (imageId?: number) => {
      let formData = new FormData();

      formData.append('imageId', imageId?.toString() ?? '0');
      formData.append('storeId', store?.id?.toString() ?? '0');

      deleteStoreImageMutate(formData);
    },
    [store?.id, deleteStoreImageMutate]
  );

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
