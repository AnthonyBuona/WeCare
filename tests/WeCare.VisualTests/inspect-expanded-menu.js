const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({ ignoreHTTPSErrors: true });
  const page = await context.newPage();
  
  try {
    await page.goto('https://localhost:44373/Account/Login');
    await page.waitForTimeout(2000);
    
    // Switch Tenant
    await page.click('#AbpTenantSwitchLink');
    await page.waitForSelector('#Input_Name', { state: 'visible' });
    await page.fill('#Input_Name', 'ClinicaBemViver');
    await Promise.all([
      page.waitForNavigation({ waitUntil: 'load' }),
      page.click('.modal-footer button[type="submit"]')
    ]);
    
    // Log in
    await page.fill('#LoginInput_UserNameOrEmailAddress', 'admin@abp.io');
    await page.fill('#LoginInput_Password', '1q2w3E*');
    await Promise.all([
      page.waitForNavigation({ waitUntil: 'load' }),
      page.click('button[type="submit"]:has-text("Login")')
    ]);

    // Go to /Gamification
    console.log('Navigating to /Gamification...');
    await page.goto('https://localhost:44373/Gamification');
    await page.waitForTimeout(2000);

    // Let's inspect the Acompanhamento Clínico menu list item
    console.log('Inspecting active sidebar states on expanded Gamification page...');
    
    const results = await page.evaluate(() => {
      const items = Array.from(document.querySelectorAll('.lpx-nav .lpx-menu-item-text'));
      return items.map(el => {
        const style = window.getComputedStyle(el);
        const parentStyle = window.getComputedStyle(el.parentElement);
        const grandParentStyle = window.getComputedStyle(el.parentElement.parentElement);
        return {
          text: el.innerText.trim(),
          classes: el.className,
          display: style.display,
          visibility: style.visibility,
          opacity: style.opacity,
          parentDisplay: parentStyle.display,
          parentVisibility: parentStyle.visibility,
          grandParentDisplay: grandParentStyle.display,
          grandParentClassName: el.parentElement.parentElement.className
        };
      });
    });
    
    console.log('Computed menu item styles on expanded page:', JSON.stringify(results, null, 2));
  } catch (err) {
    console.error('Error:', err);
  } finally {
    await browser.close();
  }
})();
