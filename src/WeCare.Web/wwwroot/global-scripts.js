/* Your Global Scripts */

$(function () {
    // ==========================================
    // Sidebar Pull-Handle ("Fitinha") — Posicionamento dinâmico via ResizeObserver
    // 'left' é sempre a largura real do sidebar, sem conflito de CSS
    // ==========================================

    function syncHandlePosition() {
        var $sidebar = $('.lpx-sidebar-container');
        var $handle = $('#wecare-sidebar-handle');
        if ($sidebar.length > 0 && $handle.length > 0) {
            var w = $sidebar[0].getBoundingClientRect().width;
            if (w > 20) {
                $handle.css('left', w + 'px').show();
                $handle.find('i')
                    .toggleClass('fa-chevron-right', w < 120)
                    .toggleClass('fa-chevron-left', w >= 120);
            } else {
                $handle.hide();
            }
        }
    }

    function setupSidebarHandle() {
        var $wrapper = $('#lpx-wrapper');
        var $sidebar = $('.lpx-sidebar-container');

        if ($wrapper.length === 0 || $sidebar.length === 0) return;

        // Cria o handle apenas uma vez
        if ($('#wecare-sidebar-handle').length === 0) {
            // Anula qualquer observer anterior
            if (window.wecareSidebarObserver) {
                window.wecareSidebarObserver.disconnect();
                window.wecareSidebarObserver = null;
            }
            // Insere o handle como filho direto do body para evitar problemas de z-index
            $('body').append('<div id="wecare-sidebar-handle" class="wecare-sidebar-handle"><i class="fa fa-chevron-left"></i></div>');
        }

        // Posiciona imediatamente (sem esperar resize)
        syncHandlePosition();

        // Observa mudanças de tamanho do sidebar (collapse/expand)
        if (!window.wecareSidebarObserver) {
            var observer = new ResizeObserver(function () {
                syncHandlePosition();
            });
            observer.observe($sidebar[0]);
            window.wecareSidebarObserver = observer;
        }
    }

    // Configura na inicialização
    setupSidebarHandle();

    // Click no handle: aciona o toggle nativo do LeptonX
    $(document).on('click', '#wecare-sidebar-handle', function () {
        var $nativeToggle = $('i[data-lpx-toggle="sidebar"], .menu-collapse-icon, [data-lpx-toggle="sidebar"]');
        if ($nativeToggle.length > 0) {
            $nativeToggle.first().trigger('click');
        } else {
            // Fallback manual
            var $wrapper = $('#lpx-wrapper');
            if ($wrapper.hasClass('hover-trigger')) {
                $wrapper.removeClass('hover-trigger');
                localStorage.setItem('lpx:side-menu-state', '0');
            } else {
                $wrapper.addClass('hover-trigger');
                localStorage.setItem('lpx:side-menu-state', '1');
            }
        }
    });

    // Garante que o handle persiste após navegações SPA (Blazor/AJAX)
    setInterval(setupSidebarHandle, 1500);

    // ==========================================
    // Global Patient Quick-Search — Topbar
    // ==========================================

    function setupGlobalPatientSearch() {
        if ($('#wecare-global-patient-search-wrap').length > 0) return;

        var $topbarRight = $('.lpx-topbar-area, .lpx-topbar .lpx-toolbar-container, .lpx-topbar .d-flex, .lpx-topbar nav');
        if ($topbarRight.length === 0) return;

        var searchHtml = [
            '<div id="wecare-global-patient-search-wrap" class="wecare-search-wrap me-2">',
            '  <div class="wecare-search-inner">',
            '    <i class="fa fa-search wecare-search-icon"></i>',
            '    <input type="text" id="global-patient-search" class="wecare-search-input" placeholder="Buscar paciente..." autocomplete="off" />',
            '    <div id="wecare-search-dropdown" class="wecare-search-dropdown"></div>',
            '  </div>',
            '</div>'
        ].join('');

        $topbarRight.first().prepend(searchHtml);

        // Autocomplete logic
        var searchTimer = null;
        $('#global-patient-search').on('input', function () {
            clearTimeout(searchTimer);
            var term = $(this).val().trim();
            if (term.length < 2) {
                $('#wecare-search-dropdown').hide().empty();
                return;
            }
            searchTimer = setTimeout(function () {
                weCare.patients.patient.getList({
                    filter: term,
                    maxResultCount: 8,
                    skipCount: 0
                }).then(function (result) {
                    var $dd = $('#wecare-search-dropdown');
                    $dd.empty();
                    if (!result.items || result.items.length === 0) {
                        $dd.append('<div class="wecare-search-no-result">Nenhum paciente encontrado</div>');
                        $dd.show();
                        return;
                    }
                    result.items.forEach(function (p) {
                        var $item = $('<div class="wecare-search-item"></div>');
                        $item.html(
                            '<i class="fa fa-user-circle wecare-search-item-icon"></i>' +
                            '<div><div class="wecare-search-item-name">' + $('<span>').text(p.name).html() + '</div>' +
                            '<div class="wecare-search-item-sub">' + (p.gender || '') + (p.birthDate ? ' · ' + new Date(p.birthDate).toLocaleDateString('pt-BR') : '') + '</div></div>'
                        );
                        $item.on('click', function () {
                            window.location.href = '/Patients/Dashboard/' + p.id;
                        });
                        $dd.append($item);
                    });
                    $dd.show();
                }).catch(function () {
                    // silently ignore errors (user may not have permission)
                    $('#wecare-search-dropdown').hide().empty();
                });
            }, 280);
        });

        // Close dropdown on outside click
        $(document).on('click.wecareSearch', function (e) {
            if (!$(e.target).closest('#wecare-global-patient-search-wrap').length) {
                $('#wecare-search-dropdown').hide().empty();
            }
        });

        // Keyboard navigation
        $('#global-patient-search').on('keydown', function (e) {
            var $dd = $('#wecare-search-dropdown');
            var $items = $dd.find('.wecare-search-item');
            var $active = $items.filter('.active');
            if (e.key === 'ArrowDown') {
                e.preventDefault();
                if ($active.length === 0) $items.first().addClass('active');
                else { $active.removeClass('active').next('.wecare-search-item').addClass('active'); }
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                if ($active.length === 0) $items.last().addClass('active');
                else { $active.removeClass('active').prev('.wecare-search-item').addClass('active'); }
            } else if (e.key === 'Enter') {
                if ($active.length > 0) { $active.trigger('click'); }
            } else if (e.key === 'Escape') {
                $dd.hide().empty();
            }
        });
    }

    // Run after DOM is ready and retry until topbar is present
    setupGlobalPatientSearch();
    var searchSetupInterval = setInterval(function () {
        if ($('#wecare-global-patient-search-wrap').length === 0) {
            setupGlobalPatientSearch();
        } else {
            clearInterval(searchSetupInterval);
        }
    }, 800);

    // ==========================================
    // In-App Notifications — Polling
    // ==========================================

    function setupNotifications() {
        if (typeof abp === 'undefined' || !abp.currentUser || !abp.currentUser.isAuthenticated) return;
        if ($('#wecare-notification-wrap').length > 0) return;

        var $topbarRight = $('.lpx-topbar-area, .lpx-topbar .lpx-toolbar-container, .lpx-topbar .d-flex, .lpx-topbar nav');
        if ($topbarRight.length === 0) return;

        var notifHtml = [
            '<div id="wecare-notification-wrap" class="wecare-notification-wrap me-3" style="position: relative;">',
            '  <div class="wecare-notification-bell-btn" id="notification-bell" style="cursor: pointer; position: relative; font-size: 1.25rem;">',
            '    <i class="fa fa-bell" style="color: var(--bs-primary);"></i>',
            '    <span class="wecare-notification-badge" id="notification-badge" style="display: none; position: absolute; top: -6px; right: -6px; background-color: #f5576c; color: white; font-size: 0.68rem; font-weight: bold; border-radius: 20px; padding: 2px 6px; box-shadow: 0 2px 5px rgba(0,0,0,0.2);">0</span>',
            '  </div>',
            '  <div class="wecare-notification-dropdown" id="notification-dropdown" style="display: none; position: absolute; top: 46px; right: 0; width: 340px; background: rgba(255,255,255,0.9); backdrop-filter: blur(20px) saturate(180%); border: 1px solid rgba(255,255,255,0.5); border-radius: 16px; box-shadow: 0 10px 30px rgba(0,0,0,0.1); z-index: 10000; overflow: hidden;">',
            '    <div class="wecare-notification-header" style="display: flex; justify-content: space-between; align-items: center; padding: 12px 16px; border-bottom: 1px solid rgba(0,0,0,0.05); background: rgba(255,255,255,0.5);">',
            '      <h6 class="mb-0 fw-bold text-dark" style="font-size: 0.95rem;">Notificações</h6>',
            '      <button class="btn btn-sm btn-link text-decoration-none p-0 fw-bold" id="notification-clear-all" style="font-size: 0.8rem; color: var(--bs-primary);">Limpar tudo</button>',
            '    </div>',
            '    <div class="wecare-notification-body" id="notification-list" style="max-height: 320px; overflow-y: auto; padding: 8px 0;">',
            '      <div class="wecare-notification-empty" style="text-align: center; padding: 24px; color: #888; font-size: 0.88rem;">',
            '        <i class="fa fa-check-circle-o fa-2x mb-2 text-success" style="opacity: 0.7;"></i>',
            '        <div>Tudo em dia por aqui!</div>',
            '      </div>',
            '    </div>',
            '  </div>',
            '</div>'
        ].join('');

        // Prepend next to search bar
        $topbarRight.first().prepend(notifHtml);

        // Click handler to toggle dropdown
        $('#notification-bell').on('click', function (e) {
            e.stopPropagation();
            $('#notification-dropdown').slideToggle(200);
            $('#wecare-search-dropdown').hide().empty(); // Close search dropdown
        });

        // Close dropdown on outside click
        $(document).on('click.wecareNotif', function (e) {
            if (!$(e.target).closest('#wecare-notification-wrap').length) {
                $('#notification-dropdown').hide();
            }
        });

        var allNotifIds = [];

        function updateNotifications() {
            if (typeof weCare === 'undefined' || !weCare.notifications || !weCare.notifications.notification) return;

            weCare.notifications.notification.getUnread().then(function (result) {
                var readIds = JSON.parse(localStorage.getItem('wecare-read-notifications') || '[]');
                var unreadList = (result || []).filter(function (n) {
                    return readIds.indexOf(n.id) === -1;
                });

                allNotifIds = (result || []).map(function (n) { return n.id; });

                var $badge = $('#notification-badge');
                var $list = $('#notification-list');

                if (unreadList.length > 0) {
                    $badge.text(unreadList.length).show();
                    $list.empty();

                    unreadList.forEach(function (n) {
                        var iconClass = 'fa-info-circle text-primary';
                        if (n.type === 'consultation') iconClass = 'fa-calendar-check-o text-success';
                        else if (n.type === 'report') iconClass = 'fa-file-text-o text-warning';
                        else if (n.type === 'consent') iconClass = 'fa-shield text-danger';

                        var $item = $('<div class="wecare-notification-item" style="display: flex; gap: 12px; padding: 12px 16px; border-bottom: 1px solid rgba(0,0,0,0.03); cursor: pointer; transition: background 0.2s ease;"></div>');
                        $item.html(
                            '  <div class="notif-icon-wrap" style="font-size: 1.15rem; flex-shrink: 0; padding-top: 2px;">' +
                            '    <i class="fa ' + iconClass + '"></i>' +
                            '  </div>' +
                            '  <div style="flex-grow: 1;">' +
                            '    <div style="font-size: 0.88rem; font-weight: bold; color: #111827; margin-bottom: 2px;">' + $('<span>').text(n.title).html() + '</div>' +
                            '    <div style="font-size: 0.8rem; color: #4b5563; line-height: 1.35; margin-bottom: 4px;">' + $('<span>').text(n.message).html() + '</div>' +
                            '    <div style="font-size: 0.72rem; color: #9ca3af;"><i class="fa fa-clock-o me-1"></i>' + new Date(n.createdAt).toLocaleDateString('pt-BR') + '</div>' +
                            '  </div>'
                        );

                        $item.hover(
                            function () { $(this).css('background', 'rgba(0,0,0,0.03)'); },
                            function () { $(this).css('background', 'transparent'); }
                        );

                        $item.on('click', function () {
                            readIds.push(n.id);
                            localStorage.setItem('wecare-read-notifications', JSON.stringify(readIds));
                            $('#notification-dropdown').hide();
                            window.location.href = n.actionUrl;
                        });

                        $list.append($item);
                    });
                } else {
                    $badge.hide().text('0');
                    $list.html(
                        '      <div class="wecare-notification-empty" style="text-align: center; padding: 24px; color: #888; font-size: 0.88rem;">' +
                        '        <i class="fa fa-check-circle-o fa-2x mb-2 text-success" style="opacity: 0.7;"></i>' +
                        '        <div>Tudo em dia por aqui!</div>' +
                        '      </div>'
                    );
                }
            }).catch(function () {
                // Ignore silent API failures (guest pages etc.)
            });
        }

        $('#notification-clear-all').on('click', function () {
            var readIds = JSON.parse(localStorage.getItem('wecare-read-notifications') || '[]');
            allNotifIds.forEach(function (id) {
                if (readIds.indexOf(id) === -1) readIds.push(id);
            });
            localStorage.setItem('wecare-read-notifications', JSON.stringify(readIds));
            updateNotifications();
        });

        // Run immediately and poll every 90s
        updateNotifications();
        setInterval(updateNotifications, 90000);
    }

    // Run notifications setup and retry until topbar is present
    setupNotifications();
    var notifSetupInterval = setInterval(function () {
        if ($('#wecare-notification-wrap').length === 0) {
            setupNotifications();
        } else {
            clearInterval(notifSetupInterval);
        }
    }, 800);

    // Masking for Admin Password in all tables (Tenants, Clinics, etc.)
    $(document).on('draw.dt', function (e, settings) {
        maskAdminPasswords();
    });

    function maskAdminPasswords() {
        $('table.dataTable tbody tr').each(function () {
            var $row = $(this);
            // Tentativa de encontrar a coluna pela posição ou nome se possível
            // Como usamos ObjectExtensionManager, o ABP coloca os campos extras no final ou em ordem alfabética.
            // Vamos procurar pelo conteúdo que parece uma senha ou pela posição aproximada se soubermos.
            // Melhor ainda: procurar por uma célula que ainda não foi mascarada e que sabemos ser a de senha.

            // Nota: O ObjectExtensionManager costuma atribuir nomes de colunas baseados no nome da propriedade.
            // Vamos tentar encontrar a coluna "Admin Password" pelo índice do header.
            var passwordColumnIndex = -1;
            $row.closest('table').find('thead th').each(function (index) {
                var headerText = $(this).text().trim().toLowerCase();
                if (headerText === 'admin password') {
                    passwordColumnIndex = index;
                }
            });

            if (passwordColumnIndex !== -1) {
                var $cell = $row.find('td').eq(passwordColumnIndex);
                if (!$cell.hasClass('password-masked') && $cell.text().trim() !== '') {
                    var originalValue = $cell.text().trim();
                    $cell.addClass('password-masked');
                    $cell.data('original-value', originalValue);
                    $cell.html('<span class="masked-text">********</span> <a href="javascript:void(0);" class="btn btn-sm btn-link reveal-password"><i class="fa fa-eye"></i></a>');
                }
            }
        });
    }

    $(document).on('click', '.reveal-password', function () {
        var $cell = $(this).closest('td');
        var originalValue = $cell.data('original-value');
        var $span = $cell.find('.masked-text');
        var $icon = $(this).find('i');

        if ($span.text() === '********') {
            $span.text(originalValue);
            $icon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            $span.text('********');
            $icon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });
});
