import {
  ActionIcon,
  Popover,
  Text,
  useMantineColorScheme,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconMoon, IconSun } from '@tabler/icons-react';

export const ThemeToggle = () => {
  const [opened, { close, open }] = useDisclosure(false);
  const { colorScheme, setColorScheme, clearColorScheme } =
    useMantineColorScheme();

  const toggleColorTheme = () => {
    if (colorScheme === 'dark') setColorScheme('light');
    else setColorScheme('dark');
  };

  const capitalizeFirstLetter = (str: string) => {
    return str.charAt(0).toUpperCase() + str.slice(1);
  };

  const isDarkMode = colorScheme === 'dark';

  return (
    <Popover
      width={200}
      position="bottom"
      withArrow
      shadow="md"
      opened={opened}
    >
      <Popover.Target>
        <ActionIcon
          variant="default"
          color="teal"
          size="lg"
          radius="md"
          aria-label="Settings"
          onMouseEnter={open}
          onMouseLeave={close}
          onClick={toggleColorTheme}
        >
          {isDarkMode ? (
            <IconSun style={{ width: '70%', height: '70%' }} stroke={1.5} />
          ) : (
            <IconMoon style={{ width: '70%', height: '70%' }} stroke={1.5} />
          )}
        </ActionIcon>
      </Popover.Target>
      <Popover.Dropdown style={{ pointerEvents: 'none' }}>
        <Text size="sm">{capitalizeFirstLetter(colorScheme)} mode</Text>
      </Popover.Dropdown>
    </Popover>
  );
};
