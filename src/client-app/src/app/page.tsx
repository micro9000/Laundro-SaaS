'use client'
import { MsalProvider } from "@azure/msal-react"
import { msalInstance } from "./infrastructure/auth/authConfig";
import { AuthorizationProvider } from "./infrastructure/auth/AuthorizationProvider";

export default function Home() {
  return (
    <MsalProvider instance={msalInstance}>
      <AuthorizationProvider instance={msalInstance}>
      <h1>Hello world</h1>
      </AuthorizationProvider>
    </MsalProvider>
  );
}
