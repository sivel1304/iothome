import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'


export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    proxy: {
      '/dht11': {
        target: 'http://localhost:5195',
        changeOrigin: true,
      },
    },
  },
})