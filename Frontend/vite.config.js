import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'http://www.newtlike.com:3000',
        changeOrigin: true,
        secure: false,
      }
    }
  }
})
/* export default defineConfig({
  plugins: [
    react({
      babel: {
        plugins: [['babel-plugin-react-compiler']],
      },
    }),
  ],
  build: {
    outDir: 'build'
  },
  server: {
    host: '0.0.0.0',
    port: 80
  },
  preview: {
    host: '0.0.0.0',
    port: 80
  }
}) */