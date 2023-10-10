import { resolve } from "path"
import { defineConfig } from "vite"
import { getAspDotNetCertificate } from './build/certs';

export default defineConfig(async ({ mode }) => {

  console.log(`Configuring Vite for ${mode} mode.`)

  const config = {
    css: {
      devSourcemap: true
    },
    build: {
      manifest: true,
      cssMinify: false,
      outDir: '../wwwroot/frontend',
      emptyOutDir: true,
      rollupOptions: {
        input: {
          app: resolve(__dirname, 'app.js')
        }
      }
    }
  };

  if (mode === 'development') {
    // only get the certificate if we're in development mode
    const cert = getAspDotNetCertificate();

    config.server = {
      strictPort: true,
      hmr: {
        clientPort: 5173
      },
      https: {
        cert: cert.certificate,
        key: cert.privateKey
      }
    };
  }
  return config;
})
