import { getToken } from '@/infrastructure/auth/msal';
import { UserContext } from '@/models/userContext';

// A mock function to mimic making an async request for data
export const fetchUserContext = async () => {
  var token = await getToken();

  console.log(token);

  return new Promise<{ data: UserContext }>((resolve) =>
    setTimeout(
      () =>
        resolve({
          data: {
            userId: 1,
            email: 'raniel.garcia@test.com',
          } as UserContext,
        }),
      500
    )
  );
};
