'use client';

import { useRouter } from 'next/navigation';
import React, { useEffect } from 'react';

import { selectUserContext } from '@/features/userContext/userContextSlice';
import { useAppSelector } from '@/state/hooks';

export default function Onboarding() {
  const router = useRouter();
  const userContext = useAppSelector(selectUserContext);

  if (userContext?.tenantGuid !== null) {
    router.push('/portal');
  }

  return (
    <>
      <h1>Create New Store</h1>
    </>
  );
}
