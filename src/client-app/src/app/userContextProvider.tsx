'use client';

import { useRouter } from 'next/navigation';
import React, { useEffect, useState } from 'react';

import { useMsal } from '@azure/msal-react';
import { LoadingOverlay } from '@mantine/core';

import {
  populateUserContextThunkAsync,
  selectUserContextStatus,
  selectUserTenantGuid,
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
  var userTenantGuid = useAppSelector(selectUserTenantGuid);
  const [isLoadingOverlayVisible, setIsLoadingOverlayVisible] =
    useState<boolean>(true);

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
    if (userContextLoadingStatus === 'idle' && accounts.length > 0) {
      setIsLoadingOverlayVisible(false);
    } else {
      setIsLoadingOverlayVisible(true);
    }
  }, [
    userContextLoadingStatus,
    notifyError,
    dispatch,
    accounts.length,
    userTenantGuid,
  ]);

  // useEffect(() => {
  //   if (!userTenantGuid) {
  //     if (userTenantGuid == && userContextLoadingStatus !== 'idle' && accounts.length > 0) {
  //       dispatch(setLoadingOverlayVisible());
  //     }
  //   }
  // }, [dispatch, accounts.length, userContextLoadingStatus, userTenantGuid]);

  return (
    <>
      <LoadingOverlay
        visible={isLoadingOverlayVisible}
        zIndex={1000}
        overlayProps={{ radius: 'sm', blur: 2 }}
      />
      {children}
    </>
  );
}
