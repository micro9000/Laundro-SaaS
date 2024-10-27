import { InteractionRequiredAuthError } from '@azure/msal-browser';
import { useMsal } from '@azure/msal-react';
import { useMutation as useReactMutation } from '@tanstack/react-query';
import axios, { AxiosError, AxiosProgressEvent } from 'axios';

import { loginRequest } from '../auth/authConfig';
import { Config } from '../config';

interface useQueryParams<TData extends {}, TError = unknown> {
  mutationKey: string;
  path: string;
  params?: any;
  progressCallBack?: (progressEvent: AxiosProgressEvent) => void;
}

const useAppMutation = <TData extends {}, TError = unknown>({
  mutationKey,
  path,
  params,
  progressCallBack,
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
            'Content-Type': 'multipart/form-data',
            Accept: '*/*',
          },
          params: params,
          onUploadProgress: (progressEvent: AxiosProgressEvent) => {
            progressCallBack && progressCallBack(progressEvent);
          },
        }
      );

      return response.data;
    },
  });
};

export default useAppMutation;
