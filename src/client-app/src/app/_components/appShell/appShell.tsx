'use client';

// import { useEffect } from 'react';
// import {
//   InteractionRequiredAuthError,
//   InteractionType,
// } from '@azure/msal-browser';
// import { useMsalAuthentication } from '@azure/msal-react';
import React from 'react';

import { AppShell, Burger, Group, UnstyledButton } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { MantineLogo } from '@mantinex/mantine-logo';

import { AuthButton } from '@/app/components/authenticationButtons';

// import { loginRequest } from '@/infrastructure/auth/auth-config';
// import { Config } from '@/infrastructure/config';
import classes from './appShell.module.css';
import { ThemeToggle } from './themeToggle';

interface ApplicationShellProps {
  children: React.ReactNode;
  isDesktopView?: boolean;
}

export function ApplicationShell({
  children,
  isDesktopView = true,
}: ApplicationShellProps): React.ReactElement {
  const [opened, { toggle }] = useDisclosure();

  // const { login, result, error } = useMsalAuthentication(
  //   InteractionType.Silent,
  //   loginRequest
  // );

  // useEffect(() => {
  //   if (error instanceof InteractionRequiredAuthError) {
  //     var interactionType =
  //       Config.SignInFlow === 'popup'
  //         ? InteractionType.Popup
  //         : InteractionType.Redirect;
  //     login(interactionType, loginRequest);
  //   }
  // }, [login, error]);

  return (
    <AppShell
      header={{ height: 60 }}
      navbar={{
        width: 300,
        breakpoint: 'sm',
        collapsed: { desktop: isDesktopView, mobile: !opened },
      }}
      padding="md"
    >
      <AppShell.Header>
        <Group h="100%" px="md">
          <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" />
          <Group justify="space-between" style={{ flex: 1 }}>
            <MantineLogo size={30} />
            <Group ml="xl" gap={0} visibleFrom="sm">
              <UnstyledButton className={classes.control}>Home</UnstyledButton>
              <UnstyledButton className={classes.control}>Blog</UnstyledButton>
              <UnstyledButton className={classes.control}>
                Contacts
              </UnstyledButton>
              <UnstyledButton className={classes.control}>
                Support
              </UnstyledButton>
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
