import React, {useContext} from "react";

export interface IAuthorizationContext {
    hasAnyRole: boolean;
}

export const defaultAuthorizationContext: IAuthorizationContext ={
    hasAnyRole: false
}

export const AuthorizationContext = React.createContext<IAuthorizationContext>(defaultAuthorizationContext);
export const AuthorizationConsumer = AuthorizationContext.Consumer;

export const useAuthorization = () => {
    return useContext(AuthorizationContext);
}