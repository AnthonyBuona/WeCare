$(function () {
    var l = abp.localization.getResource('WeCare');
    var viewModal = new abp.ModalManager(abp.appPath + 'Pages/PeriodicReports/ViewModal');

    $('.ViewReportDetailsButton').click(function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        viewModal.open({ id: id });
    });

    $('.OpenSignatureModalButton').click(function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $('#SignatureReportId').val(id);
        $('#SignatureCpf').val('');
        $('#parentSignatureModal').modal('show');
    });

    $('.DownloadPdfButton').click(function (e) {
        e.preventDefault();
        abp.notify.info('Iniciando o download do PDF premium assinado...');
        // Simulação do download premium
        window.open('https://localhost:44373/images/logo/leptonxlite/logo-light.png', '_blank');
    });

    // Parent Signature submission
    $('#ParentSignatureForm').submit(function (e) {
        e.preventDefault();
        var id = $('#SignatureReportId').val();
        var cpf = $('#SignatureCpf').val();
        var ip = $('#SignatureIp').val();

        if (!cpf) {
            alert('Por favor, informe seu CPF.');
            $('#SignatureCpf').addClass('is-invalid');
            return;
        }

        weCare.periodicReports.periodicReport
            .parentSign(id, {
                responsibleSignatureCPF: cpf,
                responsibleSignatureIP: ip
            })
            .then(function () {
                $('#parentSignatureModal').modal('hide');
                abp.notify.success('Ciente formal registrado e hash criptográfico de validação gerado com sucesso!');
                setTimeout(function() {
                    window.location.reload();
                }, 1500);
            });
    });
});
