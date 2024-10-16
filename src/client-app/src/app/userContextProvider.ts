'use client';

import React, { useEffect } from 'react';

import { notifications } from '@mantine/notifications';

import {
  populateUserContextThunkAsync,
  selectUserContextStatus,
} from '@/features/userContext/userContextSlice';
import { useAppDispatch, useAppSelector } from '@/state/hooks';

export default function UserContextProvider({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  var dispatch = useAppDispatch();
  var userContextLoadingStatus = useAppSelector(selectUserContextStatus);

  useEffect(() => {
    dispatch(populateUserContextThunkAsync());
  }, [dispatch]);

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
