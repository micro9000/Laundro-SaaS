import { PayloadAction, createSlice } from '@reduxjs/toolkit';

import { RootState } from '../../state/store';

export interface PortalState {
  currentPage: string;
}

const initialState: PortalState = {
  currentPage: '/portal',
};

export const portalSlice = createSlice({
  name: 'portalState',
  initialState,
  reducers: {
    setActivePage: (state, action: PayloadAction<string>) => {
      state.currentPage = action.payload;
    },
  },
});

export const { setActivePage } = portalSlice.actions;

// Export the slice reducer for use in the store configuration
export default portalSlice.reducer;

export const selectActivePage = (state: RootState) =>
  state.portalState.currentPage;
