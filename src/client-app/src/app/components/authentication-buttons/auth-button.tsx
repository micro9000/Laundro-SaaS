import React from 'react';

import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
} from '@azure/msal-react';

import { LoginButton } from './login-button';
import { LogoutButton } from './logout-button';

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
