import { useEffect, useRef } from 'react';

import { InteractionRequiredAuthError } from '@azure/msal-browser';
import { useMsal } from '@azure/msal-react';
import { useMutation as useReactMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import { isArray } from 'lodash';

import { ExtractErrorMessages } from '@/utilities';

import { loginRequest } from '../auth/authConfig';
import { Config } from '../config';
import useAppNotification from './useAppNotification';

interface useQueryParams<TData extends {}, TError = unknown> {
  httpVerb?: 'post' | 'delete' | 'put';
  mutationKey: string;
  path: string;
  params?: any;
  successMessage: string;
  successCallback: () => void;
  failedMessage: string;
  failedCallback: () => void;
}

const useAppMutationWithNotification = <TData extends {}, TError = unknown>({
  mutationKey,
  path,
  params,
  httpVerb = 'post',
  successMessage,
  successCallback,
  failedMessage,
  failedCallback,
}: useQueryParams<TData, TError>) => {
  const { instance, accounts } = useMsal();
  const notification = useAppNotification();
  const notificationRef = useRef(notification); // to resolve React Hook useEffect has a missing dependency:

  const request = {
    ...loginRequest,
    account: accounts[0],
  };

  const mutationResponse = useReactMutation({
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

  const { isError, error, isSuccess, isPending } = mutationResponse;

  useEffect(() => {
    if (isError && error && error instanceof AxiosError) {
      var errorsToDisplay = ExtractErrorMessages(error);

      if (isArray(errorsToDisplay)) {
        errorsToDisplay.forEach((err) => {
          notificationRef.current.notifyError(
            failedMessage ?? 'One or more error occurred',
            err
          );
        });

        failedCallback();
      }
    }
  }, [isError, error]);

  useEffect(() => {
    if (isSuccess && !isPending) {
      notificationRef?.current.notifySuccess(successMessage);
      successCallback();
    }
  }, [isSuccess, isPending]);

  return mutationResponse;
};

export default useAppMutationWithNotification;
