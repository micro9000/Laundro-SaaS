'use client';

import {
  ActionIcon,
  Tooltip,
  useComputedColorScheme,
  useMantineColorScheme,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconMoon, IconSun } from '@tabler/icons-react';
import cx from 'clsx';

import classes from './themeToggle.module.css';

const ThemeToggle = () => {
  const [opened, { close, open }] = useDisclosure(false);
  const { setColorScheme } = useMantineColorScheme();
  const computedColorScheme = useComputedColorScheme('light', {
    getInitialValueInEffect: true,
  });

  const capitalizeFirstLetter = (str: string) => {
    return str.charAt(0).toUpperCase() + str.slice(1);
  };

  return (
    <Tooltip
      label={`${capitalizeFirstLetter(computedColorScheme)} mode`}
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
        onClick={() =>
          setColorScheme(computedColorScheme === 'light' ? 'dark' : 'light')
        }
      >
        <IconSun className={cx(classes.icon, classes.light)} stroke={1.5} />
        <IconMoon className={cx(classes.icon, classes.dark)} stroke={1.5} />
      </ActionIcon>
    </Tooltip>
  );
};
export default ThemeToggle;
