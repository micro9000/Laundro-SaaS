'use client';

import React, { useEffect } from 'react';

import { useMsal } from '@azure/msal-react';
import { notifications } from '@mantine/notifications';

import {
  populateUserContextThunkAsync,
  selectUserContextStatus,
} from '@/features/userContext/userContextSlice';
import useAppQuery from '@/infrastructure/hooks/useAppQuery';
import { useAppDispatch, useAppSelector } from '@/state/hooks';

export default function UserContextProvider({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const { instance, accounts, inProgress } = useMsal();
  var dispatch = useAppDispatch();
  var userContextLoadingStatus = useAppSelector(selectUserContextStatus);

  useEffect(() => {
    dispatch(populateUserContextThunkAsync());
  }, [dispatch]);

  // useEffect(() => {
  //   msalInstance.handleRedirectPromise().then((response) => {
  //     if (response && response.account) {
  //       // User is authenticated, you can proceed to  app
  //       navigate('/Dashboard', { replace: true });
  //     }
  //   });
  //   // Check if the user is already signed in
  //   const account = msalInstance.getActiveAccount();
  //   if (account) {
  //     // User is already signed in, you can proceed to  app
  //     navigate('/Dashboard', { replace: true });
  //   } else {
  //     // If the user is not signed in, initiate the login process
  //     msalInstance.initialize();
  //   }
  // }, []);

  var { data, isLoading, isError } = useAppQuery({
    path: '/user-context-state',
    queryOptions: {
      queryKey: ['user-context-state'],
    },
  });

  console.log(data, isLoading, isError);

  useEffect(() => {
    if (userContextLoadingStatus == 'failed') {
      notifications.show({
        title: 'Failed to load user context',
        message: 'Unable to load user context due to internal server error',
      });
    }
  }, [userContextLoadingStatus]);

  return children;
}
