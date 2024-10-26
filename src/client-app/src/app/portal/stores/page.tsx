'use client';

import React from 'react';

import { Button, Container, Group, Space, Table } from '@mantine/core';
import { IconPlus } from '@tabler/icons-react';

import StoresTable from './storesTable';

export default function Page() {
  return (
    <Container size="lg">
      <Group justify="right">
        <Button leftSection={<IconPlus size={14} />} variant="subtle">
          Add New Store
        </Button>
      </Group>
      <Space h="md" />
      <StoresTable />
    </Container>
  );
}
