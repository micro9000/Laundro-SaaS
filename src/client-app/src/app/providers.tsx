"use client";

import { useEffect } from "react";
import { msalInstance } from "./infrastructure/auth/authConfig";
import { initializeMsal } from "./infrastructure/auth/msal";
import { MsalProvider } from "@azure/msal-react";
import { AuthorizationProvider } from "./infrastructure/auth/AuthorizationProvider";
import { MantineProvider, createTheme } from "@mantine/core";
import { ApplicationShell } from "./components/AppShell/ApplicationShell";

const theme = createTheme({
	fontFamily: "Open Sans, sans-serif",
	primaryColor: "cyan",
});

export default function Providers({ children }: { children: React.ReactNode }) {
	useEffect(() => {
		initializeMsal();
	}, []);

	return (
		<MsalProvider instance={msalInstance}>
			<AuthorizationProvider instance={msalInstance}>
				<MantineProvider theme={theme}>
					<ApplicationShell>{children}</ApplicationShell>
				</MantineProvider>
			</AuthorizationProvider>
		</MsalProvider>
	);
}
