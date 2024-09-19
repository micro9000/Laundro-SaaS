export const Config = {
  Auth: {
    ClientId: process.env.NEXT_PUBLIC_OAUTH_CLIENT_ID,
    Authority: process.env.NEXT_PUBLIC_OAUTH_AUTHORITY,
    RedirectUri: process.env.NEXT_PUBLIC_OAUTH_REDIRECT_URI,
    Scope: process.env.NEXT_PUBLIC_OAUTH_SCOPE,
  },
  SignInFlow: process.env.NEXT_PUBLIC_SIGNIN_FLOW,
  VerboseAuthLogging: process.env.NEXT_PUBLIC_VERBOSE_AUTH_LOGGING,
  LogSensitiveInformation: process.env.NEXT_PUBLIC_LOG_SENSITIVE_INFORMATION,
  ApiUrl: process.env.NEXT_PUBLIC_API_URL,
} as const;
