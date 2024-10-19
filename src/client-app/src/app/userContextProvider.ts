'use client';

import { useRouter } from 'next/navigation';
import React, { useEffect } from 'react';

import { useMsal } from '@azure/msal-react';

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

  useEffect(() => {
    dispatch(populateUserContextThunkAsync());
  }, [dispatch]);

  useEffect(() => {
    msalInstance.handleRedirectPromise().then((response) => {
      if (response && response.account) {
        if (typeof userTenantGuid !== 'undefined' && userTenantGuid !== null) {
          router.replace('/portal');
        } else {
          router.replace('/onboarding');
        }
      }
    });
    const account = msalInstance.getActiveAccount();
    if (account) {
      if (typeof userTenantGuid !== 'undefined' && userTenantGuid !== null) {
        router.replace('/portal');
      } else {
        router.replace('/onboarding');
      }
    } else {
      // If the user is not signed in, initiate the login process
      // msalInstance.initialize();
    }
  }, [router, msalInstance, userTenantGuid, userContextLoadingStatus]);

  // var { data, isLoading, isError } = useAppQuery<UserContext>({
  //   path: '/user-context-state',
  //   queryOptions: {
  //     queryKey: ['user-context-state'],
  //   },
  // });

  // var mutateTenant = useAppMutation({
  //   mutationKey: 'create-tenant',
  //   path: '/tenant/create',
  // });

  // useEffect(() => {
  //   var formData = new FormData();
  //   formData.append('name', 'test');
  //   mutateTenant.mutate(formData);
  // }, []);

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
