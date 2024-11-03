import { Container, Text } from '@mantine/core';

import { Store } from '@/models';

export default function GallerySection({ store }: { store?: Store | null }) {
  return (
    <>
      <Container size="xl">
        <Text fw={700}>Gallery</Text>
      </Container>
    </>
  );
}
