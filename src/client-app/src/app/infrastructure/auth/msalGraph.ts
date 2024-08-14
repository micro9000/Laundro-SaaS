import Error from "next/error";
import { msalInstance, userDataLoginRequest, graphConfig } from "./authConfig";
import { InteractionRequiredAuthError } from "@azure/msal-browser";
import { getToken } from "./msal";

export async function getUserPhotoAvatar() {
	await msalInstance.initialize();
	const instance = msalInstance;
	const account = instance.getActiveAccount();

	if (!account) {
		throw 'No active account! Verify a user has been signed in and setActiveAccount has been called.';
	}

	const request = {
		...userDataLoginRequest,
		account: account,
	};

	var accessToken = await getToken();

	const headers = new Headers();
	headers.append("Authorization", `Bearer ${accessToken}`);

	const photoEndpoint = `${graphConfig.graphMeEndpoint}/photo/$value`;

	const options = {
		method: "GET",
		headers: headers,
	};

	return fetch(photoEndpoint, options)
		.then((response) => response.blob())
		.then((blob) => {
			const url = URL.createObjectURL(blob);

			return url;
		})
		.catch((error) => console.log(error));
}
