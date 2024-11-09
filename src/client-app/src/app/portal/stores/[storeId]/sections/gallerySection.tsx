import { useCallback, useEffect, useRef, useState } from 'react';

import {
  Button,
  Card,
  Container,
  Drawer,
  Group,
  Image,
  SimpleGrid,
  Space,
  Text,
} from '@mantine/core';
import { FileRejection } from '@mantine/dropzone';
import { useForm } from '@mantine/form';
import { useDisclosure } from '@mantine/hooks';
import { modals } from '@mantine/modals';
import { IconPlus, IconTrash } from '@tabler/icons-react';
import { useQueryClient } from '@tanstack/react-query';

import { FileDropzone } from '@/app/portal/_components/fileDropzone/FileDropzone';
import {
  GenerateStoreImageUrl,
  StoreImageEndpoints,
} from '@/constants/apiEndpoints';
import { selectUserTenantGuid } from '@/features/userContext/userContextSlice';
import { useAppMutation, useAppNotification } from '@/infrastructure/hooks';
import { Store, StoreImage } from '@/models';
import { useAppSelector } from '@/state/hooks';

import { maximumStoreImages } from '../../storeConfigs';
import { getStoreDetailsById } from '../sharedApiRequestKeys';

export default function GallerySection({ store }: { store?: Store | null }) {
  const tenantGuid = useAppSelector(selectUserTenantGuid);
  const notification = useAppNotification();
  const notificationRef = useRef(notification); // to resolve React Hook useEffect has a missing dependency:
  const queryClient = useQueryClient();
  const queryClientRef = useRef(queryClient);
  const [storeImages, setStoreImages] = useState<File[] | null>(null);

  const [images, setImages] = useState<StoreImage[]>();
  const [opened, { open, close }] = useDisclosure(false);

  useEffect(() => {
    setImages(store?.images);
  }, [store, store?.images.length]);

  const { mutate: deleteStoreImageMutate } = useAppMutation({
    path: StoreImageEndpoints.delete,
    mutationKey: 'delete-store-image',
    httpVerb: 'delete',
    enableNotification: true,
    successMessage: 'Successfully deleted an image',
    successCallback: () => {
      queryClientRef.current.invalidateQueries({
        queryKey: [getStoreDetailsById],
      });
    },
    failedMessage: 'Unable to delete an image',
    failedCallback: () => {},
  });

  const { mutate: uploadImagesMutate } = useAppMutation({
    path: StoreImageEndpoints.upload,
    mutationKey: 'upload-store-image',
    enableNotification: true,
    successMessage: 'Successfully uploaded new image(s)',
    successCallback: () => {
      queryClientRef.current.invalidateQueries({
        queryKey: [getStoreDetailsById],
      });
    },
    failedMessage: 'Unable to upload an image',
    failedCallback: () => {},
  });

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

  const uploadImages = (images: File[]) => {
    setStoreImages(images);
  };

  const onRejectFiles = useCallback((images: FileRejection[]) => {
    var invalidFiles = images.map((i) => i.file.name).join(', ');
    var errors = images.map((i) => i.errors).flat();
    notificationRef.current.notifyWarning(`Invalid files: [${invalidFiles}]`);
    errors.forEach((e) => {
      notificationRef.current.notifyError(e.code, e.message);
    });
  }, []);

  const previews = storeImages?.map((file, index) => {
    const imageUrl = URL.createObjectURL(file);
    return (
      <Image
        key={index}
        src={imageUrl}
        onLoad={() => URL.revokeObjectURL(imageUrl)}
      />
    );
  });

  const form = useForm({
    mode: 'uncontrolled',
  });

  const onFormSubmit = useCallback(() => {
    var formData = new FormData();

    if (storeImages !== null && storeImages.length > 0) {
      storeImages.forEach((f) => formData.append('images', f));
    }
    formData.append('storeId', store?.id?.toString() ?? '0');

    uploadImagesMutate(formData);
  }, [storeImages, store?.id]);

  return (
    <>
      <Container size="xl">
        <Text fw={700}>Gallery</Text>

        <Drawer
          offset={4}
          // radius="md"
          size="lg"
          opened={opened}
          onClose={close}
          title="Uplaod New Store Image"
          position="right"
        >
          <form onSubmit={form.onSubmit(onFormSubmit)}>
            <FileDropzone
              dropzoneProps={{
                onDrop: (files) => uploadImages(files),
                onReject: onRejectFiles,
                maxFiles: maximumStoreImages,
              }}
              title="Drag images here or click to select files"
              description="Attach as at least 4 images, each file should not exceed 5mb"
            />
            <SimpleGrid
              cols={{ base: 1, sm: 4 }}
              mt={previews && previews.length > 0 ? 'xl' : 0}
            >
              {previews}
            </SimpleGrid>

            <Group justify="right" mt="md">
              <Button
                type="submit"
                disabled={storeImages === null || storeImages?.length === 0}
              >
                Submit
              </Button>
            </Group>
          </form>
        </Drawer>

        <Space h="lg" />
        <Group justify="right">
          <Button
            leftSection={<IconPlus size={14} />}
            variant="subtle"
            disabled={(images?.length ?? 0) >= maximumStoreImages}
            onClick={open}
          >
            Upload New Image
          </Button>
        </Group>
        <Space h="md" />

        <SimpleGrid cols={4}>
          {images && images?.length > 1
            ? images.map((image) => (
                <Card
                  key={image?.id}
                  shadow="sm"
                  padding="lg"
                  radius="md"
                  withBorder
                >
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
                </Card>
              ))
            : null}
        </SimpleGrid>
      </Container>
    </>
  );
}
