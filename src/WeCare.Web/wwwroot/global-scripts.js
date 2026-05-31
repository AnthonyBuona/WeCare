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
