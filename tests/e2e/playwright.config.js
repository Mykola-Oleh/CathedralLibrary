const { defineConfig } = require('@playwright/test');
module.exports = defineConfig({
  use: { baseURL: process.env.BASE_URL || 'https://localhost:7123', ignoreHTTPSErrors: true },
  reporter: 'list'
});
