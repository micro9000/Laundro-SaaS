import React, {
  PropsWithChildren,
  useCallback,
  useEffect,
  useMemo,
  useState,
} from 'react';

import {
  EventMessage,
  EventType,
  IPublicClientApplication,
} from '@azure/msal-browser';
import { has as hasProperty } from 'lodash';

import { UserRoles } from '@/constants';

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

  const TmpRolesRequired: string[] = useMemo(() => [UserRoles.new_user], []);

  const UpdateContextValue = useCallback(() => {
    var accounts = instance.getAllAccounts();
    var currentAccount = accounts[0];
    var currentRole: string | undefined =
      currentAccount?.idTokenClaims?.roles?.at(0);

    setContextValue({
      userName: currentAccount?.name,
      userEmail: currentAccount?.username,
      hasAnyRole: hasProperty(TmpRolesRequired, currentRole ?? ''),
    } as IAuthorizationContext);
  }, [instance, TmpRolesRequired]);

  instance.addEventCallback((message: EventMessage) => {
    if (message.eventType === EventType.LOGIN_SUCCESS) {
      UpdateContextValue();
    }
  });

  var activeAccount = instance.getActiveAccount();

  useEffect(() => {
    UpdateContextValue();
  }, [UpdateContextValue, activeAccount?.username]);

  return (
    <AuthorizationContext.Provider value={contextValue}>
      {children}
    </AuthorizationContext.Provider>
  );
};
