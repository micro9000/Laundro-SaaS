import { useEffect } from 'react';

import { Alert, Divider, Table, Text } from '@mantine/core';
import { FormErrors, UseFormReturnType } from '@mantine/form';
import { IconInfoCircle } from '@tabler/icons-react';
import { isEmpty } from 'lodash';

import { OnboardingFormValues } from './onboardingFormValues';

interface VerifyDetailsProps {
  formValues: OnboardingFormValues;
  formErrors: FormErrors;
}

export default function VerifyDetails({
  formValues,
  formErrors,
}: VerifyDetailsProps) {
  return (
    <>
      {!isEmpty(formErrors) && (
        <Alert
          variant="light"
          color="red"
          title="Validation errors"
          icon={<IconInfoCircle />}
        >
          {Object.keys(formErrors).map((key) => (
            <Text key={key}>{formErrors[key]}</Text>
          ))}
        </Alert>
      )}
      <h3>Tenant Info</h3>

      <Table>
        <Table.Tbody>
          <Table.Tr>
            <Table.Td>Tenant Name</Table.Td>
            <Table.Td>{formValues.tenantName}</Table.Td>
          </Table.Tr>
        </Table.Tbody>
      </Table>

      <Divider my="md" />
      <h3>Store Info</h3>
    </>
  );
}
