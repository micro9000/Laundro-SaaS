'use client';

import React, { useEffect } from 'react';

import {
  populateUserContextThunkAsync,
  selectUserContext,
} from '@/features/userContext/userContextSlice';
import { useAppDispatch } from '@/state/hooks';

export default function UserContextProvider({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  var dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(populateUserContextThunkAsync());
  }, [dispatch]);

  // TODO: Add some error handling here and show some modal or redirect the user to a page that shows the friendly error message

  return children;
}
