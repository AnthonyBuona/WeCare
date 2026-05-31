const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch({ headless: true });
  
  // Test different viewport widths
  const viewports = [1400, 1200, 1024, 991, 768];

  for (let width of viewports) {
    console.log(`\n=================== Viewport Width: ${width}px ===================`);
    const context = await browser.newContext({
      ignoreHTTPSErrors: true,
      viewport: { width: width, height: 800 }
    });
    const page = await context.newPage();
    page.context().setDefaultNavigationTimeout(30000);

    try {
      await page.goto('https://localhost:44373/Account/Login');
      await page.waitForLoadState('load');

      // Switch Tenant & Log in
      await page.click('#AbpTenantSwitchLink');
      await page.waitForSelector('#Input_Name', { state: 'visible' });
      await page.fill('#Input_Name', 'ClinicaBemViver');
      await Promise.all([
        page.waitForNavigation({ waitUntil: 'load' }),
        page.click('.modal-footer button[type="submit"]')
      ]);

      await page.fill('#LoginInput_UserNameOrEmailAddress', 'admin@abp.io');
      await page.fill('#LoginInput_Password', '1q2w3E*');
      await Promise.all([
        page.waitForNavigation({ waitUntil: 'load' }),
        page.click('button[type="submit"]:has-text("Login")')
      ]);

      await page.waitForTimeout(2000);

      // Check DOM structure and sidebar width
      const info = await page.evaluate(() => {
        const wrapper = document.getElementById('lpx-wrapper');
        const sidebar = document.querySelector('.lpx-sidebar-container');
        const handle = document.getElementById('wecare-sidebar-handle');

        const wrapperClasses = wrapper ? wrapper.className : 'not found';
        const sidebarWidth = sidebar ? window.getComputedStyle(sidebar).width : 'not found';
        const sidebarDisplay = sidebar ? window.getComputedStyle(sidebar).display : 'not found';
        const handleLeft = handle ? window.getComputedStyle(handle).left : 'not found';
        
        return {
          wrapperClasses,
          sidebarWidth,
          sidebarDisplay,
          handleLeft
        };
      });

      console.log(`Wrapper Classes: "${info.wrapperClasses}"`);
      console.log(`Sidebar Computed Width: ${info.sidebarWidth}`);
      console.log(`Sidebar Display: ${info.sidebarDisplay}`);
      console.log(`Handle Left: ${info.handleLeft}`);

    } catch (err) {
      console.error(err);
    } finally {
      await context.close();
    }
  }

  await browser.close();
})();
