import { AuthenticationResult, EventType } from '@azure/msal-browser';

import { Config } from '../config';
import { basePath, loginRequest, msalInstance } from './authConfig';
import { getCurrentToken } from './tokenFetcher';

export async function getToken() {
  const authToken = await getCurrentToken(msalInstance);
  // console.log("AUTH TOKEN:", authToken);
  return authToken;
}

export const handleLogin = () => {
  if (Config.SignInFlow === 'popup') {
    msalInstance.loginPopup(loginRequest).catch((e) => {
      console.error(`loginPopup failed: ${e}`);
    });
  } else if (Config.SignInFlow === 'redirect') {
    msalInstance.loginRedirect(loginRequest).catch((e) => {
      console.error(`loginRedirect failed: ${e}`);
    });
  }
};

export const handleLogout = () => {
  if (Config.SignInFlow === 'popup') {
    const logoutRequest = {
      account: msalInstance.getActiveAccount(),
      postLogoutRedirectUri: basePath,
      mainWindowRedirectUri: basePath,
    };
    msalInstance.logoutPopup(logoutRequest).catch((e: any) => {
      console.error(`logoutPopup failed: ${e}`);
    });
  } else if (Config.SignInFlow === 'redirect') {
    const logoutRequest = {
      account: msalInstance.getActiveAccount(),
      postLogoutRedirectUri: basePath,
    };
    msalInstance.logoutRedirect(logoutRequest).catch((e) => {
      console.error(`logoutRedirect failed: ${e}`);
    });
  }
};
