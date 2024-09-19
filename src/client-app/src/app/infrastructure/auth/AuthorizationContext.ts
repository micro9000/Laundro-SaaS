import React, { useContext } from 'react';

export interface IAuthorizationContext {
  userName: string;
  userEmail: string;
  hasAnyRole: boolean;
}

export const defaultAuthorizationContext: IAuthorizationContext = {
  userName: '',
  userEmail: '',
  hasAnyRole: false,
};

export const AuthorizationContext = React.createContext<IAuthorizationContext>(
  defaultAuthorizationContext
);
export const AuthorizationConsumer = AuthorizationContext.Consumer;

export const useAuthorization = () => {
  return useContext(AuthorizationContext);
};
