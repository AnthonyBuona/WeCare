$(function () {
    var l = abp.localization.getResource('WeCare');
    var createModal = new abp.ModalManager(abp.appPath + 'Pages/PeriodicReports/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Pages/PeriodicReports/EditModal');
    var viewModal = new abp.ModalManager(abp.appPath + 'Pages/PeriodicReports/ViewModal');

    var dataTable = $('#PeriodicReportsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[3, "desc"]], // Creation time/Date sorting
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(weCare.periodicReports.periodicReport.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: 'Visualizar Detalhes',
                                action: function (data) {
                                    viewModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: 'Editar Rascunho',
                                visible: abp.auth.isGranted('WeCare.PeriodicReports.Edit'),
                                action: function (data) {
                                    if (data.record.status === 0) { // Draft
                                        editModal.open({ id: data.record.id });
                                    } else {
                                        abp.message.warn('Apenas relatórios em rascunho podem ser editados.');
                                    }
                                }
                            },
                            {
                                text: 'Assinar e Publicar (Terapeuta)',
                                visible: abp.auth.isGranted('WeCare.PeriodicReports.Edit'),
                                action: function (data) {
                                    if (data.record.status === 0) { // Draft
                                        abp.message.confirm(
                                            'Deseja assinar digitalmente e publicar este relatório para os responsáveis?',
                                            'Assinatura Digital do Terapeuta',
                                            function (isConfirmed) {
                                                if (isConfirmed) {
                                                    weCare.periodicReports.periodicReport
                                                        .signAndPublish(data.record.id)
                                                        .then(function () {
                                                            abp.notify.success('Relatório assinado e publicado com sucesso!');
                                                            dataTable.ajax.reload();
                                                        });
                                                }
                                            }
                                        );
                                    } else {
                                        abp.message.warn('Apenas relatórios em rascunho podem ser publicados.');
                                    }
                                }
                            },
                            {
                                text: 'Dar Ciente Formal (Responsável)',
                                action: function (data) {
                                    if (data.record.status === 1) { // Published
                                        $('#SignatureReportId').val(data.record.id);
                                        $('#SignatureCpf').val('');
                                        $('#parentSignatureModal').modal('show');
                                    } else {
                                        abp.message.warn('Este relatório não está pendente de assinatura do responsável.');
                                    }
                                }
                            },
                            {
                                text: 'Download PDF Premium',
                                action: function (data) {
                                    if (data.record.status === 2) { // Signed
                                        abp.notify.info('Iniciando o download do PDF premium assinado...');
                                        // Simulação do download premium
                                        window.open('https://localhost:44373/images/logo/leptonxlite/logo-light.png', '_blank');
                                    } else {
                                        abp.message.warn('O PDF premium com selo digital está disponível apenas após a assinatura de ciente formal do responsável.');
                                    }
                                }
                            },
                            {
                                text: l('Delete'),
                                visible: abp.auth.isGranted('WeCare.PeriodicReports.Delete'),
                                confirmMessage: function (data) {
                                    return l('AreYouSureToDelete');
                                },
                                action: function (data) {
                                    if (data.record.status === 0) { // Draft
                                        weCare.periodicReports.periodicReport
                                            .delete(data.record.id)
                                            .then(function () {
                                                abp.notify.info(l('SuccessfullyDeleted'));
                                                dataTable.ajax.reload();
                                            });
                                    } else {
                                        abp.message.warn('Relatórios publicados ou assinados não podem ser excluídos por segurança jurídica.');
                                    }
                                }
                            }
                        ]
                    }
                },
                {
                    title: 'Título',
                    data: "title"
                },
                {
                    title: 'Paciente',
                    data: "patientName"
                },
                {
                    title: 'Terapeuta',
                    data: "therapistName"
                },
                {
                    title: 'Período Evolutivo',
                    data: "startDate",
                    render: function (data, type, row) {
                        var start = luxon.DateTime.fromISO(row.startDate).toFormat('dd/MM/yyyy');
                        var end = luxon.DateTime.fromISO(row.endDate).toFormat('dd/MM/yyyy');
                        return start + ' - ' + end;
                    }
                },
                {
                    title: 'Status',
                    data: "status",
                    render: function (data) {
                        var text = 'Rascunho';
                        if (data === 1) text = 'Publicado / Pendente Ciente';
                        if (data === 2) text = 'Assinado & Validado';
                        return '<span class="status-badge-' + data + '">' + text + '</span>';
                    }
                }
            ]
        })
    );

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#NewPeriodicReportButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    // Parent Signature submission
    $('#ParentSignatureForm').submit(function (e) {
        e.preventDefault();
        var id = $('#SignatureReportId').val();
        var cpf = $('#SignatureCpf').val();
        var ip = $('#SignatureIp').val();

        if (!cpf) {
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
                dataTable.ajax.reload();
            });
    });
});
