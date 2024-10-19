import axios from 'axios';

import { getToken } from '@/infrastructure/auth/msal';
import { Config } from '@/infrastructure/config';
import { UserContext } from '@/models/userContext';

// A mock function to mimic making an async request for data
export const fetchUserContext = async (): Promise<{
  data: UserContext | undefined;
}> => {
  var accessToken = await getToken();

  if (accessToken !== null) {
    const response = await axios.get<UserContext>(
      `${Config.ApiUrl}/user-context-state`,
      {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      }
    );

    return response;
  }

  return new Promise<{ data: UserContext | undefined }>((resolve) =>
    setTimeout(() => resolve({ data: undefined }), 500)
  );
};
