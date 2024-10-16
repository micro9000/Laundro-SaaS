'use client';

import { useEffect } from 'react';

import { MsalProvider } from '@azure/msal-react';
import { MantineProvider, createTheme } from '@mantine/core';
import { Provider as ReduxProvider } from 'react-redux';

import { store } from '@/state/store';

import { msalInstance } from '../infrastructure/auth/authConfig';
import { AuthorizationProvider } from '../infrastructure/auth/authorizationProvider';
import { initializeMsal } from '../infrastructure/auth/msal';
import UserContextProvider from './userContextProvider';

const theme = createTheme({
  fontFamily: 'Open Sans, sans-serif',
  // primaryColor: 'cyan',
});

export default function Providers({ children }: { children: React.ReactNode }) {
  useEffect(() => {
    initializeMsal();
  }, []);

  return (
    <MsalProvider instance={msalInstance}>
      <AuthorizationProvider instance={msalInstance}>
        <ReduxProvider store={store}>
          <UserContextProvider>
            <MantineProvider defaultColorScheme="dark" theme={theme}>
              {children}
            </MantineProvider>
          </UserContextProvider>
        </ReduxProvider>
      </AuthorizationProvider>
    </MsalProvider>
  );
}
