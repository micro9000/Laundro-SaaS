import { PayloadAction, createSlice } from '@reduxjs/toolkit';

import { RootState } from '../../state/store';

export interface PortalState {
  activeStore: string;
}

const initialState: PortalState = {
  activeStore: '/portal',
};

export const portalSlice = createSlice({
  name: 'portalState',
  initialState,
  reducers: {
    setActiveStore: (state, action: PayloadAction<string>) => {
      state.activeStore = action.payload;
    },
  },
});

export const { setActiveStore } = portalSlice.actions;

// Export the slice reducer for use in the store configuration
export default portalSlice.reducer;

export const selectActivePage = (state: RootState) =>
  state.portalState.activeStore;
