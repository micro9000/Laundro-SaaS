import { Carousel } from '@mantine/carousel';
import { Image } from '@mantine/core';

import { GenerateStoreImageUrl } from '@/constants/apiEndpoints';
import { selectUserTenantGuid } from '@/features/userContext/userContextSlice';
import { StoreImage } from '@/models';
import { useAppSelector } from '@/state/hooks';

interface StoreImagesCarouselProps {
  storeImages?: StoreImage[];
  height?: number;
  width?: string;
  fit?: React.CSSProperties['objectFit'];
}

export default function StoreImageCarousel({
  storeImages,
  height = 200,
  width = 'auto',
  fit = 'contain',
}: StoreImagesCarouselProps) {
  const tenantGuid = useAppSelector(selectUserTenantGuid);

  if (storeImages && storeImages.length === 0) {
    return (
      <Image
        src={null}
        height={height}
        width={width}
        fit={fit}
        alt="Norway"
        fallbackSrc="https://placehold.co/600x400?text=Placeholder"
      />
    );
  }

  if (storeImages && storeImages.length === 1) {
    let image = storeImages.at(0);
    return (
      <Image
        src={`${GenerateStoreImageUrl(image?.storeId, image?.id, tenantGuid)}`}
        height={height}
        width={width}
        fit={fit}
        alt={image?.filename}
        fallbackSrc="https://placehold.co/600x400?text=Placeholder"
      />
    );
  }

  return (
    <>
      <Carousel withIndicators height={height}>
        {storeImages?.map((image) => (
          <Carousel.Slide key={image.id}>
            <Image
              src={`${GenerateStoreImageUrl(image?.storeId, image?.id, tenantGuid)}`}
              height={height}
              width={width}
              fit={fit}
              alt={image?.filename}
              fallbackSrc="https://placehold.co/600x400?text=Placeholder"
            />
          </Carousel.Slide>
        ))}
      </Carousel>
    </>
  );
}
