import React, { useEffect } from 'react';

interface TenantHomeProps {
  params: {
    tenantGuid: string;
  };
}

export default function Page({ params: { tenantGuid } }: TenantHomeProps) {
  return (
    <>
      <h1>{tenantGuid}</h1>
    </>
  );
}
