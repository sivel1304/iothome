import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'


export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    proxy: {
      '/modules': { target: 'http://localhost:5195', changeOrigin: true },
      '/latest-reading': { target: 'http://localhost:5195', changeOrigin: true },
      '/hubs': { target: 'http://localhost:5195', changeOrigin: true, ws: true },
    },
  },
})