import React, { forwardRef, useEffect, useState } from 'react';

import { Avatar, Group, Menu, Text, UnstyledButton, rem } from '@mantine/core';
import {
  IconChevronRight,
  IconLogout2,
  IconSettings,
} from '@tabler/icons-react';

import { useAuthorization } from '@/infrastructure/auth/authorization-context';
import { handleLogout } from '@/infrastructure/auth/msal';

// import { msalInstance } from '@/infrastructure/auth/auth-config';
// import { extractInitials } from '@/infrastructure/auth/helpers';
// import { getUserPhotoAvatar } from '@/infrastructure/auth/msalGraph';

interface UserAvatarProps extends React.ComponentPropsWithoutRef<'button'> {
  image: string;
  name: string;
  email: string;
  icon?: React.ReactNode;
}

const UserAvatar = forwardRef<HTMLButtonElement, UserAvatarProps>(
  ({ image, name, email, icon, ...others }: UserAvatarProps, ref) => {
    return (
      <UnstyledButton
        ref={ref}
        style={{
          padding: 'var(--mantine-spacing-md)',
          color: 'var(--mantine-color-text)',
          borderRadius: 'var(--mantine-radius-sm)',
        }}
        {...others}
      >
        <Group>
          <Avatar src={image} radius="xl" />
          <div style={{ flex: 1 }}>
            <Text size="sm" fw={500}>
              {name}
            </Text>
            <Text c="dimmed" size="xs">
              {email}
            </Text>
          </div>
          {icon || <IconChevronRight size="1rem" />}
        </Group>
      </UnstyledButton>
    );
  }
);
UserAvatar.displayName = 'UserButton';

export function LogoutButton() {
  const auth = useAuthorization();

  // const [userPhoto, setUserPhoto] = useState<string | null>(null);
  // const [showUserInitials, setShowUserInitials] = useState(false);
  // const [userInitials, setUserInitials] = useState('?');
  // const user = msalInstance.getActiveAccount();

  // useEffect(() => {
  //   if (user) {
  //     getUserPhotoAvatar().then((response: any) => {
  //       console.log('getUserPhotoAvatar', response);
  //       if (response instanceof Blob) {
  //         const url = URL.createObjectURL(response);
  //         setUserPhoto(url);
  //       } else if (typeof response === 'string') {
  //         setUserPhoto(response);
  //         setShowUserInitials(false);
  //       } else {
  //         console.log('Unsupported photo data type.');
  //       }
  //     });
  //     setShowUserInitials(false);
  //     setUserInitials(extractInitials(user.name));
  //     console.log(user);
  //   }
  // }, [user?.username]);

  return (
    <Menu withArrow trigger="hover" openDelay={100} closeDelay={400}>
      <Menu.Target>
        <UserAvatar
          image="https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-8.png"
          name={auth.userName ?? 'No name'}
          email={auth.userEmail}
        />
      </Menu.Target>

      <Menu.Dropdown>
        <Menu.Item
          leftSection={
            <IconSettings style={{ width: rem(14), height: rem(14) }} />
          }
        >
          Settings
        </Menu.Item>

        <Menu.Item
          leftSection={
            <IconLogout2 style={{ width: rem(14), height: rem(14) }} />
          }
          onClick={() => handleLogout()}
        >
          Sign-out
        </Menu.Item>
      </Menu.Dropdown>
    </Menu>
  );
}
