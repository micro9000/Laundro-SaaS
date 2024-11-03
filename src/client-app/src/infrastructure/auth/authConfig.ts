import { LogLevel, PublicClientApplication } from '@azure/msal-browser';

import { Config } from '../config';

export var basePath = `${window.location.protocol}//${window.location.host}/`;

export const msalConfig = {
  auth: {
    clientId: Config.Auth.ClientId, // This is the ONLY mandatory field that you need to supply.
    authority: Config.Auth.Authority, // Replace the placeholder with your tenant subdomain
    redirectUri: basePath, // Points to window.location.origin. You must register this URI on Azure Portal/App Registration.
    postLogoutRedirectUri: basePath, // Indicates the page to navigate after logout.
    navigateToLoginRequestUrl: false, // If "true", will navigate back to the original request location before processing the auth code response.
  },
  cache: {
    cacheLocation: 'localStorage', // Configures cache location. "sessionStorage" is more secure, but "localStorage" gives you SSO between tabs.
    storeAuthStateInCookie: false, // Set this to "true" if you are having issues on IE11 or Edge
  },
  system: {
    loggerOptions: {
      loggerCallback: (level: any, message: any, containsPii: any) => {
        if (Config.VerboseAuthLogging == 'false') {
          return;
        }
        if (Config.LogSensitiveInformation == 'false' && containsPii) {
          return;
        }
        switch (level) {
          case LogLevel.Error:
            console.error(message);
            return;
          case LogLevel.Info:
            console.info(message);
            return;
          case LogLevel.Verbose:
            console.debug(message);
            return;
          case LogLevel.Warning:
            console.warn(message);
            return;
          default:
            return;
        }
      },
      piiLoggingEnabled:
        Config.VerboseAuthLogging == 'true' &&
        Config.LogSensitiveInformation == 'true',
    },
  },
};

export const loginRequest = {
  scopes: [Config.Auth.Scope],
};

export const userDataLoginRequest = {
  scopes: ['user.read'],
};

export const graphConfig = {
  graphMeEndpoint: 'https://graph.microsoft.com/v1.0/me',
};

export const msalInstance = new PublicClientApplication(msalConfig);
