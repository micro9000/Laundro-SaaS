import React from 'react';

import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
} from '@azure/msal-react';

import { LoginButton } from './loginButton';
import { LogoutButton } from './logoutButton';

export function AuthButton() {
  return (
    <>
      <AuthenticatedTemplate>
        <LogoutButton />
      </AuthenticatedTemplate>
      <UnauthenticatedTemplate>
        <LoginButton />
      </UnauthenticatedTemplate>
    </>
  );
}
