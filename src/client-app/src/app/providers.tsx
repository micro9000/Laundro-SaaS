'use client';

import { useEffect } from 'react';

import { AuthenticationResult, EventType } from '@azure/msal-browser';
import { MsalProvider } from '@azure/msal-react';
import { MantineProvider, createTheme } from '@mantine/core';
import { Notifications } from '@mantine/notifications';
import { Provider as ReduxProvider } from 'react-redux';

import { store } from '@/state/store';

import { msalInstance } from '../infrastructure/auth/authConfig';
import { AuthorizationProvider } from '../infrastructure/auth/authorizationProvider';
import UserContextProvider from './userContextProvider';

const theme = createTheme({
  fontFamily: 'Open Sans, sans-serif',
  // primaryColor: 'cyan',
});

export default function Providers({ children }: { children: React.ReactNode }) {
  msalInstance.initialize().then(() => {
    const accounts = msalInstance.getAllAccounts();
    if (accounts.length > 0) {
      msalInstance.setActiveAccount(accounts[0]);
    }

    msalInstance.addEventCallback(async (event) => {
      if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
        const payload = event.payload as AuthenticationResult;
        const account = payload.account;
        msalInstance.setActiveAccount(account);
      }
    });
  });

  return (
    <MsalProvider instance={msalInstance}>
      <AuthorizationProvider instance={msalInstance}>
        <ReduxProvider store={store}>
          <UserContextProvider>
            <MantineProvider defaultColorScheme="dark" theme={theme}>
              <Notifications />
              {children}
            </MantineProvider>
          </UserContextProvider>
        </ReduxProvider>
      </AuthorizationProvider>
    </MsalProvider>
  );
}
