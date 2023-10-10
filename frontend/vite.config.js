import { resolve } from "path"
import { defineConfig } from "vite"
import { cert, key } from "./build/certs"

export default defineConfig({
	css: {
		devSourcemap : true
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
  },
  server: {
    strictPort: true,
    https: {
      cert: cert,
      key: key
    }
  },
})
