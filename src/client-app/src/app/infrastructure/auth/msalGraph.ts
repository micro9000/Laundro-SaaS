import Error from "next/error";
import { msalInstance, userDataLoginRequest, graphConfig } from "./authConfig";

export async function getUserPhotoAvatar() {
	const instance = msalInstance;
	const account = instance.getActiveAccount();

	if (!account) {
		throw 'No active account! Verify a user has been signed in and setActiveAccount has been called.';
	}

	await instance.initialize();
	const tokenResponse = await instance.acquireTokenSilent({
		...userDataLoginRequest,
		account: account,
	});

	const headers = new Headers();
	headers.append("Authorization", `Bearer ${tokenResponse.accessToken}`);

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
