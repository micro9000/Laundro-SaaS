'use client';

import { AuthenticatedTemplate, UnauthenticatedTemplate } from '@azure/msal-react';
import { AppShell, Avatar, Burger, Button, Group, Skeleton, UnstyledButton } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { MantineLogo } from '@mantinex/mantine-logo';
import classes from './ApplicationShell.module.css';
import {IconLogin2} from '@tabler/icons-react'
import { handleLogin } from '@/app/infrastructure/auth/msal';
import UserAvatar from '../UserAvatar/UserAvatar';

export function ApplicationShell({ children }: { children: React.ReactNode }): React.ReactElement {
  const [opened, { toggle }] = useDisclosure();

  return (
    <AppShell
      header={{ height: 60 }}
      navbar={{ width: 300, breakpoint: 'sm', collapsed: { mobile: !opened } }}
      padding="md"
    >
      <AppShell.Header>
				<Group h="100%" px="md">
          <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" />
          <Group justify="space-between" style={{ flex: 1 }}>
            <MantineLogo size={30} />
            <Group ml="xl" gap={0} visibleFrom="sm">
							<UserAvatar />
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
      <AppShell.Main>
				<AuthenticatedTemplate>
					{children}
				</AuthenticatedTemplate>
				<UnauthenticatedTemplate>
					{/* <UnauthorizedMessage /> */}
					<h1>Unauthorized</h1>
				</UnauthenticatedTemplate>
			</AppShell.Main>
    </AppShell>
  );
}
