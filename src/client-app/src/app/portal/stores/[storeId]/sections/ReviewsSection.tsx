import { Container, Text } from '@mantine/core';

import { Store } from '@/models';

export default function ReviewsSection({ store }: { store?: Store | null }) {
  return (
    <>
      <Container size="xl">
        <Text fw={700}>Reviews</Text>
      </Container>
    </>
  );
}
