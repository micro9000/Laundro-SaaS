'use client';

import { AuthenticatedTemplate } from '@azure/msal-react';

import { PortalShell } from '../components/portalShell/portalShell';

export default function OnboardingLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <PortalShell>
      <AuthenticatedTemplate>{children}</AuthenticatedTemplate>
    </PortalShell>
  );
}
