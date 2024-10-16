'use client';

import { AuthenticatedTemplate } from '@azure/msal-react';

import { ApplicationShell } from '../components/appShell/appShell';

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <ApplicationShell isDesktopView={false}>
      <AuthenticatedTemplate>{children}</AuthenticatedTemplate>
    </ApplicationShell>
  );
}
