'use client';

import { useRouter } from 'next/navigation';
import React, { useEffect } from 'react';

import { useMsal } from '@azure/msal-react';

import {
  populateUserContextThunkAsync,
  selectUserContextStatus,
} from '@/features/userContext/userContextSlice';
import { useAppNotification } from '@/infrastructure/hooks';
import { useAppDispatch, useAppSelector } from '@/state/hooks';

export default function UserContextProvider({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const router = useRouter();
  const { notifyError } = useAppNotification();
  const { instance: msalInstance, accounts } = useMsal();
  var dispatch = useAppDispatch();
  var userContextLoadingStatus = useAppSelector(selectUserContextStatus);

  useEffect(() => {
    dispatch(populateUserContextThunkAsync());
  }, [dispatch]);

  useEffect(() => {
    msalInstance.handleRedirectPromise().then((response) => {
      if (response && response.account) {
        router.replace('/portal');
      }
    });
    const account = msalInstance.getActiveAccount();
    if (account) {
      router.replace('/portal');
    } else {
      // If the user is not signed in, initiate the login process
      // msalInstance.initialize();
    }
  }, [router, msalInstance]);

  useEffect(() => {
    if (userContextLoadingStatus == 'failed' && accounts.length > 0) {
      notifyError(
        'Failed to load user context',
        'Unable to load user context due to internal server error'
      );
    }
  }, [userContextLoadingStatus, notifyError, accounts.length]);

  return children;
}
