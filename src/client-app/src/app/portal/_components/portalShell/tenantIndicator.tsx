import { Chip, Text } from '@mantine/core';

import { selectUserTenantName } from '@/features/userContext/userContextSlice';
import { useAppSelector } from '@/state/hooks';

export function TenantIndicator() {
  const tenantName = useAppSelector(selectUserTenantName);

  return (
    <>
      <Text>Laundro | {tenantName} | Portal</Text>
    </>
  );
}
