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
          <Table.Tr>
            <Table.Td>Company Address</Table.Td>
            <Table.Td>{formValues.companyAddress}</Table.Td>
          </Table.Tr>
          <Table.Tr>
            <Table.Td>Website Url</Table.Td>
            <Table.Td>{formValues.websiteUrl}</Table.Td>
          </Table.Tr>
          <Table.Tr>
            <Table.Td>Business Registration Number</Table.Td>
            <Table.Td>{formValues.businessRegistrationNumber}</Table.Td>
          </Table.Tr>
          <Table.Tr>
            <Table.Td>Primary Contact Name</Table.Td>
            <Table.Td>{formValues.primaryContactName}</Table.Td>
          </Table.Tr>
          <Table.Tr>
            <Table.Td>Contact Email</Table.Td>
            <Table.Td>{formValues.contactEmail}</Table.Td>
          </Table.Tr>
          <Table.Tr>
            <Table.Td>Phone Number</Table.Td>
            <Table.Td>{formValues.phoneNumber}</Table.Td>
          </Table.Tr>
          <Table.Tr>
            <Table.Td>Store Name</Table.Td>
            <Table.Td>{formValues.storeName}</Table.Td>
          </Table.Tr>
          <Table.Tr>
            <Table.Td>Store Location</Table.Td>
            <Table.Td>{formValues.storeLocation}</Table.Td>
          </Table.Tr>
        </Table.Tbody>
      </Table>
    </>
  );
}
