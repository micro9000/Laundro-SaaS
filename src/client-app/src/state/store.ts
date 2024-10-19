import type { Action, ThunkAction } from '@reduxjs/toolkit';
import { configureStore } from '@reduxjs/toolkit';

import portalReducer from '@/app/portal/portalSlice';
import userContextReducer from '@/features/userContext/userContextSlice';

// Code reference: https://redux.js.org/tutorials/essentials/part-2-app-structure

export const store = configureStore({
  reducer: {
    userContext: userContextReducer,
    portalState: portalReducer,
  },
});

// Infer the type of `store`
export type AppStore = typeof store;
export type RootState = ReturnType<AppStore['getState']>;
// Infer the `AppDispatch` type from the store itself
export type AppDispatch = AppStore['dispatch'];
// Define a reusable type describing thunk functions
export type AppThunk<ThunkReturnType = void> = ThunkAction<
  ThunkReturnType,
  RootState,
  unknown,
  Action
>;
