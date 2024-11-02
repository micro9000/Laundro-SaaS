'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import React, { useEffect, useState } from 'react';

import {
  InteractionRequiredAuthError,
  InteractionType,
} from '@azure/msal-browser';
import { useIsAuthenticated, useMsalAuthentication } from '@azure/msal-react';
import {
  AppShell,
  Badge,
  Burger,
  Group,
  LoadingOverlay,
  NavLink,
  ScrollArea,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
// import { MantineLogo } from '@mantinex/mantine-logo';
import {
  IconBuildingStore,
  IconChevronRight,
  IconHome2,
  IconUserEdit,
} from '@tabler/icons-react';

import { AuthButton } from '@/app/components/authenticationButtons';
import {
  populateUserContextThunkAsync,
  selectCurrentSelectedStore,
  selectUserContextStatus,
} from '@/features/userContext/userContextSlice';
import { loginRequest } from '@/infrastructure/auth/authConfig';
import { Config } from '@/infrastructure/config';
import { useAppNotification } from '@/infrastructure/hooks';
import { useAppDispatch, useAppSelector } from '@/state/hooks';

import PortalBreadcrumb from '../breadcrumb';
import ThemeToggle from '../themeToggle';
import { StoreSwitch } from './storeSwitch';
import { TenantIndicator } from './tenantIndicator';

interface PortalShellProps {
  children: React.ReactNode;
}

export function PortalShell({
  children,
}: PortalShellProps): React.ReactElement {
  const notification = useAppNotification();
  const dispatch = useAppDispatch();
  const pathname = usePathname();
  const [mobileOpened, { toggle: toggleMobile }] = useDisclosure();
  const [desktopOpened, { toggle: toggleDesktop }] = useDisclosure(true);
  const currentStore = useAppSelector(selectCurrentSelectedStore);
  const [isLoadingOverlayVisible, setIsLoadingOverlayVisible] =
    useState<boolean>(true);
  var userContextLoadingStatus = useAppSelector(selectUserContextStatus);
  const isAuthenticated = useIsAuthenticated();

  const { login, error } = useMsalAuthentication(
    InteractionType.Silent,
    loginRequest
  );

  useEffect(() => {
    if (error instanceof InteractionRequiredAuthError) {
      var interactionType =
        Config.SignInFlow === 'popup'
          ? InteractionType.Popup
          : InteractionType.Redirect;
      login(interactionType, loginRequest);
    }
  }, [login, error]);

  useEffect(() => {
    toggleMobile();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [pathname, currentStore, currentStore?.id]);

  useEffect(() => {
    if (isAuthenticated) {
      dispatch(populateUserContextThunkAsync());
    }
  }, [dispatch, isAuthenticated]);

  useEffect(() => {
    if (isAuthenticated && userContextLoadingStatus === 'loading') {
      setIsLoadingOverlayVisible(true);
    } else if (isAuthenticated && userContextLoadingStatus === 'failed') {
      notification.notifyError(
        'Failed to load user context',
        'Unable to load user context due to internal server error'
      );
    } else if (isAuthenticated && userContextLoadingStatus === 'idle') {
      setIsLoadingOverlayVisible(false);
    }
  }, [isAuthenticated, userContextLoadingStatus, notification]);

  return (
    <>
      {/* <LoadingOverlay
        visible={isLoadingOverlayVisible}
        zIndex={1000}
        overlayProps={{ radius: 'sm', blur: 2 }}
      /> */}
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
              {/* <MantineLogo size={30} /> */}
              <TenantIndicator />
              <Group ml="xl" gap="md" visibleFrom="sm">
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
              href="/portal/employees"
              component={Link}
              label="Employees"
              leftSection={<IconUserEdit size="1rem" stroke={1.5} />}
              rightSection={
                <IconChevronRight
                  size="0.8rem"
                  stroke={1.5}
                  className="mantine-rotate-rtl"
                />
              }
              variant="filled"
              active={pathname === '/portal/employees'}
            />
          </AppShell.Section>

          <AppShell.Section>
            <StoreSwitch />
          </AppShell.Section>
        </AppShell.Navbar>
        <AppShell.Main>
          <PortalBreadcrumb />
          {children}
        </AppShell.Main>
      </AppShell>
    </>
  );
}
