'use client';

import React from 'react';

import { AppShell, Burger, Group, UnstyledButton } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { MantineLogo } from '@mantinex/mantine-logo';

import { AuthButton } from '@/app/components/authenticationButtons';

import classes from './portalShell.module.css';
import { ThemeToggle } from './themeToggle';

interface PortalShellProps {
  children: React.ReactNode;
}

export function PortalShell({
  children,
}: PortalShellProps): React.ReactElement {
  const [opened, { toggle }] = useDisclosure();

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
              <ThemeToggle />
              <AuthButton />
            </Group>
          </Group>
        </Group>
      </AppShell.Header>
      <AppShell.Navbar p="md">
        <UnstyledButton className={classes.control}>Home</UnstyledButton>
        <UnstyledButton className={classes.control}>Blog</UnstyledButton>
        <UnstyledButton className={classes.control}>Contacts</UnstyledButton>
        <UnstyledButton className={classes.control}>Support</UnstyledButton>
      </AppShell.Navbar>
      <AppShell.Main>{children}</AppShell.Main>
    </AppShell>
  );
}
