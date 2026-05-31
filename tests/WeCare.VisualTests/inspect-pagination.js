const { chromium } = require('playwright');

(async () => {
    const browser = await chromium.launch({ 
        headless: true,
        args: ['--ignore-certificate-errors', '--ignore-ssl-errors']
    });
    const context = await browser.newContext({
        ignoreHTTPSErrors: true
    });
    const page = await context.newPage();
    
    // Navigate and login
    await page.goto('https://localhost:44373/Account/Login');
    await page.waitForTimeout(2000);
    
    // Switch tenant
    const switchBtn = await page.$('a:has-text("switch"), button:has-text("switch")');
    if (switchBtn) {
        await switchBtn.click();
        await page.waitForTimeout(1000);
        const tenantInput = await page.$('#TenantName, input[id*="enant"]');
        if (tenantInput) {
            await tenantInput.fill('ClinicaBemViver');
            const applyBtn = await page.$('button:has-text("Salvar"), button[type="submit"]');
            if (applyBtn) await applyBtn.click();
            await page.waitForTimeout(1000);
        }
    }
    
    await page.fill('#Input_UserNameOrEmailAddress', 'admin');
    await page.fill('#Input_Password', '1q2w3E*');
    await page.click('button[type="submit"]');
    await page.waitForNavigation({ waitUntil: 'networkidle' });
    
    await page.goto('https://localhost:44373/Patients');
    await page.waitForTimeout(3000);
    
    // Get pagination HTML and classes
    const paginationInfo = await page.evaluate(() => {
        const pag = document.querySelector('.dataTables_paginate');
        if (!pag) return { html: 'NOT FOUND', classes: [] };
        
        const activeBtn = pag.querySelector('.current, .active, [aria-current]');
        return {
            html: pag.outerHTML.substring(0, 800),
            activeClass: activeBtn ? activeBtn.className : 'no active btn found',
            activeHtml: activeBtn ? activeBtn.outerHTML : 'none'
        };
    });
    
    console.log('Pagination info:', JSON.stringify(paginationInfo, null, 2));
    
    // Also get sidebar menu item classes
    const sidebarInfo = await page.evaluate(() => {
        const sidebar = document.querySelector('.lpx-sidebar, aside');
        const activeItem = sidebar ? sidebar.querySelector('.active, .selected, [class*="active"]') : null;
        const menuItems = sidebar ? sidebar.querySelectorAll('li, .lpx-menu-item') : [];
        return {
            activeItemClass: activeItem ? activeItem.className : 'none',
            totalMenuItems: menuItems.length,
            firstItemHtml: menuItems[0] ? menuItems[0].outerHTML.substring(0, 300) : 'none'
        };
    });
    
    console.log('Sidebar info:', JSON.stringify(sidebarInfo, null, 2));
    
    await browser.close();
})();
