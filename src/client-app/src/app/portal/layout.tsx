'use client';

import { AuthenticatedTemplate } from '@azure/msal-react';

import { ApplicationShell } from '../components/app-shell/app-shell';

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
