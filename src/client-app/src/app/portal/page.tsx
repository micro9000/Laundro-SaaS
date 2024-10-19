'use client';

import { useRouter } from 'next/navigation';
import React, { useEffect } from 'react';

import { selectUserContext } from '@/features/userContext/userContextSlice';
import { useAppSelector } from '@/state/hooks';

export default function PortalPage() {
  const router = useRouter();
  const userContext = useAppSelector(selectUserContext);

  if (userContext?.tenantGuid === null) {
    router.push('/onboarding');
  }

  return (
    <>
      <h1>Portal</h1>
    </>
  );
}
