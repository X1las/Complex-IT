import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
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
})
