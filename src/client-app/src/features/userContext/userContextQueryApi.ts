import axios from 'axios';

import { getToken } from '@/infrastructure/auth/msal';
import { Config } from '@/infrastructure/config';
import { UserContext } from '@/models/userContext';

// A mock function to mimic making an async request for data
export const fetchUserContext = async () => {
  var accessToken = await getToken();

  const response = await axios.get<UserContext>(
    `${Config.ApiUrl}/user-context-state`,
    {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    }
  );

  return new Promise<{ data: UserContext }>((resolve) =>
    setTimeout(
      () =>
        resolve({
          data: response.data,
        }),
      500
    )
  );
};
