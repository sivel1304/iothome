import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/dht11': {
        target: 'http://localhost:5195',
        changeOrigin: true,
      },
    },
  },
})