const { chromium } = require('playwright');
const path = require('path');
const fs = require('fs');

(async () => {
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({
    ignoreHTTPSErrors: true,
    viewport: { width: 1280, height: 800 }
  });
  const page = await context.newPage();
  page.context().setDefaultNavigationTimeout(30000);

  const screenshotDir = path.join(__dirname, 'screenshots');
  if (!fs.existsSync(screenshotDir)) {
    fs.mkdirSync(screenshotDir);
  }

  console.log('--- STARTING WECARE PARENT PORTAL QA EXPLORATION ---');

  try {
    // 1. Load login page
    console.log('1. Navigating to login page...');
    await page.goto('https://localhost:44373/Account/Login');
    await page.waitForLoadState('load');

    // 2. Switch Tenant to ClinicaBemViver
    console.log('2. Opening Tenant switcher...');
    await page.click('#AbpTenantSwitchLink');
    await page.waitForSelector('#Input_Name', { state: 'visible' });
    await page.fill('#Input_Name', 'ClinicaBemViver');
    
    console.log('Saving tenant ClinicaBemViver...');
    await Promise.all([
      page.waitForNavigation({ waitUntil: 'load' }),
      page.click('.modal-footer button[type="submit"]')
    ]);
    
    // Verify tenant switched
    const tenantText = await page.innerText('#AbpTenantSwitchLink');
    console.log('Tenant switched to:', tenantText);

    // 3. Log in as Parent (Responsible)
    console.log('3. Logging in as Parent...');
    await page.fill('#LoginInput_UserNameOrEmailAddress', 'responsavel@7d7f722f.com');
    await page.fill('#LoginInput_Password', '1q2w3E*');
    
    await Promise.all([
      page.waitForNavigation({ waitUntil: 'load' }),
      page.click('button[type="submit"]:has-text("Login")')
    ]);

    console.log('Logged in as parent! Current URL:', page.url());
    
    // Take screenshot of the Parent Portal dashboard
    await page.screenshot({ path: path.join(screenshotDir, '09_parent_dashboard.png') });
    console.log('Screenshot 09_parent_dashboard.png saved.');

    // 4. Open the report details view modal
    console.log('4. Clicking "Visualizar Detalhes" button...');
    await page.waitForSelector('.ViewReportDetailsButton', { state: 'visible', timeout: 5000 }).catch(() => null);
    
    const viewBtnCount = await page.locator('.ViewReportDetailsButton').count();
    if (viewBtnCount > 0) {
      await page.click('.ViewReportDetailsButton');
      await page.waitForTimeout(1000); // Wait for modal animation
      
      await page.screenshot({ path: path.join(screenshotDir, '10_parent_report_view.png') });
      console.log('Screenshot 10_parent_report_view.png saved.');
      
      // Close the view modal using the close button
      console.log('Closing report details modal...');
      await page.click('.modal-footer button:has-text("Close"), .modal-footer button:has-text("Fechar")').catch(() => null);
      await page.waitForTimeout(500);
    } else {
      console.log('No report details button visible (no published reports seeded).');
    }

    // 5. Open the digital signature click-wrap modal
    console.log('5. Clicking "Dar Ciente Legal" button...');
    await page.waitForSelector('.OpenSignatureModalButton', { state: 'visible', timeout: 5000 }).catch(() => null);
    
    const signBtnCount = await page.locator('.OpenSignatureModalButton').count();
    if (signBtnCount > 0) {
      await page.click('.OpenSignatureModalButton');
      await page.waitForTimeout(1000); // Wait for modal animation
      
      await page.screenshot({ path: path.join(screenshotDir, '11_parent_signature_modal.png') });
      console.log('Screenshot 11_parent_signature_modal.png saved.');
    } else {
      console.log('No signature button visible (no pending published reports).');
    }

    console.log('--- WECARE PARENT PORTAL QA EXPLORATION SUCCESSFUL ---');

  } catch (err) {
    console.error('CRITICAL ERROR DURING PARENT EXPLORATION:', err);
    await page.screenshot({ path: path.join(screenshotDir, 'parent_error_state.png') });
    console.log('Captured error state to parent_error_state.png');
  } finally {
    await browser.close();
  }
})();
