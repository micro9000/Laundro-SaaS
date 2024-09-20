import { AuthenticationResult, EventType } from '@azure/msal-browser';

import { Config } from '../config';
import { loginRequest, msalInstance } from './auth-config';
import { getCurrentToken } from './tokenFetcher';

export async function initializeMsal() {
  console.log('=> msal initialization..');

  await msalInstance.initialize();

  const accounts = msalInstance.getAllAccounts();
  if (accounts.length > 0) {
    msalInstance.setActiveAccount(accounts[0]);
  }

  msalInstance.addEventCallback(async (event) => {
    if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
      const payload = event.payload as AuthenticationResult;
      const account = payload.account;
      msalInstance.setActiveAccount(account);
    }
  });
}

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
    msalInstance.logoutPopup().catch((e: any) => {
      console.error(`logoutPopup failed: ${e}`);
    });
  } else if (Config.SignInFlow === 'redirect') {
    const logoutRequest = {
      account: msalInstance.getActiveAccount(),
      postLogoutRedirectUri: '/',
    };
    msalInstance.logoutRedirect(logoutRequest).catch((e) => {
      console.error(`logoutRedirect failed: ${e}`);
    });
  }
};
