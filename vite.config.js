import { defineConfig } from 'vite';
import { resolve } from 'path';

export default defineConfig({
  build: {
    manifest: true,
    outDir: 'wwwroot/frontend',
    rollupOptions: {
      input: {
        app: resolve(__dirname, 'app.js')
      },
      output: {
        entryFileNames: 'assets/[name]-[hash].js',
        chunkFileNames: 'assets/[name]-[hash].js',
        assetFileNames: 'assets/[name]-[hash].[ext]'
      }
    }
  },
  server: {
    port: 5173,
    strictPort: true,
    hmr: {
      protocol: 'ws',
      host: 'localhost'
    }
  }
});