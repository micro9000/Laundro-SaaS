'use client';

import React, { forwardRef } from 'react';

import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
} from '@azure/msal-react';
import {
  Avatar,
  Button,
  Group,
  Menu,
  Text,
  UnstyledButton,
  rem,
} from '@mantine/core';
import {
  IconChevronRight,
  IconLogin2,
  IconLogout2,
  IconSettings,
} from '@tabler/icons-react';

import { useAuthorization } from '@/app/infrastructure/auth/AuthorizationContext';
import { msalInstance } from '@/app/infrastructure/auth/authConfig';
import { handleLogin, handleLogout } from '@/app/infrastructure/auth/msal';

// import { getUserPhotoAvatar } from "@/app/infrastructure/auth/msalGraph";
// import { extractInitials } from "@/app/infrastructure/auth/helpers";

interface UserButtonProps extends React.ComponentPropsWithoutRef<'button'> {
  image: string;
  name: string;
  email: string;
  icon?: React.ReactNode;
}

const UserButton = forwardRef<HTMLButtonElement, UserButtonProps>(
  function UserButton(
    { image, name, email, icon, ...others }: UserButtonProps,
    ref
  ) {
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

export default function UserAvatar() {
  const auth = useAuthorization();
  // const [userPhoto, setUserPhoto] = useState<string | null>(null);
  // const [showUserInitials, setShowUserInitials] = useState(false);
  // const [userInitials, setUserInitials] = useState('?');

  const user = msalInstance.getActiveAccount();

  // useEffect(() => {
  // 	if (user) {
  // 		getUserPhotoAvatar().then((response: any) => {
  // 				console.log("getUserPhotoAvatar", response);
  // 				if (response instanceof Blob) {
  // 						const url = URL.createObjectURL(response);
  // 						setUserPhoto(url);
  // 				} else if (typeof response === "string") {
  // 						setUserPhoto(response);
  // 						setShowUserInitials(false);
  // 				} else {
  // 						console.log("Unsupported photo data type.");
  // 				}
  // 		});
  // 		setShowUserInitials(false);
  // 		setUserInitials(extractInitials(user.name));
  // 		console.log(user);
  // 	}
  // }, [user?.username]);

  return (
    <>
      <AuthenticatedTemplate>
        <Menu withArrow>
          <Menu.Target>
            <UserButton
              image="https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-8.png"
              name={auth.userName ?? 'No name'}
              email={auth.userEmail}
            />
          </Menu.Target>

          <Menu.Dropdown>
            <Menu.Label>Application</Menu.Label>
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
      </AuthenticatedTemplate>
      <UnauthenticatedTemplate>
        <Button
          leftSection={<IconLogin2 size={14} />}
          variant="default"
          onClick={() => handleLogin()}
        >
          Sign-in
        </Button>
      </UnauthenticatedTemplate>
    </>
  );
}
