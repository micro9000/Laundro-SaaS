'use client';

import { AuthenticatedTemplate } from '@azure/msal-react';

import { ApplicationShell } from '@/app/components/app-shell/app-shell';

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <ApplicationShell>
      <AuthenticatedTemplate>{children}</AuthenticatedTemplate>
    </ApplicationShell>
  );
}
