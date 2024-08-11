import { EventMessage, EventType, IPublicClientApplication } from "@azure/msal-browser";
import React, { PropsWithChildren, useEffect, useState } from "react";
import { AuthorizationContext, IAuthorizationContext, defaultAuthorizationContext } from "./AuthorizationContext";
import { UserRoles } from "@/app/constants/UserRoles";
import { has as hasProperty } from "lodash";

export type AuthorizationProviderProps = PropsWithChildren<{
    instance: IPublicClientApplication
}>;

export const AuthorizationProvider = ({instance, children}: AuthorizationProviderProps): React.ReactElement => {

    const [contextValue, setContextValue] = useState<IAuthorizationContext>(defaultAuthorizationContext);

    const TmpRolesRequired: string[] = [UserRoles.BusinessOwner];

    const UpdateContextValue = () => {
        var accounts = instance.getAllAccounts();
        var currentAccount = accounts[0];
        var currentRole: string | undefined = currentAccount?.idTokenClaims?.roles?.at(0);

        setContextValue({
            hasAnyRole: hasProperty(UserRoles, currentRole ?? "")
        } as IAuthorizationContext);
    }

    instance.addEventCallback((message: EventMessage) => {
        if (message.eventType === EventType.LOGIN_SUCCESS){
            UpdateContextValue();
        }
    });

    var activeAccount = instance.getActiveAccount();

    useEffect(() => {
        UpdateContextValue();
    }, [activeAccount]);

    return <AuthorizationContext.Provider value={contextValue}>{children}</AuthorizationContext.Provider>
}