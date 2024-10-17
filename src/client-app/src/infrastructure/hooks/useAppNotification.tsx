import React from 'react';

import { DefaultMantineColor } from '@mantine/core';
import { showNotification } from '@mantine/notifications';
import { IconCheck, IconExclamationMark, IconX } from '@tabler/icons-react';

const useAppNotification = () => {
  const notify = (
    message: string | React.ReactNode,
    color: DefaultMantineColor,
    icon?: React.ReactNode,
    title?: string
  ) => {
    const config = {
      autoClose: 10000,
      color: color,
      icon: icon,
      title: title,
      message: message,
    };

    showNotification(config);
  };

  const notifyError = (message: string, details: string) => {
    notify(details, 'red', <IconX />, message);
  };

  const notifyInfo = (message: string | React.ReactNode) => {
    notify(message, 'blue');
  };

  const notifySuccess = (message: string | React.ReactNode) => {
    notify(message, 'green', <IconCheck />);
  };

  const notifyWarning = (message: string | React.ReactNode) => {
    notify(message, 'orange', <IconExclamationMark />);
  };

  return {
    notifyError,
    notifyInfo,
    notifySuccess,
    notifyWarning,
  };
};

export default useAppNotification;
