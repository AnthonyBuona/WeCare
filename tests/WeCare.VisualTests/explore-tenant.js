const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({ ignoreHTTPSErrors: true });
  const page = await context.newPage();
  
  try {
    await page.goto('https://localhost:44373/Account/Login');
    console.log('Main login page loaded.');

    console.log('Clicking tenant switch link...');
    await page.click('#AbpTenantSwitchLink');
    
    // Wait for the modal or dynamic container
    await page.waitForTimeout(1000); // Wait for modal animation
    
    console.log('Dumping modal elements...');
    const inputs = await page.$$eval('input', el => el.map(i => ({ id: i.id, name: i.name, type: i.type, class: i.className })));
    console.log('Inputs found after clicking switch:', inputs);
    
    const buttons = await page.$$eval('button', el => el.map(b => ({ id: b.id, text: b.innerText, type: b.type, class: b.className })));
    console.log('Buttons found after clicking switch:', buttons);
  } catch (err) {
    console.error('Error:', err);
  } finally {
    await browser.close();
  }
})();
