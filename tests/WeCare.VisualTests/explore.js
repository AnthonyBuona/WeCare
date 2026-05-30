const { chromium } = require('playwright');
const path = require('path');

(async () => {
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({
    ignoreHTTPSErrors: true
  });
  const page = await context.newPage();
  
  page.context().setDefaultNavigationTimeout(30000);
  
  console.log('Navigating to https://localhost:44373/Account/Login...');
  try {
    await page.goto('https://localhost:44373/Account/Login');
    console.log('Page loaded!');
    
    // Print all links to see where the "Change" tenant link is
    const links = await page.$$eval('a', el => el.map(a => ({ id: a.id, text: a.innerText, href: a.href })));
    console.log('Links on page:', links);
    
    // Print all forms and inputs
    const inputs = await page.$$eval('input', el => el.map(i => ({ id: i.id, name: i.name, type: i.type, class: i.className })));
    console.log('Inputs on page:', inputs);

    // Take a screenshot of the login page to verify the visual state
    const screenshotPath = path.join(__dirname, 'login-page.png');
    await page.screenshot({ path: screenshotPath });
    console.log('Screenshot saved as:', screenshotPath);
  } catch (err) {
    console.error('Error during exploration:', err);
  } finally {
    await browser.close();
  }
})();
