'use client';

import React, { useState } from 'react';

import { LoadingOverlay } from '@mantine/core';

export default function UserContextProvider({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  // const pathname = usePathname();
  // const router = useRouter();
  // const { notifyError } = useAppNotification();
  // const { instance: msalInstance, accounts } = useMsal();

  // var userContextLoadingStatus = useAppSelector(selectUserContextStatus);
  // var userTenantGuid = useAppSelector(selectUserTenantGuid);
  const [isLoadingOverlayVisible, setIsLoadingOverlayVisible] =
    useState<boolean>(true);

  // useEffect(() => {
  //   msalInstance.handleRedirectPromise().then((response) => {
  //     // if (!response && pathname !== '/') {
  //     //   msalInstance.loginRedirect(loginRequest).catch((e) => {
  //     //     console.error(`loginRedirect failed: ${e}`);
  //     //   });
  //     // }

  //     if (
  //       response &&
  //       response.account &&
  //       pathname === '/' &&
  //       userContextLoadingStatus === 'idle'
  //     ) {
  //       router.replace('/portal');
  //     }
  //   });
  //   const account = msalInstance.getActiveAccount();
  //   if (account && pathname === '/' && userContextLoadingStatus === 'idle') {
  //     router.replace('/portal');
  //   } else {
  //     // If the user is not signed in, initiate the login process
  //     // this is currently not supported, let the user click the login button
  //     console.log('here');
  //     msalInstance.initialize();
  //   }
  // }, [router, msalInstance, pathname, userContextLoadingStatus]);

  // useEffect(() => {
  //   if (userContextLoadingStatus == 'failed' && accounts.length > 0) {
  //     notifyError(
  //       'Failed to load user context',
  //       'Unable to load user context due to internal server error'
  //     );
  //   }
  //   if (userContextLoadingStatus === 'idle' && accounts.length > 0) {
  //     setIsLoadingOverlayVisible(false);
  //   } else if (userContextLoadingStatus === 'empty' && accounts.length === 0) {
  //     setIsLoadingOverlayVisible(false);
  //   } else {
  //     setIsLoadingOverlayVisible(true);
  //   }
  // }, [
  //   userContextLoadingStatus,
  //   notifyError,
  //   dispatch,
  //   accounts.length,
  //   userTenantGuid,
  // ]);

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
