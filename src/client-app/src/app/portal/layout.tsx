'use client';

import { useEffect, useState } from 'react';

import { AuthenticatedTemplate } from '@azure/msal-react';

import {
  isCurrentUserIsNewUser,
  selectUserTenantGuid,
} from '@/features/userContext/userContextSlice';
import { useAppSelector } from '@/state/hooks';

import { PortalShell } from './_components/portalShell/portalShell';
import OnboardingForm from './onboarding';

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const userTenantGuid = useAppSelector(selectUserTenantGuid);
  const userIsNewUser = useAppSelector(isCurrentUserIsNewUser);
  const [isNeedToOnBoardTheUser, setIsNeedToOnBoardTheUser] = useState(false);

  useEffect(() => {
    setIsNeedToOnBoardTheUser(
      (typeof userTenantGuid === 'undefined' || userTenantGuid === null) &&
        userIsNewUser
    );
  }, [userTenantGuid, userIsNewUser]);

  return (
    <PortalShell>
      <AuthenticatedTemplate>
        <OnboardingForm isNeedToOnBoardTheUser={isNeedToOnBoardTheUser} />
        {children}
      </AuthenticatedTemplate>
    </PortalShell>
  );
}
