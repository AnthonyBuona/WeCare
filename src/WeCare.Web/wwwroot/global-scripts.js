/* Your Global Scripts */

$(function () {
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
