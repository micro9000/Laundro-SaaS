import { useEffect, useState } from 'react';

import { Box, CloseButton, Image, SimpleGrid } from '@mantine/core';

interface ImagePreviewsProps {
  images: File[] | null;
  removeImage: (file: File) => void;
}

export default function ImagePreviews({
  images,
  removeImage,
}: ImagePreviewsProps) {
  const [previews, setPreviews] = useState<JSX.Element[] | undefined>();

  useEffect(() => {
    const previews = images?.map((file, index) => {
      const imageUrl = URL.createObjectURL(file);
      return (
        <Box style={{ position: 'relative', display: 'inline-block' }}>
          <Image
            key={index}
            src={imageUrl}
            style={{ width: '100%', height: 'auto', display: 'block' }}
            onLoad={() => URL.revokeObjectURL(imageUrl)}
          />
          <CloseButton
            onClick={() => removeImage(file)}
            style={{
              position: 'absolute',
              top: '8px',
              right: '8px',
              backgroundColor: 'rgba(0, 0, 0, 0.6)',
              color: 'white',
              border: 'none',
              borderRadius: '50%',
              width: '24px',
              height: '24px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              cursor: 'pointer',
            }}
          />
        </Box>
      );
    });

    setPreviews(previews);
  }, [images]);

  return (
    <SimpleGrid
      cols={{ base: 1, sm: 4 }}
      mt={previews && previews.length > 0 ? 'xl' : 0}
    >
      {previews}
    </SimpleGrid>
  );
}
