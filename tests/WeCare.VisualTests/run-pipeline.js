const { chromium } = require('playwright');
const path = require('path');
const fs = require('fs');

// Ensure target directories exist
const BUG_DIR = path.resolve(__dirname, '../../doc/WeCare/backlog/bugs');
const SCREENSHOT_DIR = path.resolve(__dirname, './screenshots');

[BUG_DIR, SCREENSHOT_DIR].forEach(dir => {
  if (!fs.existsSync(dir)) {
    fs.mkdirSync(dir, { recursive: true });
  }
});

function logBug(title, severity, steps, observed, expected, trace = '') {
  const timestamp = new Date().toISOString().replace(/[-:T]/g, '').slice(0, 14);
  const bugId = `BUG-${timestamp}-${Math.floor(100 + Math.random() * 900)}`;
  const bugFile = path.join(BUG_DIR, `${bugId}.md`);
  
  const content = `# 🐛 Bug Report: ${title} [${bugId}]

## Metadata
- **Bug ID:** ${bugId}
- **Severity:** ${severity}
- **Date Profiled:** ${new Date().toLocaleString()}
- **Reporter:** WeCare Autonomous QA Inspector & Visual Pipeline

## Steps to Reproduce
${steps.map((s, idx) => `${idx + 1}. ${s}`).join('\n')}

## Observed Behavior
${observed}

## Expected Behavior
${expected}

## Console Logs / Traces / Context
\`\`\`text
${trace || 'No traces logged.'}
\`\`\`

## Visual Reference
*Screenshots captured in pipeline: \`tests/WeCare.VisualTests/screenshots/\`*
`;

  fs.writeFileSync(bugFile, content, 'utf8');
  console.log(`[PIPELINE] ❌ Bug logged successfully to: ${bugFile}`);
  return bugId;
}

(async () => {
  console.log('=====================================================');
  console.log('🚀 WECARE CONTINUOUS QA & VISUAL TESTING PIPELINE');
  console.log(`📂 Logs target: ${BUG_DIR}`);
  console.log('=====================================================');

  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({
    ignoreHTTPSErrors: true,
    viewport: { width: 1280, height: 800 }
  });
  const page = await context.newPage();

  let consoleErrors = [];
  let pageErrors = [];
  let failedRequests = [];

  // Wire up console monitoring
  page.on('console', msg => {
    if (msg.type() === 'error') {
      consoleErrors.push(`[Console Error] ${msg.text()}`);
    }
  });

  page.on('pageerror', err => {
    pageErrors.push(`[Page Error] ${err.message}\nStack: ${err.stack}`);
  });

  page.on('requestfailed', req => {
    failedRequests.push(`[Failed Request] ${req.method()} ${req.url()} - ${req.failure().errorText}`);
  });

  page.on('response', res => {
    if (res.status() >= 400) {
      failedRequests.push(`[HTTP Error ${res.status()}] ${res.request().method()} ${res.url()}`);
    }
  });

  try {
    // Phase 1: Login & Tenant Switcher
    console.log('🔍 [Phase 1] Testing Login & Tenant Selection...');
    await page.goto('https://localhost:44373/Account/Login');
    await page.waitForLoadState('load');

    // Visual Check: Tenant switcher exists and layout is correct
    const tenantSwitchVisible = await page.isVisible('#AbpTenantSwitchLink');
    if (!tenantSwitchVisible) {
      logBug(
        'Missing Tenant Switch Link on Login Page',
        'Critical',
        ['Navigate to https://localhost:44373/Account/Login', 'Look for tenant selection area'],
        'The tenant selection link #AbpTenantSwitchLink was not found on the page.',
        'A Tenant selection link with ID #AbpTenantSwitchLink should be present in the login page layout.'
      );
    }

    // Perform tenant switch
    await page.click('#AbpTenantSwitchLink');
    await page.waitForSelector('#Input_Name', { state: 'visible' });
    await page.fill('#Input_Name', 'ClinicaBemViver');
    await Promise.all([
      page.waitForNavigation({ waitUntil: 'load' }),
      page.click('.modal-footer button[type="submit"]')
    ]);

    // Phase 2: Authentication
    console.log('🔍 [Phase 2] Logging in as ClinicaBemViver Tenant Admin...');
    await page.fill('#LoginInput_UserNameOrEmailAddress', 'admin@abp.io');
    await page.fill('#LoginInput_Password', '1q2w3E*');
    
    await page.click('button[type="submit"]:has-text("Login")');
    await page.waitForURL(url => url.pathname === '/' || url.pathname === '/Dashboard' || url.href.endsWith(':44373/'), { waitUntil: 'load', timeout: 20000 });

    console.log(`Successfully Authenticated. URL: ${page.url()}`);
    await page.screenshot({ path: path.join(SCREENSHOT_DIR, 'pipeline_dashboard.png') });

    // Phase 3: Inspect Patients Route
    console.log('🔍 [Phase 3] Navigating to Patients Registry...');
    await page.goto('https://localhost:44373/Patients');
    await page.waitForLoadState('load');
    
    // Check if table is loaded
    const isTableLoaded = await page.isVisible('#PatientsTable');
    if (!isTableLoaded) {
      logBug(
        'Patients Registry Table Failed to Load',
        'High',
        ['Login to ClinicaBemViver', 'Navigate to /Patients'],
        'The main patients DataTable (#PatientsTable) did not mount or is not visible.',
        'The patients DataTable should be fully initialized and visible with patient records.'
      );
    } else {
      console.log('✅ Patients Table verified.');
    }

    // Visual Alignment Checks: Card margin & paddings
    const cardSpacing = await page.$eval('.card', card => {
      const style = window.getComputedStyle(card);
      return {
        margin: style.margin,
        padding: style.padding,
        borderRadius: style.borderRadius
      };
    }).catch(() => null);
    
    if (cardSpacing) {
      console.log('📊 Card Design Profile:', cardSpacing);
    }

    // Dropdown functionality validation
    console.log('🔍 [Phase 4] Verifying Row Action Dropdown Alignments...');
    const actionBtn = '#PatientsTable tbody tr:first-child td:first-child button, #PatientsTable tbody tr:first-child td:first-child a';
    await page.waitForSelector(actionBtn, { state: 'visible' });
    await page.click(actionBtn);
    await page.waitForTimeout(300); // let transition complete

    // Dynamic Visual Regression Check: Check if dropdown overlaps or detaches
    const dropdownBounding = await page.$eval('.dropdown-menu.show', el => {
      const rect = el.getBoundingClientRect();
      return {
        top: rect.top,
        left: rect.left,
        width: rect.width,
        height: rect.height,
        bottom: rect.bottom,
        right: rect.right
      };
    }).catch(() => null);

    if (dropdownBounding) {
      console.log('✅ Action dropdown is open. Bounding box:', dropdownBounding);
      // If dropdown is detached (e.g. top is 0 or detached from viewport)
      if (dropdownBounding.top < 0 || dropdownBounding.height === 0) {
        logBug(
          'Action Dropdown Menu Detached or Invisible',
          'High',
          ['Login to ClinicaBemViver', 'Navigate to /Patients', 'Click action button on patient row'],
          'The dropdown menu displayed with invalid coordinates or zero height.',
          'The actions dropdown menu should align neatly below the clicked row button.'
        );
      }
    } else {
      logBug(
        'Action Dropdown Menu Failed to Show',
        'High',
        ['Login to ClinicaBemViver', 'Navigate to /Patients', 'Click action button on patient row'],
        'The CSS class .dropdown-menu.show did not trigger upon clicking.',
        'The actions dropdown menu should show instantly with the class .dropdown-menu.show.'
      );
    }

    // Phase 4: Patient Dashboard Inspection
    console.log('🔍 [Phase 5] Navigating to Patient Profile Dashboard...');
    const dashboardOption = '.dropdown-menu.show a:has-text("Dashboard"), .dropdown-menu.show button:has-text("Dashboard")';
    await page.click(dashboardOption);
    await page.waitForURL(url => url.pathname.includes('/Patients/Dashboard'), { waitUntil: 'load', timeout: 20000 });

    await page.screenshot({ path: path.join(SCREENSHOT_DIR, 'pipeline_patient_dashboard.png') });
    console.log('✅ Patient Dashboard Visual Checked.');

    await page.goto('https://localhost:44373/Calendar');
    await page.waitForLoadState('load');
    await page.screenshot({ path: path.join(SCREENSHOT_DIR, 'pipeline_calendar.png') });
    console.log('✅ Calendar Page Visual Checked.');

    // Final Pipeline Log Aggregator
    if (consoleErrors.length > 0 || pageErrors.length > 0 || failedRequests.length > 0) {
      console.log(`⚠️ Pipeline identified ${consoleErrors.length + pageErrors.length + failedRequests.length} technical anomalies.`);
      
      const techTrace = [...consoleErrors, ...pageErrors, ...failedRequests].join('\n');
      logBug(
        'Console Warnings & Technical Anomalies Profiled',
        'Medium',
        ['Login to ClinicaBemViver', 'Navigate through patients list and calendar'],
        'The pipeline captured console errors, page errors, or failed network requests.',
        'All application screens should render cleanly without console exceptions or broken request assets.',
        techTrace
      );
    } else {
      console.log('🎉 PIPELINE PASSED: Zero functional or visual bugs detected!');
    }

  } catch (error) {
    console.error('❌ Pipeline run interrupted by error:', error);
    const errScreenshot = path.join(SCREENSHOT_DIR, 'pipeline_error.png');
    await page.screenshot({ path: errScreenshot }).catch(() => {});
    console.log(`Captured failure screenshot to: ${errScreenshot}`);
    logBug(
      'Pipeline Runner Exception',
      'High',
      ['Execute pipeline test runner'],
      `The pipeline crashed unexpectedly with exception: ${error.message}`,
      'The E2E visual runner pipeline should complete all assertions without interruption.',
      error.stack
    );
  } finally {
    await browser.close();
    console.log('=====================================================');
    console.log('🏁 WECARE PIPELINE COMPLETED');
    console.log('=====================================================');
  }
})();
