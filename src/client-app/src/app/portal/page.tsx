'use client';

import React, { useEffect, useState } from 'react';

import {
  isCurrentUserIsNewUser,
  selectUserTenantGuid,
} from '@/features/userContext/userContextSlice';
import { useAppSelector } from '@/state/hooks';

import OnboardingForm from './onboarding';

export default function PortalPage() {
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
    <>
      <OnboardingForm isNeedToOnBoardTheUser={isNeedToOnBoardTheUser} />

      <h1>Portal Home page</h1>
    </>
  );
}
