import { useEffect, useState } from 'react';

import { Button, Group, Modal, Space, Stepper, Text } from '@mantine/core';
import { useForm } from '@mantine/form';
import { useMediaQuery } from '@mantine/hooks';
import { current } from '@reduxjs/toolkit';

import { useAppMutation, useAppNotification } from '@/infrastructure/hooks';
import { Tenant } from '@/models/tenant';
import { nameof } from '@/utilities';

import { OnboardingFormValues } from './onboardingFormValues';
import StoreDetailsInputs from './storeDetailsInputs';
import TenantDetailsInputs from './tenantDetailsInputs';
import VerifyDetails from './verifyDetails';

interface OnboardingFormIndexProps {
  isNeedToOnBoardTheUser: boolean;
}

export default function OnboardingFormIndex({
  isNeedToOnBoardTheUser,
}: OnboardingFormIndexProps) {
  const isMobile = useMediaQuery('(max-width: 50em)');
  const [activeForm, setActiveForm] = useState(0);

  var notification = useAppNotification();
  const { mutate, isError, isSuccess, error, isPending, data } =
    useAppMutation<{
      tenant: Tenant;
    }>({
      path: '/tenant/create',
      mutationKey: 'tenant/create',
    });

  console.log(data);
  console.log(error);
  useEffect(() => {
    if (isError && error) {
      console.log(error.errors);
      notification.notifyError(error.statuCode.toString(), error.message);
    }
  }, [isError, error, notification]);

  useEffect(() => {
    if (isSuccess) {
      notification.notifySuccess('Successfully submit onboarding form');

      setTimeout(() => {
        location.reload();
      }, 500);
    }
  }, [isSuccess, notification]);

  const form = useForm<OnboardingFormValues>({
    mode: 'uncontrolled',
    initialValues: {
      tenantName: '',
      companyAddress: '',
      websiteUrl: undefined,
      businessRegistrationNumber: undefined,
      primaryContactName: '',
      contactEmail: '',
      phoneNumber: '',
      storeName: '',
      storeLocation: '',
    },

    validate: {
      tenantName: (value) => (value ? null : 'Tenant name is required'),
      companyAddress: (value) => (value ? null : 'Company address is required'),
      primaryContactName: (value) =>
        value ? null : 'Primary Contact Name is required',
      contactEmail: (value) =>
        /^\S+@\S+$/.test(value) ? null : 'Invalid email',
      phoneNumber: (value) => (value ? null : 'Phone Number is required'),
      storeName: (value) => (value ? null : 'Store Name is required'),
      storeLocation: (value) => (value ? null : 'Store Location is required'),
    },
  });

  const onFormSubmit = (values: OnboardingFormValues) => {
    var formData = new FormData();
    formData.append(
      nameof<OnboardingFormValues>('tenantName'),
      values.tenantName
    );
    formData.append(
      nameof<OnboardingFormValues>('companyAddress'),
      values.companyAddress
    );
    formData.append(
      nameof<OnboardingFormValues>('websiteUrl'),
      values.websiteUrl ?? ''
    );
    formData.append(
      nameof<OnboardingFormValues>('businessRegistrationNumber'),
      values.businessRegistrationNumber ?? ''
    );

    formData.append(
      nameof<OnboardingFormValues>('primaryContactName'),
      values.primaryContactName
    );
    formData.append(
      nameof<OnboardingFormValues>('contactEmail'),
      values.contactEmail
    );
    formData.append(
      nameof<OnboardingFormValues>('phoneNumber'),
      values.phoneNumber
    );
    formData.append(
      nameof<OnboardingFormValues>('storeName'),
      values.storeName
    );
    formData.append(
      nameof<OnboardingFormValues>('storeLocation'),
      values.storeLocation
    );
    mutate(formData);
  };
  const numberOfOnboardingFormTabs = 3;
  const nextStep = () => {
    if (activeForm === numberOfOnboardingFormTabs - 1) {
      // manually validate the form on the last form (Before verification)
      form.validate();
    }

    setActiveForm((current) =>
      current < numberOfOnboardingFormTabs ? current + 1 : current
    );
  };

  const prevStep = () =>
    setActiveForm((current) => (current > 0 ? current - 1 : current));

  return (
    <>
      <Modal.Root
        opened={isNeedToOnBoardTheUser}
        onClose={() => {}}
        size="xl"
        fullScreen={isMobile}
        transitionProps={{ transition: 'fade', duration: 200 }}
      >
        <Modal.Overlay backgroundOpacity={0.55} blur={3} />
        <Modal.Content>
          <Modal.Header>
            <Modal.Title>
              <h3>Onboarding</h3>
            </Modal.Title>
            <Modal.CloseButton />
          </Modal.Header>
          <Modal.Body>
            <form onSubmit={form.onSubmit(onFormSubmit)}>
              <Stepper
                iconSize={28}
                active={activeForm}
                onStepClick={setActiveForm}
              >
                <Stepper.Step label="Welcome" description="Introduction">
                  <Text size="sm">
                    To get started, you will need to create a tenant. This will
                    set up your workspace and allow you to access all the
                    features. Simply follow the prompts to create your tenant,
                    and you will be on your way to using the system!{' '}
                    <Space h="sm" /> If you have any questions, feel free to
                    reach out for assistance.
                  </Text>
                </Stepper.Step>
                <Stepper.Step
                  label="Tenant Information"
                  description="Enter company info"
                >
                  <TenantDetailsInputs form={form} />
                </Stepper.Step>
                <Stepper.Step
                  label="Initial Store"
                  description="Enter your first store"
                >
                  <StoreDetailsInputs form={form} />
                </Stepper.Step>
                <Stepper.Completed>
                  <VerifyDetails
                    formValues={form.getValues()}
                    formErrors={form.errors}
                  />

                  <Group justify="center" mt="md">
                    <Button variant="default" onClick={prevStep}>
                      Back
                    </Button>
                    <Button type="submit" disabled={isPending}>
                      Submit
                    </Button>
                  </Group>
                </Stepper.Completed>
              </Stepper>

              {activeForm < numberOfOnboardingFormTabs && (
                <Group justify="center" mt="xl">
                  <Button variant="default" onClick={prevStep}>
                    Back
                  </Button>
                  <Button onClick={nextStep}>Next step</Button>
                </Group>
              )}
            </form>
          </Modal.Body>
        </Modal.Content>
      </Modal.Root>
    </>
  );
}
