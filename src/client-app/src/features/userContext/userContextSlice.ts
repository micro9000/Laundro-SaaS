// for defining reducer logic and actions, as well as related thunks and selectors.
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';

import { UserRoles } from '@/constants';
import { UserContext } from '@/models/userContext';
import type { AppThunk, RootState } from '@/state/store';

import { fetchUserContext } from './userContextQueryApi';

export interface UserContextState {
  userContext?: UserContext | null;
  status: 'idle' | 'loading' | 'failed';
}

const initialState: UserContextState = {
  userContext: null,
  status: 'idle',
};

export const userContextSlice = createSlice({
  name: 'userContext',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(populateUserContextThunkAsync.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(populateUserContextThunkAsync.fulfilled, (state, action) => {
        state.userContext = action.payload;
        state.status = 'idle';
      })
      .addCase(populateUserContextThunkAsync.rejected, (state) => {
        state.status = 'failed';
      });
  },
});

export default userContextSlice.reducer;

// Selectors
export const selectUserContext = (state: RootState) =>
  state.userContext.userContext;
export const selectUserContextStatus = (state: RootState) =>
  state.userContext.status;
export const selectUserTenantName = (state: RootState) =>
  state.userContext.userContext?.tenantName;
export const selectUserTenantGuid = (state: RootState) =>
  state.userContext.userContext?.tenantGuid;

// Current User Role selectors
export const isCurrentUserIsNewUser = (state: RootState): boolean =>
  state.userContext.userContext?.roleSystemKey === UserRoles.new_user;
export const isCurrentUserIsTenantOwner = (state: RootState): boolean =>
  state.userContext.userContext?.roleSystemKey === UserRoles.tenant_owner;
export const isCurrentUserIsTenantEmployee = (state: RootState): boolean =>
  state.userContext.userContext?.roleSystemKey === UserRoles.tenant_employee;
export const isCurrentUserIsStoreManager = (state: RootState): boolean =>
  state.userContext.userContext?.roleSystemKey === UserRoles.store_manager;
export const isCurrentUserIsStoreStaff = (state: RootState): boolean =>
  state.userContext.userContext?.roleSystemKey === UserRoles.store_staff;

export const populateUserContextThunkAsync = createAsyncThunk(
  'userContext/fetchUserContext',
  async () => {
    const response = await fetchUserContext();
    return response.data;
  }
);
