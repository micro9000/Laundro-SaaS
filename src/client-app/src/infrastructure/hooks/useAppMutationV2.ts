import { useEffect, useRef } from 'react';

import { InteractionRequiredAuthError } from '@azure/msal-browser';
import { useMsal } from '@azure/msal-react';
import { useMutation as useReactMutation } from '@tanstack/react-query';
import axios, {
  AxiosError,
  AxiosProgressEvent,
  AxiosRequestConfig,
} from 'axios';
import { isArray } from 'lodash';

import { ExtractErrorMessages } from '@/utilities';

import { loginRequest } from '../auth/authConfig';
import { Config } from '../config';
import useAppNotification from './useAppNotification';

interface BaseUseMutationParams {
  httpVerb?: 'post' | 'delete' | 'put';
  mutationKey: string;
  path: string;
  params?: any;
  enableMultipartForm?: boolean;
  progressCallBack?: (progressEvent: AxiosProgressEvent) => void;
  enableNotification?: boolean;
}

interface EnableNotificationUseMutationParams extends BaseUseMutationParams {
  enableNotification: true;
  successMessage: string;
  successCallback: () => void;
  failedMessage: string;
  failedCallback: () => void;
}

interface DisableNotificationUseMutationParams extends BaseUseMutationParams {
  enableNotification: false;
}

type useMutationParams<TData extends {}, TError = unknown> =
  | EnableNotificationUseMutationParams
  | DisableNotificationUseMutationParams;

const useAppMutation = <TData extends {}, TError = unknown>(
  props: useMutationParams<TData, TError>
) => {
  const { instance, accounts } = useMsal();
  const notification = useAppNotification();
  const notificationRef = useRef(notification); // to resolve React Hook useEffect has a missing dependency:

  const {
    mutationKey,
    path,
    params,
    httpVerb = 'post',
    enableMultipartForm,
    progressCallBack,
    enableNotification,
  } = props;

  const request = {
    ...loginRequest,
    account: accounts[0],
  };

  const mutationResponse = useReactMutation({
    mutationKey: [mutationKey, params],
    onError: (err: AxiosError) => err,
    mutationFn: async (formData: FormData) => {
      var accessToken = null;

      let defaultAxiosConfig: AxiosRequestConfig<any> | undefined = {
        data: formData,
        headers: {
          Authorization: `Bearer ${accessToken}`,
          'Content-Type': 'application/json; charset=utf8',
        },
        params: params,
      };

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
        const response = await axios.delete<TData>(
          `${Config.ApiUrl}${path}`,
          defaultAxiosConfig
        );

        return response.data;
      } else if (httpVerb === 'put') {
        const response = await axios.put<TData>(
          `${Config.ApiUrl}${path}`,
          defaultAxiosConfig
        );

        return response.data;
      }

      if (enableMultipartForm) {
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
    if (isError && error && error instanceof AxiosError && enableNotification) {
      var errorsToDisplay = ExtractErrorMessages(error);

      if (isArray(errorsToDisplay)) {
        errorsToDisplay.forEach((err) => {
          notificationRef.current.notifyError(
            props.failedMessage ?? 'One or more error occurred',
            err
          );
        });

        props.failedCallback();
      }
    }
  }, [isError, error, enableNotification]);

  useEffect(() => {
    if (isSuccess && !isPending && enableNotification) {
      notificationRef?.current.notifySuccess(props.successMessage);
      props.successCallback();
    }
  }, [isSuccess, isPending, enableNotification]);

  return mutationResponse;
};

export default useAppMutation;
