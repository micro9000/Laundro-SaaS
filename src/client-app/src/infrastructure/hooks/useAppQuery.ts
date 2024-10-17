import { InteractionRequiredAuthError } from '@azure/msal-browser';
import { useMsal } from '@azure/msal-react';
import {
  UseQueryOptions,
  useQuery as useReactQuery,
} from '@tanstack/react-query';
import axios from 'axios';

import { loginRequest } from '../auth/authConfig';
import { Config } from '../config';

interface useQueryParams<TData extends {}, TError = unknown> {
  path: string;
  params?: any;
  queryOptions?: UseQueryOptions<TData, TError>;
}

const useAppQuery = <TData extends {}, TError = unknown>({
  path,
  params,
  queryOptions,
}: useQueryParams<TData, TError>) => {
  const { instance, accounts } = useMsal();

  const request = {
    ...loginRequest,
    account: accounts[0],
  };

  queryOptions = {
    ...queryOptions,
    queryFn: async () => {
      var accessToken = null;

      try {
        var tokenResponse = await instance.acquireTokenSilent(request);
        accessToken = tokenResponse.accessToken;
      } catch (e) {
        if (e instanceof InteractionRequiredAuthError) {
          var tokenResponse = await instance.acquireTokenPopup(request);
          accessToken = tokenResponse.accessToken;
        }
      }

      const response = await axios.get<TData>(`${Config.ApiUrl}${path}`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
        params: params,
      });

      return response.data;
    },
  } as UseQueryOptions<TData, TError>;

  return useReactQuery({ ...queryOptions });
};

export default useAppQuery;
