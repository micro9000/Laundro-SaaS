// for defining reducer logic and actions, as well as related thunks and selectors.
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';

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
export const selectUserTenant = (state: RootState) =>
  state.userContext.userContext?.tenant;

export const populateUserContextThunkAsync = createAsyncThunk(
  'userContext/fetchUserContext',
  async () => {
    const response = await fetchUserContext();
    return response.data;
  }
);
