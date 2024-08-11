declare global {
    namespace NodeJS {
        interface ProcessEnv {
            NODE_ENV: 'development' | 'production';
            NEXT_PUBLIC_API_URL: string;
            NEXT_PUBLIC_OAUTH_CLIENT_ID:string;
            NEXT_PUBLIC_OAUTH_AUTHORITY:string;
            NEXT_PUBLIC_OAUTH_REDIRECT_URI:string;
            NEXT_PUBLIC_OAUTH_SCOPE:sring
            NEXT_PUBLIC_SIGNIN_FLOW: 'popup' | 'redirect'
            NEXT_PUBLIC_VERBOSE_AUTH_LOGGING: 'true' | 'false'
            NEXT_PUBLIC_LOG_SENSITIVE_INFORMATION: 'true' | 'false' 
        }
    }
}

// If this file has no import/export statements (i.e. is a script)
// convert it into a module by adding an empty export statement.
export {}