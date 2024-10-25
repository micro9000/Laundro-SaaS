'use client';

import { useEffect, useState } from 'react';

import { ActionIcon, Tooltip, useMantineColorScheme } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconMoon, IconSun } from '@tabler/icons-react';

export const ThemeToggle = () => {
  const [isDarkMode, setIsDarkMode] = useState(false);
  const [opened, { close, open }] = useDisclosure(false);
  const { colorScheme, setColorScheme } = useMantineColorScheme();

  useEffect(() => {
    setIsDarkMode(colorScheme === 'dark');
  }, [colorScheme]);

  const toggleColorTheme = () => {
    if (colorScheme === 'dark') setColorScheme('light');
    else setColorScheme('dark');
  };

  const capitalizeFirstLetter = (str: string) => {
    return str.charAt(0).toUpperCase() + str.slice(1);
  };

  return (
    <Tooltip
      label={`${capitalizeFirstLetter(colorScheme)} mode`}
      opened={opened}
    >
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
    </Tooltip>
  );
};
