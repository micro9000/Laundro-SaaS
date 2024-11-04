'use client';

import React from 'react';

import { AuthenticationResult, EventType } from '@azure/msal-browser';
import { MsalProvider } from '@azure/msal-react';
import { MantineProvider, createTheme } from '@mantine/core';
import { ModalsProvider } from '@mantine/modals';
import { Notifications } from '@mantine/notifications';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Provider as ReduxProvider } from 'react-redux';

import { store } from '@/state/store';

import { msalInstance } from '../infrastructure/auth/authConfig';
import { AuthorizationProvider } from '../infrastructure/auth/authorizationProvider';

const theme = createTheme({
  fontFamily: 'Open Sans, sans-serif',
  // primaryColor: 'cyan',
});

const queryClient = new QueryClient();

export default function Providers({ children }: { children: React.ReactNode }) {
  msalInstance.initialize();
  const activeAccount = msalInstance.getActiveAccount();

  if (!activeAccount) {
    // Account selection
    const accounts = msalInstance.getAllAccounts();
    if (accounts.length > 0) {
      msalInstance.setActiveAccount(accounts[0]);
    }
  }
  //set the account
  msalInstance.addEventCallback(async (event) => {
    if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
      const payload = event.payload as AuthenticationResult;
      const account = payload.account;
      msalInstance.setActiveAccount(account);
    }
  });
  //enable account storage event
  msalInstance.enableAccountStorageEvents();

  return (
    <MsalProvider instance={msalInstance}>
      <AuthorizationProvider instance={msalInstance}>
        <ReduxProvider store={store}>
          <QueryClientProvider client={queryClient}>
            <MantineProvider defaultColorScheme="dark" theme={theme}>
              <ModalsProvider>
                <Notifications />
                {children}
              </ModalsProvider>
            </MantineProvider>
          </QueryClientProvider>
        </ReduxProvider>
      </AuthorizationProvider>
    </MsalProvider>
  );
}
