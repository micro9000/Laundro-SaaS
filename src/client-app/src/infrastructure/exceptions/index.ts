export interface AppValidationError {
  statuCode: number;
  message: string;
  errors: {
    [property: string]: [];
  };
}

export interface AppGeneralError {
  statuCode: number;
  message: string;
  errors: {
    generalErrors: [];
  };
}
