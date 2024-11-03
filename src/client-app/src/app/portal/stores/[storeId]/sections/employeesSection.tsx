import { useEffect, useState } from 'react';

import {
  Button,
  Container,
  Drawer,
  Group,
  Space,
  Table,
  Text,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconPlus, IconTrashX } from '@tabler/icons-react';

import { Store, StoreUser, User } from '@/models';

import AssignNewEmployeeToStoreForm from './_components/AssignNewEmployeeToStoreForm';

export default function EmployeesSection({ store }: { store?: Store | null }) {
  const [employees, setEmployees] = useState<StoreUser[]>([]);

  const [opened, { open, close }] = useDisclosure(false);

  useEffect(() => {
    if (store && store?.storeUser && store?.storeUser?.length > 0) {
      setEmployees(store?.storeUser);
    }
  }, [store, store?.storeUser]);

  const rows = employees?.map((emp) => (
    <Table.Tr key={emp.userId}>
      <Table.Td>{emp.user?.email}</Table.Td>
      <Table.Td>{emp.user?.name}</Table.Td>
      <Table.Td>{emp.role?.name}</Table.Td>
      <Table.Td>
        <Button leftSection={<IconTrashX size={14} />} variant="subtle">
          Unassign
        </Button>
      </Table.Td>
    </Table.Tr>
  ));

  return (
    <>
      <Container size="xl">
        <Drawer
          offset={4}
          // radius="md"
          size="lg"
          opened={opened}
          onClose={close}
          title="Assign New Employee"
          position="right"
        >
          <AssignNewEmployeeToStoreForm store={store!} onSuccess={close} />
        </Drawer>
        <Text fw={700}>Employees</Text>
        <Space h="lg" />
        <Group justify="right">
          <Button
            leftSection={<IconPlus size={14} />}
            variant="subtle"
            onClick={open}
          >
            Assign New Employee On This Store
          </Button>
        </Group>
        <Space h="md" />

        <Table>
          <Table.Thead>
            <Table.Tr>
              <Table.Th>Email</Table.Th>
              <Table.Th>Name</Table.Th>
              <Table.Th>Role on this store</Table.Th>
              <Table.Th>Actions</Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>{rows}</Table.Tbody>
        </Table>
      </Container>
    </>
  );
}
