'use client';

import { AuthenticatedTemplate } from '@azure/msal-react';

import { ApplicationShell } from '@/app/components/appShell/appShell';

export default function OnboardingLayout({
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
