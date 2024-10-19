'use client';

import React, { useEffect } from 'react';

import {
  populateUserContextThunkAsync,
  selectUserContextStatus,
} from '@/features/userContext/userContextSlice';
import {
  useAppMutation,
  useAppNotification,
  useAppQuery,
} from '@/infrastructure/hooks';
import { UserContext } from '@/models/userContext';
import { useAppDispatch, useAppSelector } from '@/state/hooks';

export default function UserContextProvider({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const { notifyError } = useAppNotification();
  // const { instance, accounts, inProgress } = useMsal();
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

  // var { data, isLoading, isError } = useAppQuery<UserContext>({
  //   path: '/user-context-state',
  //   queryOptions: {
  //     queryKey: ['user-context-state'],
  //   },
  // });

  var mutateTenant = useAppMutation({
    mutationKey: 'create-tenant',
    path: '/tenant/create',
  });

  useEffect(() => {
    var formData = new FormData();
    formData.append('name', 'test');
    mutateTenant.mutate(formData);
  }, []);

  useEffect(() => {
    if (userContextLoadingStatus == 'failed') {
      notifyError(
        'Failed to load user context',
        'Unable to load user context due to internal server error'
      );
    }
  }, [userContextLoadingStatus, notifyError]);

  return children;
}
