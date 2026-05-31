const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({
    ignoreHTTPSErrors: true,
    viewport: { width: 1280, height: 800 }
  });
  const page = await context.newPage();

  try {
    await page.goto('https://localhost:44373/Account/Login');
    await page.waitForLoadState('load');

    // Login
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

    console.log('Resizing viewport to 1024px tablet resolution...');
    await page.setViewportSize({ width: 1024, height: 800 });
    await page.waitForTimeout(2000);

    // Find and print toggle elements in the DOM
    const toggles = await page.evaluate(() => {
      const elList = Array.from(document.querySelectorAll('*')).filter(el => {
        const hasToggleAttr = Array.from(el.attributes).some(attr => attr.name.includes('toggle') || attr.name.includes('lpx'));
        const hasToggleClass = Array.from(el.classList).some(c => c.includes('toggle') || c.includes('collapse') || c.includes('hamburger') || c.includes('menu'));
        return hasToggleAttr || hasToggleClass;
      });
      return elList.map(el => ({
        tagName: el.tagName,
        id: el.id,
        className: el.className,
        attributes: Array.from(el.attributes).map(attr => `${attr.name}="${attr.value}"`),
        innerText: el.innerText ? el.innerText.substring(0, 30) : ''
      }));
    });
    console.log('--- FOUND TOGGLE CANDIDATES IN DOM:', toggles.slice(0, 20));

    // Get initial state
    let state = await page.evaluate(() => {
      const wrapper = document.getElementById('lpx-wrapper');
      const sidebar = document.querySelector('.lpx-sidebar-container');
      const handle = document.getElementById('wecare-sidebar-handle');
      const content = document.querySelector('.lpx-content-container') || document.querySelector('.lpx-content') || document.querySelector('main');
      return {
        wrapperClasses: wrapper ? wrapper.className : 'null',
        sidebarWidth: sidebar ? window.getComputedStyle(sidebar).width : 'null',
        contentTagName: content ? content.tagName : 'null',
        contentClassName: content ? content.className : 'null',
        contentWidth: content ? window.getComputedStyle(content).width : 'null',
        contentPaddingLeft: content ? window.getComputedStyle(content).paddingLeft : 'null',
        contentMarginLeft: content ? window.getComputedStyle(content).marginLeft : 'null',
        handleLeft: handle ? window.getComputedStyle(handle).left : 'null'
      };
    });
    console.log('--- Initial Tablet State:', state);

    // Let's click the handle to toggle
    console.log('Clicking the handle...');
    await page.click('#wecare-sidebar-handle');
    await page.waitForTimeout(1000);

    state = await page.evaluate(() => {
      const wrapper = document.getElementById('lpx-wrapper');
      const sidebar = document.querySelector('.lpx-sidebar-container');
      const handle = document.getElementById('wecare-sidebar-handle');
      const content = document.querySelector('.lpx-content-container') || document.querySelector('.lpx-content') || document.querySelector('main');
      return {
        wrapperClasses: wrapper ? wrapper.className : 'null',
        sidebarWidth: sidebar ? window.getComputedStyle(sidebar).width : 'null',
        contentWidth: content ? window.getComputedStyle(content).width : 'null',
        contentPaddingLeft: content ? window.getComputedStyle(content).paddingLeft : 'null',
        contentMarginLeft: content ? window.getComputedStyle(content).marginLeft : 'null',
        handleLeft: handle ? window.getComputedStyle(handle).left : 'null'
      };
    });
    console.log('--- After First Toggle:', state);

    // Let's click it again
    console.log('Clicking the handle again...');
    await page.click('#wecare-sidebar-handle');
    await page.waitForTimeout(1000);

    state = await page.evaluate(() => {
      const wrapper = document.getElementById('lpx-wrapper');
      const sidebar = document.querySelector('.lpx-sidebar-container');
      const handle = document.getElementById('wecare-sidebar-handle');
      const content = document.querySelector('.lpx-content-container') || document.querySelector('.lpx-content') || document.querySelector('main');
      return {
        wrapperClasses: wrapper ? wrapper.className : 'null',
        sidebarWidth: sidebar ? window.getComputedStyle(sidebar).width : 'null',
        contentWidth: content ? window.getComputedStyle(content).width : 'null',
        contentPaddingLeft: content ? window.getComputedStyle(content).paddingLeft : 'null',
        contentMarginLeft: content ? window.getComputedStyle(content).marginLeft : 'null',
        handleLeft: handle ? window.getComputedStyle(handle).left : 'null'
      };
    });
    console.log('--- After Second Toggle:', state);

  } catch (err) {
    console.error(err);
  } finally {
    await browser.close();
  }
})();
