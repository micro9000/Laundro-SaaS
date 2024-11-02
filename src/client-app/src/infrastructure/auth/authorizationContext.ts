import React, { useContext } from 'react';

export interface IAuthorizationContext {
  userName: string;
  userEmail: string;
}

export const defaultAuthorizationContext: IAuthorizationContext = {
  userName: '',
  userEmail: '',
};

export const AuthorizationContext = React.createContext<IAuthorizationContext>(
  defaultAuthorizationContext
);
export const AuthorizationConsumer = AuthorizationContext.Consumer;

export const useAuthorization = () => {
  return useContext(AuthorizationContext);
};
