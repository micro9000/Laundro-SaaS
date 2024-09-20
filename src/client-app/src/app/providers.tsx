'use client';

import { useEffect } from 'react';

import { MsalProvider } from '@azure/msal-react';
import { MantineProvider, createTheme } from '@mantine/core';

import { ApplicationShell } from './components/app-shell';
import { msalInstance } from './infrastructure/auth/auth-config';
import { AuthorizationProvider } from './infrastructure/auth/authorization-provider';
import { initializeMsal } from './infrastructure/auth/msal';

const theme = createTheme({
  fontFamily: 'Open Sans, sans-serif',
  primaryColor: 'cyan',
});

export default function Providers({ children }: { children: React.ReactNode }) {
  useEffect(() => {
    initializeMsal();
  }, []);

  return (
    <MsalProvider instance={msalInstance}>
      <AuthorizationProvider instance={msalInstance}>
        <MantineProvider theme={theme}>
          <ApplicationShell>{children}</ApplicationShell>
        </MantineProvider>
      </AuthorizationProvider>
    </MsalProvider>
  );
}
