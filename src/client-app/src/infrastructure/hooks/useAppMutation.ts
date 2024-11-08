import { InteractionRequiredAuthError } from '@azure/msal-browser';
import { useMsal } from '@azure/msal-react';
import { useMutation as useReactMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { loginRequest } from '../auth/authConfig';
import { Config } from '../config';

interface useQueryParams<TData extends {}, TError = unknown> {
  httpVerb?: 'post' | 'delete' | 'put';
  mutationKey: string;
  path: string;
  params?: any;
}

const useAppMutation = <TData extends {}, TError = unknown>({
  mutationKey,
  path,
  params,
  httpVerb = 'post',
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

      if (httpVerb === 'delete') {
        const response = await axios.delete<TData>(`${Config.ApiUrl}${path}`, {
          data: formData,
          headers: {
            Authorization: `Bearer ${accessToken}`,
            'Content-Type': 'application/json; charset=utf8',
          },
          params: params,
        });

        return response.data;
      } else if (httpVerb === 'put') {
        const response = await axios.put<TData>(`${Config.ApiUrl}${path}`, {
          data: formData,
          headers: {
            Authorization: `Bearer ${accessToken}`,
            'Content-Type': 'application/json; charset=utf8',
          },
          params: params,
        });

        return response.data;
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
