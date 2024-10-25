'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import React, { useEffect } from 'react';

import {
  AppShell,
  Badge,
  Burger,
  Group,
  NavLink,
  ScrollArea,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { MantineLogo } from '@mantinex/mantine-logo';
import {
  IconBuildingStore,
  IconChevronRight,
  IconHome2,
  IconUserEdit,
} from '@tabler/icons-react';

import { AuthButton } from '@/app/components/authenticationButtons';
import { selectCurrentSelectedStore } from '@/features/userContext/userContextSlice';
import { useAppSelector } from '@/state/hooks';

import { StoreSwitch } from './storeSwitch';
import { TenantIndicator } from './tenantIndicator';
import { ThemeToggle } from './themeToggle';

interface PortalShellProps {
  children: React.ReactNode;
}

export function PortalShell({
  children,
}: PortalShellProps): React.ReactElement {
  const pathname = usePathname();
  const [mobileOpened, { toggle: toggleMobile }] = useDisclosure();
  const [desktopOpened, { toggle: toggleDesktop }] = useDisclosure(true);
  const currentStore = useAppSelector(selectCurrentSelectedStore);

  useEffect(() => {
    toggleMobile();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [pathname, currentStore, currentStore?.id]);

  return (
    <AppShell
      header={{ height: 60 }}
      navbar={{
        width: 300,
        breakpoint: 'sm',
        collapsed: { mobile: !mobileOpened, desktop: !desktopOpened },
      }}
      padding="md"
    >
      <AppShell.Header>
        <Group h="100%" px="md">
          <Burger
            opened={mobileOpened}
            onClick={toggleMobile}
            hiddenFrom="sm"
            size="sm"
          />
          <Burger
            opened={desktopOpened}
            onClick={toggleDesktop}
            visibleFrom="sm"
            size="sm"
          />
          <Group justify="space-between" style={{ flex: 1 }}>
            <MantineLogo size={30} />
            <Group ml="xl" gap="md" visibleFrom="sm">
              <TenantIndicator />
              <ThemeToggle />
              <AuthButton />
            </Group>
          </Group>
        </Group>
      </AppShell.Header>
      <AppShell.Navbar p="md">
        <AppShell.Section grow my="md" component={ScrollArea}>
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
        </AppShell.Section>

        <AppShell.Section>
          <StoreSwitch />
        </AppShell.Section>
      </AppShell.Navbar>
      <AppShell.Main>{children}</AppShell.Main>
    </AppShell>
  );
}
