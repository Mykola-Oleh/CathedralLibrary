const { test, expect } = require('@playwright/test');

test('home page loads', async ({ page }) => {
  await page.goto('/');
  await expect(page).toHaveTitle(/Кафедральна бібліотека/);
});

test('publications page is available', async ({ page }) => {
  await page.goto('/Publications');
  await expect(page.locator('body')).toContainText('Видання');
});

test('unauthenticated create redirects to Google login', async ({ page }) => {
    await page.goto('/Authors/Create');

    await expect(page).toHaveURL(
        /accounts\.google\.com/i,
        { timeout: 15000 }
    );
});

test('unauthenticated API mutation is denied', async ({ request }) => {
  const response = await request.post('/api/authors', { data: { fullName: 'Test Author' }, maxRedirects: 0 });
  expect([401, 302]).toContain(response.status());
});
