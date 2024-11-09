import { AxiosError } from 'axios';
import { hasIn, isArrayLikeObject } from 'lodash';

import {
  AppGeneralError,
  AppValidationError,
} from '@/infrastructure/exceptions';

export const nameof = <T>(name: Extract<keyof T, string>): string => name;

export const ExtractFormValidationErrorMessages = (
  axiosError: AxiosError<unknown, any> | null
): { [property: string]: string | undefined } => {
  var validationErrors = (axiosError?.response?.data as AppValidationError)
    ?.errors;

  var errorsToDisplay: { [property: string]: string | undefined } = {};
  if (validationErrors) {
    Object.keys(validationErrors).forEach((key) => {
      let property = key
        .split(/\.?(?=[A-Z])/)
        .join('_')
        .toLowerCase();

      let validationErr = validationErrors[key]?.at(0);
      errorsToDisplay[property] = validationErr;
    });
  }

  return errorsToDisplay;
};

export const ExtractErrorMessages = (
  axiosError: AxiosError<unknown, any>
): string[] => {
  let errorMessages: string[] = [];
  if (hasIn(axiosError?.response?.data, 'errors.generalErrors')) {
    errorMessages = (axiosError?.response?.data as AppGeneralError).errors
      .generalErrors;
    return errorMessages;
  }

  if (hasIn(axiosError?.response?.data, 'errors')) {
    var validationErrors = (axiosError?.response?.data as AppValidationError)
      ?.errors;

    if (validationErrors) {
      Object.keys(validationErrors).forEach((key) => {
        let property = key
          .split(/\.?(?=[A-Z])/)
          .join('_')
          .toLowerCase();
        let validationErr = validationErrors[key]?.at(0);

        errorMessages.push(`${property} - ${validationErr}`);
      });
    }
  }

  if ((axiosError.status ?? 500) >= 500) {
    return ['Internal Server Error'];
  }

  return errorMessages;
};
