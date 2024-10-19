'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import React from 'react';

import { AppShell, Badge, Burger, Group, NavLink } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { MantineLogo } from '@mantinex/mantine-logo';
import {
  IconBuildingStore,
  IconChevronRight,
  IconHome2,
  IconUserEdit,
} from '@tabler/icons-react';

import { AuthButton } from '@/app/components/authenticationButtons';
import { selectUserTenantName } from '@/features/userContext/userContextSlice';
import { useAppSelector } from '@/state/hooks';

import { ThemeToggle } from './themeToggle';

interface PortalShellProps {
  children: React.ReactNode;
}

export function PortalShell({
  children,
}: PortalShellProps): React.ReactElement {
  const pathname = usePathname();
  const [opened, { toggle }] = useDisclosure();
  const tenantName = useAppSelector(selectUserTenantName);

  return (
    <AppShell
      header={{ height: 60 }}
      navbar={{
        width: 300,
        breakpoint: 'sm',
        collapsed: { mobile: !opened },
      }}
      padding="md"
    >
      <AppShell.Header>
        <Group h="100%" px="md">
          <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" />
          <Group justify="space-between" style={{ flex: 1 }}>
            <MantineLogo size={30} />
            <Group ml="xl" gap={0} visibleFrom="sm">
              <Badge color="cyan">Tenant: {tenantName}</Badge>
              <ThemeToggle />
              <AuthButton />
            </Group>
          </Group>
        </Group>
      </AppShell.Header>
      <AppShell.Navbar p="md">
        <NavLink
          href="/portal"
          component={Link}
          label="Home"
          leftSection={<IconHome2 size="1rem" stroke={1.5} />}
          rightSection={
            <IconChevronRight
              size="0.8rem"
              stroke={1.5}
              className="mantine-rotate-rtl"
            />
          }
          variant="filled"
          active={pathname === '/portal'}
        />
        <NavLink
          href="/portal/stores"
          component={Link}
          label="Stores"
          leftSection={<IconBuildingStore size="1rem" stroke={1.5} />}
          rightSection={
            <IconChevronRight
              size="0.8rem"
              stroke={1.5}
              className="mantine-rotate-rtl"
            />
          }
          variant="filled"
          active={pathname === '/portal/stores'}
        />
        <NavLink
          href="/portal/users"
          component={Link}
          label="Users"
          leftSection={<IconUserEdit size="1rem" stroke={1.5} />}
          rightSection={
            <IconChevronRight
              size="0.8rem"
              stroke={1.5}
              className="mantine-rotate-rtl"
            />
          }
          variant="filled"
          active={pathname === '/portal/users'}
        />
      </AppShell.Navbar>
      <AppShell.Main>{children}</AppShell.Main>
    </AppShell>
  );
}
