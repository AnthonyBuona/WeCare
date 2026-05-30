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

  // Helper to ensure screenshot directory exists
  const screenshotDir = path.join(__dirname, 'screenshots');
  if (!fs.existsSync(screenshotDir)) {
    fs.mkdirSync(screenshotDir);
  }

  console.log('--- STARTING WECARE VISUAL EXPLORATION ---');

  try {
    // 1. Load login page
    console.log('1. Navigating to login page...');
    await page.goto('https://localhost:44373/Account/Login');
    await page.waitForLoadState('load');
    await page.screenshot({ path: path.join(screenshotDir, '01_login_page.png') });
    console.log('Screenshot 01_login_page.png saved.');

    // 2. Switch Tenant
    console.log('2. Opening Tenant switcher...');
    await page.click('#AbpTenantSwitchLink');
    await page.waitForSelector('#Input_Name', { state: 'visible' });
    await page.fill('#Input_Name', 'ClinicaBemViver');
    await page.screenshot({ path: path.join(screenshotDir, '02_tenant_modal.png') });
    console.log('Screenshot 02_tenant_modal.png saved.');

    console.log('Saving tenant ClinicaBemViver...');
    await Promise.all([
      page.waitForNavigation({ waitUntil: 'load' }),
      page.click('.modal-footer button[type="submit"]')
    ]);
    
    // Verify tenant switched
    const tenantText = await page.innerText('#AbpTenantSwitchLink');
    console.log('Current Tenant Switch Link text:', tenantText);
    await page.screenshot({ path: path.join(screenshotDir, '03_login_page_tenant_selected.png') });
    console.log('Screenshot 03_login_page_tenant_selected.png saved.');

    // 3. Log in as Tenant Admin
    console.log('3. Submitting credentials...');
    await page.fill('#LoginInput_UserNameOrEmailAddress', 'admin@abp.io');
    await page.fill('#LoginInput_Password', '1q2w3E*');
    
    await page.click('button[type="submit"]:has-text("Login")');
    await page.waitForURL(url => url.pathname === '/' || url.pathname === '/Dashboard' || url.href.endsWith(':44373/'), { waitUntil: 'load', timeout: 20000 });

    console.log('Logged in! Current URL:', page.url());
    await page.screenshot({ path: path.join(screenshotDir, '04_dashboard_host.png') });
    console.log('Screenshot 04_dashboard_host.png saved.');

    // 4. Navigate to Patients page
    console.log('4. Navigating to Patients page (/Patients)...');
    await page.goto('https://localhost:44373/Patients');
    await page.waitForLoadState('load');
    await page.waitForSelector('#PatientsTable', { state: 'visible' });
    
    // Take a screenshot of the main layout, checking card alignments, padding, and layout
    await page.screenshot({ path: path.join(screenshotDir, '05_patients_list.png') });
    console.log('Screenshot 05_patients_list.png saved.');

    // 5. Interact with Datatable action dropdown
    console.log('5. Clicking Datatable dropdown action button...');
    
    // Let's locate the Actions button in the Datatable. Usually it contains .btn-group or dropdown-toggle
    const actionButtonSelector = '#PatientsTable tbody tr:first-child td:first-child button, #PatientsTable tbody tr:first-child td:first-child a';
    
    await page.waitForSelector(actionButtonSelector, { state: 'visible' });
    await page.click(actionButtonSelector);
    
    // Wait for the dropdown menu to expand
    await page.waitForTimeout(500); 
    
    // Take screenshot of open dropdown to check for misalignment or detaching
    await page.screenshot({ path: path.join(screenshotDir, '06_patients_dropdown_clicked.png') });
    console.log('Screenshot 06_patients_dropdown_clicked.png saved.');

    // Let's get the text of all items inside the open dropdown
    const dropdownItems = await page.$$eval('.dropdown-menu.show a, .dropdown-menu.show button', el => el.map(a => a.innerText.trim()));
    console.log('Visible dropdown menu options:', dropdownItems);

    // 6. Navigate to a Patient Dashboard
    console.log('6. Navigating to Patient Dashboard (Lucas Paciente)...');
    
    // Let's find the link to Dashboard in the dropdown
    // Based on Patients/index.js, the Dashboard link is the first item in the rowAction array
    const dashboardOptionSelector = '.dropdown-menu.show a:has-text("Dashboard"), .dropdown-menu.show button:has-text("Dashboard")';
    
    await page.click(dashboardOptionSelector);
    await page.waitForURL(url => url.pathname.includes('/Patients/Dashboard'), { waitUntil: 'load', timeout: 20000 });
    
    console.log('Dashboard loaded! URL:', page.url());
    
    // Capture Patient Dashboard layout
    await page.screenshot({ path: path.join(screenshotDir, '07_patient_dashboard.png') });
    console.log('Screenshot 07_patient_dashboard.png saved.');

    // 7. Check if there are other dashboards or routes
    console.log('7. Visiting Calendar page...');
    await page.goto('https://localhost:44373/Calendar');
    await page.waitForLoadState('load');
    await page.screenshot({ path: path.join(screenshotDir, '08_calendar.png') });
    console.log('Screenshot 08_calendar.png saved.');

    console.log('--- WECARE VISUAL EXPLORATION COMPLETED SUCCESSFULLY ---');

  } catch (err) {
    console.error('CRITICAL ERROR DURING EXPLORATION:', err);
    await page.screenshot({ path: path.join(screenshotDir, 'error_state.png') });
    console.log('Captured error state to error_state.png');
  } finally {
    await browser.close();
  }
})();
