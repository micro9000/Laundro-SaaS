/** @type {import('next').NextConfig} */
const nextConfig = {
	experimental: {
    optimizePackageImports: ['@mantine/core', '@mantine/hooks'],
    typedRoutes: true,
  },
};

export default nextConfig;
