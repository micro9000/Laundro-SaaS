// for defining reducer logic and actions, as well as related thunks and selectors.
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';

import { StoreLevelUserRoles, TenantLevelUserRoles } from '@/constants';
import { Store, UserContext } from '@/models';
import type { RootState } from '@/state/store';

import { fetchUserContext } from './userContextQueryApi';

export interface UserContextState {
  userContext?: UserContext | null;
  status: 'idle' | 'loading' | 'failed' | 'empty';
  currentSelectedStore?: Store;
}

const initialState: UserContextState = {
  userContext: null,
  status: 'idle',
};

export const userContextSlice = createSlice({
  name: 'userContext',
  initialState,
  reducers: {
    setCurrentSelectedStore: (state, action: PayloadAction<Store>) => {
      state.currentSelectedStore = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(populateUserContextThunkAsync.pending, (state) => {
        state.status = 'loading';
      })
      .addCase(
        populateUserContextThunkAsync.fulfilled,
        (state, action: PayloadAction<UserContext | undefined>) => {
          if (
            typeof action.payload === 'undefined' ||
            action.payload === null
          ) {
            state.userContext = null;
            state.currentSelectedStore = undefined;
            state.status = 'empty';
          } else {
            state.userContext = action.payload;
            state.currentSelectedStore = action.payload?.stores?.at(0);
            state.status = 'idle';
          }
        }
      )
      .addCase(populateUserContextThunkAsync.rejected, (state) => {
        state.status = 'failed';
      });
  },
});

export default userContextSlice.reducer;
export const { setCurrentSelectedStore } = userContextSlice.actions;

// Tenant level Selectors
export const selectUserContext = (state: RootState) =>
  state.userContext?.userContext;
export const selectUserContextStatus = (state: RootState) =>
  state.userContext?.status;
export const selectUserTenantName = (state: RootState) =>
  state.userContext?.userContext?.tenantName;
export const selectUserTenantGuid = (state: RootState) =>
  state.userContext?.userContext?.tenantGuid;

export const hasTenant = (state: RootState) =>
  typeof state.userContext?.userContext?.tenantGuid !== 'undefined' &&
  state.userContext?.userContext?.tenantGuid !== null;

export const selectStores = (state: RootState) =>
  state.userContext?.userContext?.stores;

// Current selected store selectors
export const selectCurrentSelectedStore = (state: RootState) =>
  state.userContext?.currentSelectedStore;

// Current User Role selectors
export const isCurrentUserIsNewUser = (state: RootState): boolean =>
  state.userContext?.userContext?.roleSystemKey ===
  TenantLevelUserRoles.new_user;
// export const isCurrentUserIsTenantOwner = (state: RootState): boolean =>
//   state.userContext?.userContext?.roleSystemKey === UserRoles.tenant_owner;
// export const isCurrentUserIsTenantEmployee = (state: RootState): boolean =>
//   state.userContext?.userContext?.roleSystemKey === UserRoles.tenant_employee;
// export const isCurrentUserIsStoreManager = (state: RootState): boolean =>
//   state.userContext?.userContext?.roleSystemKey === UserRoles.store_manager;
// export const isCurrentUserIsStoreStaff = (state: RootState): boolean =>
//   state.userContext?.userContext?.roleSystemKey === UserRoles.store_staff;

export const populateUserContextThunkAsync = createAsyncThunk(
  'userContext/fetchUserContext',
  async () => {
    const response = await fetchUserContext();
    return response.data;
  }
);
