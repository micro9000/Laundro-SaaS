'use client';

import React, { useEffect, useState } from 'react';

import { Modal, Space, Text } from '@mantine/core';
import { useMediaQuery } from '@mantine/hooks';

import {
  isCurrentUserIsNewUser,
  selectUserTenantGuid,
} from '@/features/userContext/userContextSlice';
import { useAppSelector } from '@/state/hooks';

import OnboardingForm from './onboarding/onboardingForm';

export default function PortalPage() {
  const isMobile = useMediaQuery('(max-width: 50em)');
  const [isNeedToOnBoardTheUser, setIsNeedToOnBoardTheUser] = useState(false);
  const userTenantGuid = useAppSelector(selectUserTenantGuid);
  const userIsNewUser = useAppSelector(isCurrentUserIsNewUser);

  console.log(userTenantGuid);
  useEffect(() => {
    setIsNeedToOnBoardTheUser(
      (typeof userTenantGuid === 'undefined' || userTenantGuid === null) &&
        userIsNewUser
    );
  }, [userTenantGuid, userIsNewUser]);

  return (
    <>
      <Modal.Root
        opened={isNeedToOnBoardTheUser}
        onClose={() => {}}
        size="lg"
        fullScreen={isMobile}
        transitionProps={{ transition: 'fade', duration: 200 }}
      >
        <Modal.Overlay backgroundOpacity={0.55} blur={3} />
        <Modal.Content>
          <Modal.Header>
            <Modal.Title>Onboarding</Modal.Title>
            <Modal.CloseButton />
          </Modal.Header>
          <Modal.Body>
            <Text size="sm">
              To get started, you will need to create a tenant. This will set up
              your workspace and allow you to access all the features. Simply
              follow the prompts to create your tenant, and you will be on your
              way to using the system! <Space h="sm" /> If you have any
              questions, feel free to reach out for assistance.
            </Text>
            <Space h="xl" />
            <OnboardingForm />
          </Modal.Body>
        </Modal.Content>
      </Modal.Root>

      <h1>Portal Home page</h1>
    </>
  );
}
