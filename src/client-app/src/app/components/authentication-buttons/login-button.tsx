import React from 'react';

import { Button } from '@mantine/core';
import { IconLogin2 } from '@tabler/icons-react';

import { handleLogin } from '@/infrastructure/auth/msal';

export function LoginButton() {
  return (
    <Button
      leftSection={<IconLogin2 size={14} />}
      variant="default"
      onClick={() => handleLogin()}
    >
      Sign-in
    </Button>
  );
}
