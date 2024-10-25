import { InteractionRequiredAuthError } from '@azure/msal-browser';
import { useMsal } from '@azure/msal-react';
import { useMutation as useReactMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { loginRequest } from '../auth/authConfig';
import { Config } from '../config';

interface useQueryParams<TData extends {}, TError = unknown> {
  mutationKey: string;
  path: string;
  params?: any;
}

export interface AppError {
  statuCode: number;
  message: string;
  errors: {
    [property: string]: [];
  };
}

// axios.interceptors.response.use(
//   (res) => {
//     return res;
//   },
//   (err) => {
//     return Promise.reject(err?.response?.data as AppError);
//   }
// );

const useAppMutation = <TData extends {}, TError = unknown>({
  mutationKey,
  path,
  params,
}: useQueryParams<TData, TError>) => {
  const { instance, accounts } = useMsal();

  const request = {
    ...loginRequest,
    account: accounts[0],
  };

  return useReactMutation({
    mutationKey: [mutationKey, params],
    onError: (err: AxiosError) => err,
    mutationFn: async (formData: FormData) => {
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

      const response = await axios.post<TData>(
        `${Config.ApiUrl}${path}`,
        formData,
        {
          headers: {
            Authorization: `Bearer ${accessToken}`,
            'Content-Type': 'application/json; charset=utf8',
          },
          params: params,
        }
      );

      return response.data;
    },
  });
};

export default useAppMutation;
