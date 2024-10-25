import {
  Combobox,
  Group,
  Input,
  InputBase,
  Stack,
  Text,
  useCombobox,
} from '@mantine/core';
import { IconCheck } from '@tabler/icons-react';

import {
  selectCurrentSelectedStore,
  selectStores,
  setCurrentSelectedStore,
} from '@/features/userContext/userContextSlice';
import { useAppDispatch, useAppSelector } from '@/state/hooks';

export function StoreSwitch() {
  const dispatch = useAppDispatch();
  const combobox = useCombobox({
    onDropdownClose: () => combobox.resetSelectedOption(),
    onDropdownOpen: (eventSource) => {
      if (eventSource === 'keyboard') {
        combobox.selectActiveOption();
      } else {
        combobox.updateSelectedOptionIndex('active');
      }
    },
  });
  const currentStore = useAppSelector(selectCurrentSelectedStore);
  const stores = useAppSelector(selectStores);

  const storeOptions = stores?.map((store) => (
    <Combobox.Option
      value={store.id.toString()}
      key={store.id}
      active={store.id === currentStore?.id}
    >
      <Group gap="xs">
        {store.id === currentStore?.id && <IconCheck size={12} />}
        <span>{store.name}</span>
      </Group>
    </Combobox.Option>
  ));

  const changeStore = (storeId: number) => {
    var store = stores?.find((s) => s.id === storeId);

    if (store) {
      dispatch(setCurrentSelectedStore(store));
    }
  };

  return (
    <Stack align="stretch" justify="flex-end" gap="xs">
      <Text>Change Store</Text>
      <Combobox
        store={combobox}
        width={250}
        resetSelectionOnOptionHover
        onOptionSubmit={(val) => {
          changeStore(Number(val));
          combobox.closeDropdown();
          combobox.updateSelectedOptionIndex('active');
        }}
      >
        <Combobox.Target>
          <InputBase
            component="button"
            type="button"
            pointer
            rightSection={<Combobox.Chevron />}
            rightSectionPointerEvents="none"
            onClick={() => combobox.toggleDropdown()}
          >
            {currentStore?.name || (
              <Input.Placeholder>Change store</Input.Placeholder>
            )}
          </InputBase>
        </Combobox.Target>

        <Combobox.Dropdown>
          <Combobox.Options>{storeOptions}</Combobox.Options>
        </Combobox.Dropdown>
      </Combobox>
    </Stack>
  );
}
