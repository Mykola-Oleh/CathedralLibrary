const { defineConfig } = require('@playwright/test');
module.exports = defineConfig({
  use: { baseURL: process.env.BASE_URL || 'https://localhost:58103', ignoreHTTPSErrors: true },
  reporter: 'list'
});
