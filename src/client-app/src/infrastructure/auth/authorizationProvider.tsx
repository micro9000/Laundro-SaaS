import React, { PropsWithChildren, useEffect, useState } from 'react';

import {
  AuthenticationResult,
  EventType,
  IPublicClientApplication,
} from '@azure/msal-browser';

import {
  AuthorizationContext,
  IAuthorizationContext,
  defaultAuthorizationContext,
} from './authorizationContext';

export type AuthorizationProviderProps = PropsWithChildren<{
  instance: IPublicClientApplication;
}>;

export const AuthorizationProvider = ({
  instance,
  children,
}: AuthorizationProviderProps): React.ReactElement => {
  const [contextValue, setContextValue] = useState<IAuthorizationContext>(
    defaultAuthorizationContext
  );

  useEffect(() => {
    const callbackId = instance.addEventCallback(async (event) => {
      if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
        const payload = event.payload as AuthenticationResult;
        const account = payload.account;

        if (account) {
          setContextValue({
            userName: account?.name,
            userEmail: account?.username,
          } as IAuthorizationContext);
        }
      }
    });

    return () => {
      if (callbackId) {
        instance.removeEventCallback(callbackId);
      }
    };
  });

  useEffect(() => {
    var accounts = instance.getAllAccounts();
    var currentAccount = accounts[0];

    if (instance && accounts.length > 0 && currentAccount) {
      setContextValue({
        userName: currentAccount?.name,
        userEmail: currentAccount?.username,
      } as IAuthorizationContext);
    }
  }, [instance]);

  return (
    <AuthorizationContext.Provider value={contextValue}>
      {children}
    </AuthorizationContext.Provider>
  );
};
