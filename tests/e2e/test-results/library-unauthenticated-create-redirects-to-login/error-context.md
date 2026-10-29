# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: library.spec.js >> unauthenticated create redirects to login
- Location: library.spec.js:13:1

# Error details

```
Error: expect(page).toHaveURL(expected) failed

Expected pattern: /account\/login/i
Received string:  "https://accounts.google.com/v3/signin/identifier?opparams=%253F&dsh=S-2059851122%3A1793296950559530&client_id=1005244889474-n0223kppp1mrf9840322p0st3rf5i8dl.apps.googleusercontent.com&code_challenge=v-13X3Xa_bW7edxZu3LLBvT-CKksy-ozmfIJvGfecf8&code_challenge_method=S256&o2v=2&redirect_uri=https%3A%2F%2Flocalhost%3A58103%2Fsignin-google&response_type=code&scope=openid+profile+email&service=lso&state=CfDJ8O-3dwhZkDtEqkpG7_JNHx779kfBCN_D98qgUUKEbp4OiRICsI9a8N7nANEVGAUvt9eaevc8EdMjbdhLLVGtG6Aiagrv9yOPb2y7so6BENCbgCheM4q_8PvkxaKA1hlEthmEDp2c-I6TbyU5zBXdkh-Jp-UbI31sPWz-ZZQWB2ul--_GeZAT8hYeXejFZ4TAl7PxBu6g6YgtChexXu-r6rWaZacIwVS6itiCz9WCzN3DzP9OtDb_CU_45GQUDV14pxEwClBuD84Jqj0eCoeEcGkYkK6Vg3YUkZuQUA5JpixU&flowName=GeneralOAuthLite&continue=https%3A%2F%2Faccounts.google.com%2Fsignin%2Foauth%2Flegacy%2Fconsent%3Fauthuser%3Dunknown%26part%3DAJi8hAOpk7Q-fmYrKIzKjxABG4sXR92zb1kZQZIpcMDZPCebU-o6s-Y2hMz056wy_fOD41_ct2nATqX_nLUA6-anljWGPHkT8LHj7eXuYEJFd08C-WivhpxQgXRX2WTMGPFAmxwbVFzPXM28WMPOOwP67jaikfit5ipIGJZM4ahwhYai_ha6M_4-Ftr1B1uvc_U6vSGk5zJ0aw0ON_c8LCUKfSVtjfJk6NuDXj4MBosj9O8N983qnMSblOasiN5cYHBsmBkwD7YbcNFgzOBKD6HQc26lrCrqzsC38ONwsleQePTSYwgDWfKBNNR8bB-uCnyKN8onCJIrWk0IwxHqo2Y3ql6ZkS0r_t6OgG0i29QhXyckP3yG3xVSAG-dGEvPy5XzsZgAg44lm7r8sFh9gBlDZ9id3G216dTeIKiLJVAoZVdrC2bt7e7SGTwmEUeZBAYFUzhoDwRDQgiVPje-uy7d6u0q0x0kog%26flowName%3DGeneralOAuthFlow%26as%3DS-2059851122%253A1793296950559530%26client_id%3D1005244889474-n0223kppp1mrf9840322p0st3rf5i8dl.apps.googleusercontent.com%23&app_domain=https%3A%2F%2Flocalhost%3A58103&rart=ANgoxcfB4wB6RuZ43fx6NPCzJTLPc_trMt0n4yzlssUiJcwRYPHXQvESUmLLzy2P3KlhnCkHUEZGsZB696TM_xUl40oWjkN4LwrLirnZfLaMmrAHjC9hpck"
Timeout: 5000ms

Call log:
  - Expect "toHaveURL" with timeout 5000ms
    13 × locator resolved to <html dir="ltr" lang="en-US">…</html>
       - unexpected value "https://accounts.google.com/v3/signin/identifier?opparams=%253F&dsh=S-2059851122%3A1793296950559530&client_id=1005244889474-n0223kppp1mrf9840322p0st3rf5i8dl.apps.googleusercontent.com&code_challenge=v-13X3Xa_bW7edxZu3LLBvT-CKksy-ozmfIJvGfecf8&code_challenge_method=S256&o2v=2&redirect_uri=https%3A%2F%2Flocalhost%3A58103%2Fsignin-google&response_type=code&scope=openid+profile+email&service=lso&state=CfDJ8O-3dwhZkDtEqkpG7_JNHx779kfBCN_D98qgUUKEbp4OiRICsI9a8N7nANEVGAUvt9eaevc8EdMjbdhLLVGtG6Aiagrv9yOPb2y7so6BENCbgCheM4q_8PvkxaKA1hlEthmEDp2c-I6TbyU5zBXdkh-Jp-UbI31sPWz-ZZQWB2ul--_GeZAT8hYeXejFZ4TAl7PxBu6g6YgtChexXu-r6rWaZacIwVS6itiCz9WCzN3DzP9OtDb_CU_45GQUDV14pxEwClBuD84Jqj0eCoeEcGkYkK6Vg3YUkZuQUA5JpixU&flowName=GeneralOAuthLite&continue=https%3A%2F%2Faccounts.google.com%2Fsignin%2Foauth%2Flegacy%2Fconsent%3Fauthuser%3Dunknown%26part%3DAJi8hAOpk7Q-fmYrKIzKjxABG4sXR92zb1kZQZIpcMDZPCebU-o6s-Y2hMz056wy_fOD41_ct2nATqX_nLUA6-anljWGPHkT8LHj7eXuYEJFd08C-WivhpxQgXRX2WTMGPFAmxwbVFzPXM28WMPOOwP67jaikfit5ipIGJZM4ahwhYai_ha6M_4-Ftr1B1uvc_U6vSGk5zJ0aw0ON_c8LCUKfSVtjfJk6NuDXj4MBosj9O8N983qnMSblOasiN5cYHBsmBkwD7YbcNFgzOBKD6HQc26lrCrqzsC38ONwsleQePTSYwgDWfKBNNR8bB-uCnyKN8onCJIrWk0IwxHqo2Y3ql6ZkS0r_t6OgG0i29QhXyckP3yG3xVSAG-dGEvPy5XzsZgAg44lm7r8sFh9gBlDZ9id3G216dTeIKiLJVAoZVdrC2bt7e7SGTwmEUeZBAYFUzhoDwRDQgiVPje-uy7d6u0q0x0kog%26flowName%3DGeneralOAuthFlow%26as%3DS-2059851122%253A1793296950559530%26client_id%3D1005244889474-n0223kppp1mrf9840322p0st3rf5i8dl.apps.googleusercontent.com%23&app_domain=https%3A%2F%2Flocalhost%3A58103&rart=ANgoxcfB4wB6RuZ43fx6NPCzJTLPc_trMt0n4yzlssUiJcwRYPHXQvESUmLLzy2P3KlhnCkHUEZGsZB696TM_xUl40oWjkN4LwrLirnZfLaMmrAHjC9hpck"

```

```yaml
- img "Google"
- text: Sign in with Google
- heading "Sign in" [level=1]
- paragraph:
  - text: to continue to
  - button "CathedralLibrary"
- textbox "Email or phone"
- text: Email or phone
- paragraph:
  - link "Forgot email?":
    - /url: /signin/v2/usernamerecovery?app_domain=https://localhost:58103&client_id=1005244889474-n0223kppp1mrf9840322p0st3rf5i8dl.apps.googleusercontent.com&code_challenge=v-13X3Xa_bW7edxZu3LLBvT-CKksy-ozmfIJvGfecf8&code_challenge_method=S256&continue=https://accounts.google.com/signin/oauth/legacy/consent?authuser%3Dunknown%26part%3DAJi8hAOpk7Q-fmYrKIzKjxABG4sXR92zb1kZQZIpcMDZPCebU-o6s-Y2hMz056wy_fOD41_ct2nATqX_nLUA6-anljWGPHkT8LHj7eXuYEJFd08C-WivhpxQgXRX2WTMGPFAmxwbVFzPXM28WMPOOwP67jaikfit5ipIGJZM4ahwhYai_ha6M_4-Ftr1B1uvc_U6vSGk5zJ0aw0ON_c8LCUKfSVtjfJk6NuDXj4MBosj9O8N983qnMSblOasiN5cYHBsmBkwD7YbcNFgzOBKD6HQc26lrCrqzsC38ONwsleQePTSYwgDWfKBNNR8bB-uCnyKN8onCJIrWk0IwxHqo2Y3ql6ZkS0r_t6OgG0i29QhXyckP3yG3xVSAG-dGEvPy5XzsZgAg44lm7r8sFh9gBlDZ9id3G216dTeIKiLJVAoZVdrC2bt7e7SGTwmEUeZBAYFUzhoDwRDQgiVPje-uy7d6u0q0x0kog%26flowName%3DGeneralOAuthFlow%26as%3DS-2059851122%253A1793296950559530%26client_id%3D1005244889474-n0223kppp1mrf9840322p0st3rf5i8dl.apps.googleusercontent.com%23&dsh=S-2059851122:1793296950559530&flowName=GeneralOAuthLite&o2v=2&opparams=%253F&rart=ANgoxcfB4wB6RuZ43fx6NPCzJTLPc_trMt0n4yzlssUiJcwRYPHXQvESUmLLzy2P3KlhnCkHUEZGsZB696TM_xUl40oWjkN4LwrLirnZfLaMmrAHjC9hpck&redirect_uri=https://localhost:58103/signin-google&response_type=code&scope=openid+profile+email&service=lso&state=CfDJ8O-3dwhZkDtEqkpG7_JNHx779kfBCN_D98qgUUKEbp4OiRICsI9a8N7nANEVGAUvt9eaevc8EdMjbdhLLVGtG6Aiagrv9yOPb2y7so6BENCbgCheM4q_8PvkxaKA1hlEthmEDp2c-I6TbyU5zBXdkh-Jp-UbI31sPWz-ZZQWB2ul--_GeZAT8hYeXejFZ4TAl7PxBu6g6YgtChexXu-r6rWaZacIwVS6itiCz9WCzN3DzP9OtDb_CU_45GQUDV14pxEwClBuD84Jqj0eCoeEcGkYkK6Vg3YUkZuQUA5JpixU
- button "Next"
- link "Create account":
  - /url: /lifecycle/flows/signup?app_domain=https://localhost:58103&client_id=1005244889474-n0223kppp1mrf9840322p0st3rf5i8dl.apps.googleusercontent.com&code_challenge=v-13X3Xa_bW7edxZu3LLBvT-CKksy-ozmfIJvGfecf8&code_challenge_method=S256&continue=https://accounts.google.com/signin/oauth/legacy/consent?authuser%3Dunknown%26part%3DAJi8hAOpk7Q-fmYrKIzKjxABG4sXR92zb1kZQZIpcMDZPCebU-o6s-Y2hMz056wy_fOD41_ct2nATqX_nLUA6-anljWGPHkT8LHj7eXuYEJFd08C-WivhpxQgXRX2WTMGPFAmxwbVFzPXM28WMPOOwP67jaikfit5ipIGJZM4ahwhYai_ha6M_4-Ftr1B1uvc_U6vSGk5zJ0aw0ON_c8LCUKfSVtjfJk6NuDXj4MBosj9O8N983qnMSblOasiN5cYHBsmBkwD7YbcNFgzOBKD6HQc26lrCrqzsC38ONwsleQePTSYwgDWfKBNNR8bB-uCnyKN8onCJIrWk0IwxHqo2Y3ql6ZkS0r_t6OgG0i29QhXyckP3yG3xVSAG-dGEvPy5XzsZgAg44lm7r8sFh9gBlDZ9id3G216dTeIKiLJVAoZVdrC2bt7e7SGTwmEUeZBAYFUzhoDwRDQgiVPje-uy7d6u0q0x0kog%26flowName%3DGeneralOAuthFlow%26as%3DS-2059851122%253A1793296950559530%26client_id%3D1005244889474-n0223kppp1mrf9840322p0st3rf5i8dl.apps.googleusercontent.com%23&dsh=S-2059851122:1793296950559530&flowEntry=SignUp&flowName=GlifWebSignIn&o2v=2&opparams=%253F&rart=ANgoxcfB4wB6RuZ43fx6NPCzJTLPc_trMt0n4yzlssUiJcwRYPHXQvESUmLLzy2P3KlhnCkHUEZGsZB696TM_xUl40oWjkN4LwrLirnZfLaMmrAHjC9hpck&redirect_uri=https://localhost:58103/signin-google&response_type=code&scope=openid+profile+email&service=lso&signInUrl=https://accounts.google.com/signin/oauth?app_domain%3Dhttps://localhost:58103%26client_id%3D1005244889474-n0223kppp1mrf9840322p0st3rf5i8dl.apps.googleusercontent.com%26code_challenge%3Dv-13X3Xa_bW7edxZu3LLBvT-CKksy-ozmfIJvGfecf8%26code_challenge_method%3DS256%26continue%3Dhttps://accounts.google.com/signin/oauth/legacy/consent?authuser%253Dunknown%2526part%253DAJi8hAOpk7Q-fmYrKIzKjxABG4sXR92zb1kZQZIpcMDZPCebU-o6s-Y2hMz056wy_fOD41_ct2nATqX_nLUA6-anljWGPHkT8LHj7eXuYEJFd08C-WivhpxQgXRX2WTMGPFAmxwbVFzPXM28WMPOOwP67jaikfit5ipIGJZM4ahwhYai_ha6M_4-Ftr1B1uvc_U6vSGk5zJ0aw0ON_c8LCUKfSVtjfJk6NuDXj4MBosj9O8N983qnMSblOasiN5cYHBsmBkwD7YbcNFgzOBKD6HQc26lrCrqzsC38ONwsleQePTSYwgDWfKBNNR8bB-uCnyKN8onCJIrWk0IwxHqo2Y3ql6ZkS0r_t6OgG0i29QhXyckP3yG3xVSAG-dGEvPy5XzsZgAg44lm7r8sFh9gBlDZ9id3G216dTeIKiLJVAoZVdrC2bt7e7SGTwmEUeZBAYFUzhoDwRDQgiVPje-uy7d6u0q0x0kog%2526flowName%253DGeneralOAuthFlow%2526as%253DS-2059851122%25253A1793296950559530%2526client_id%253D1005244889474-n0223kppp1mrf9840322p0st3rf5i8dl.apps.googleusercontent.com%2523%26dsh%3DS-2059851122:1793296950559530%26flowName%3DGeneralOAuthLite%26o2v%3D2%26opparams%3D%25253F%26rart%3DANgoxcfB4wB6RuZ43fx6NPCzJTLPc_trMt0n4yzlssUiJcwRYPHXQvESUmLLzy2P3KlhnCkHUEZGsZB696TM_xUl40oWjkN4LwrLirnZfLaMmrAHjC9hpck%26redirect_uri%3Dhttps://localhost:58103/signin-google%26response_type%3Dcode%26scope%3Dopenid%2Bprofile%2Bemail%26service%3Dlso%26state%3DCfDJ8O-3dwhZkDtEqkpG7_JNHx779kfBCN_D98qgUUKEbp4OiRICsI9a8N7nANEVGAUvt9eaevc8EdMjbdhLLVGtG6Aiagrv9yOPb2y7so6BENCbgCheM4q_8PvkxaKA1hlEthmEDp2c-I6TbyU5zBXdkh-Jp-UbI31sPWz-ZZQWB2ul--_GeZAT8hYeXejFZ4TAl7PxBu6g6YgtChexXu-r6rWaZacIwVS6itiCz9WCzN3DzP9OtDb_CU_45GQUDV14pxEwClBuD84Jqj0eCoeEcGkYkK6Vg3YUkZuQUA5JpixU&state=CfDJ8O-3dwhZkDtEqkpG7_JNHx779kfBCN_D98qgUUKEbp4OiRICsI9a8N7nANEVGAUvt9eaevc8EdMjbdhLLVGtG6Aiagrv9yOPb2y7so6BENCbgCheM4q_8PvkxaKA1hlEthmEDp2c-I6TbyU5zBXdkh-Jp-UbI31sPWz-ZZQWB2ul--_GeZAT8hYeXejFZ4TAl7PxBu6g6YgtChexXu-r6rWaZacIwVS6itiCz9WCzN3DzP9OtDb_CU_45GQUDV14pxEwClBuD84Jqj0eCoeEcGkYkK6Vg3YUkZuQUA5JpixU
- contentinfo:
  - combobox:
    - option "Afrikaans"
    - option "azərbaycan"
    - option "bosanski"
    - option "català"
    - option "Čeština"
    - option "Cymraeg"
    - option "Dansk"
    - option "Deutsch"
    - option "eesti"
    - option "English (United Kingdom)"
    - option "English (United States)" [selected]
    - option "Español (España)"
    - option "Español (Latinoamérica)"
    - option "euskara"
    - option "Filipino"
    - option "Français (Canada)"
    - option "Français (France)"
    - option "Gaeilge"
    - option "galego"
    - option "Hrvatski"
    - option "Indonesia"
    - option "isiZulu"
    - option "íslenska"
    - option "Italiano"
    - option "Kiswahili"
    - option "latviešu"
    - option "lietuvių"
    - option "magyar"
    - option "Melayu"
    - option "Nederlands"
    - option "norsk"
    - option "o‘zbek"
    - option "polski"
    - option "Português (Brasil)"
    - option "Português (Portugal)"
    - option "română"
    - option "shqip"
    - option "Slovenčina"
    - option "slovenščina"
    - option "srpski (latinica)"
    - option "Suomi"
    - option "Svenska"
    - option "Tiếng Việt"
    - option "Türkçe"
    - option "Ελληνικά"
    - option "беларуская"
    - option "български"
    - option "кыргызча"
    - option "қазақ тілі"
    - option "македонски"
    - option "монгол"
    - option "Русский"
    - option "српски (ћирилица)"
    - option "Українська"
    - option "ქართული"
    - option "հայերեն"
    - option "‫עברית‬‎"
    - option "‫اردو‬‎"
    - option "‫العربية‬‎"
    - option "‫فارسی‬‎"
    - option "አማርኛ"
    - option "नेपाली"
    - option "मराठी"
    - option "हिन्दी"
    - option "অসমীয়া"
    - option "বাংলা"
    - option "ਪੰਜਾਬੀ"
    - option "ગુજરાતી"
    - option "ଓଡ଼ିଆ"
    - option "தமிழ்"
    - option "తెలుగు"
    - option "ಕನ್ನಡ"
    - option "മലയാളം"
    - option "සිංහල"
    - option "ไทย"
    - option "ລາວ"
    - option "မြန်မာ"
    - option "ខ្មែរ"
    - option "한국어"
    - option "中文（香港）"
    - option "日本語"
    - option "简体中文"
    - option "繁體中文"
  - list:
    - listitem:
      - link "Open Google Account Help Center (external, opens in a new window)":
        - /url: https://support.google.com/accounts?hl=en-US&p=account_iph
        - text: Help
    - listitem:
      - link "Privacy Policy (external, opens in a new window)":
        - /url: https://accounts.google.com/TOS?loc=UA&hl=en-US&privacy=true
        - text: Privacy
    - listitem:
      - link "Google Terms of Service (external, opens in a new window)":
        - /url: https://accounts.google.com/TOS?loc=UA&hl=en-US
        - text: Terms
```

# Test source

```ts
  1  | const { test, expect } = require('@playwright/test');
  2  | 
  3  | test('home page loads', async ({ page }) => {
  4  |   await page.goto('/');
  5  |   await expect(page).toHaveTitle(/Кафедральна бібліотека/);
  6  | });
  7  | 
  8  | test('publications page is available', async ({ page }) => {
  9  |   await page.goto('/Publications');
  10 |   await expect(page.locator('body')).toContainText('Видання');
  11 | });
  12 | 
  13 | test('unauthenticated create redirects to login', async ({ page }) => {
  14 |   await page.goto('/Authors/Create');
> 15 |   await expect(page).toHaveURL(/account\/login/i);
     |                      ^ Error: expect(page).toHaveURL(expected) failed
  16 | });
  17 | 
  18 | test('unauthenticated API mutation is denied', async ({ request }) => {
  19 |   const response = await request.post('/api/authors', { data: { fullName: 'Test Author' }, maxRedirects: 0 });
  20 |   expect([401, 302]).toContain(response.status());
  21 | });
  22 | 
```