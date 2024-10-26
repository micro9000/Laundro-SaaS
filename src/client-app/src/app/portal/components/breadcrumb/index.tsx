import Link from 'next/link';
import { usePathname } from 'next/navigation';

import { Anchor, Breadcrumbs, Space } from '@mantine/core';

interface AnchorItem {
  title: string;
  href: string;
}

export default function PortalBreadcrumb() {
  const pathname = usePathname();
  var pathNames = pathname.split('/').filter((path) => path);

  var paths = pathNames.map((link, index) => {
    let href = `/${pathNames.slice(0, index + 1).join('/')}`;

    return {
      title: link,
      href,
    } as AnchorItem;
  });

  // https://nextjs.org/docs/app/api-reference/components/link#href-required
  const items = paths.map((item, index) => (
    <Anchor
      underline="hover"
      component={Link}
      href={{
        pathname: item.href,
      }}
      key={index}
    >
      {item.title}
    </Anchor>
  ));

  return (
    <>
      <Breadcrumbs>{items}</Breadcrumbs>
      <Space h="lg" />
    </>
  );
}
