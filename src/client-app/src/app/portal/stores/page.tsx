'use client';

import Link from 'next/link';
import React from 'react';

import { Button, Container, Group, Space } from '@mantine/core';
import { IconPlus } from '@tabler/icons-react';

import StoresCards from './storesCards';

export default function Page() {
  return (
    <Container size="lg">
      <Group justify="right">
        <Button
          leftSection={<IconPlus size={14} />}
          variant="subtle"
          component={Link}
          href="/portal/stores/create"
        >
          Add New Store
        </Button>
      </Group>
      <Space h="md" />
      <StoresCards />
    </Container>
  );
}
