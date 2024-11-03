import { Carousel } from '@mantine/carousel';
import { Image } from '@mantine/core';

import { selectUserTenantGuid } from '@/features/userContext/userContextSlice';
import { StoreImage } from '@/models';
import { useAppSelector } from '@/state/hooks';

interface StoreImagesCarouselProps {
  storeImages: StoreImage[];
}

export default function StoreImageCarousel({
  storeImages,
}: StoreImagesCarouselProps) {
  const tenantGuid = useAppSelector(selectUserTenantGuid);

  if (storeImages.length === 0) {
    return (
      <Image
        src={null}
        height={200}
        width="auto"
        fit="contain"
        alt="Norway"
        fallbackSrc="https://placehold.co/600x400?text=Placeholder"
      />
    );
  }

  if (storeImages.length === 1) {
    let image = storeImages.at(0);
    return (
      <Image
        src={`https://localhost:7177/api/store/get-image-content/${tenantGuid}/${image?.storeId}/${image?.id}`}
        height={200}
        width="auto"
        fit="contain"
        alt="Norway"
        fallbackSrc="https://placehold.co/600x400?text=Placeholder"
      />
    );
  }

  return (
    <>
      <Carousel withIndicators height={200}>
        {storeImages.map((image) => (
          <Carousel.Slide key={image.id}>
            <Image
              src={`https://localhost:7177/api/store/get-image-content/${tenantGuid}/${image.storeId}/${image.id}`}
              height={200}
              width="auto"
              fit="contain"
              alt="Norway"
              fallbackSrc="https://placehold.co/600x400?text=Placeholder"
            />
          </Carousel.Slide>
        ))}
      </Carousel>
    </>
  );
}
