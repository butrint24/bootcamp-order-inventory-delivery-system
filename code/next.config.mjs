/** @type {import('next').NextConfig} */
const nextConfig = {
  typescript: {
    ignoreBuildErrors: true,
  },
  images: {
    unoptimized: true,
  },
  async redirects() {
    return [
      // Kap gabimin e shkrimit dhe dërgo te /dashboard
      { source: "/dashbaord", destination: "/dashboard", permanent: false },
      { source: "/Dashbaord", destination: "/dashboard", permanent: false }, // (opsionale) variant me shkronjë të madhe
    ];
  },
};

export default nextConfig;
