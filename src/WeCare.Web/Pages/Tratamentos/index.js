$(function () {
    var l = abp.localization.getResource('WeCare');

    // Caminho corrigido para o modal de criação
    var createModal = new abp.ModalManager(abp.appPath + 'Tratamentos/CreateModal');

    // O modal de edição também precisará ser corrigido
    var editModal = new abp.ModalManager(abp.appPath + 'Tratamentos/EditModal');

    // Namespace corrigido do serviço da aplicação
    var tratamentoService = weCare.tratamentos.tratamento;

    var dataTable = $('#TratamentosTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(tratamentoService.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'),
                                visible: abp.auth.isGranted('WeCare.Tratamentos.Edit'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Delete'),
                                visible: abp.auth.isGranted('WeCare.Tratamentos.Delete'),
                                confirmMessage: function (data) {
                                    return l('TratamentoDeletionConfirmationMessage', data.record.tipo);
                                },
                                action: function (data) {
                                    tratamentoService.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l('SuccessfullyDeleted'));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    }
                },
                { title: l('Paciente'), data: "patientName" },
                { title: l('Terapeuta'), data: "therapistName" },
                { title: l('TipoTratamento'), data: "tipo" },
                {
                    title: l('CreationTime'),
                    data: "creationTime",
                    render: function (data) {
                        return luxon.DateTime.fromISO(data, {
                            locale: abp.localization.currentCulture.name
                        }).toLocaleString(luxon.DateTime.DATETIME_SHORT);
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

    // CORREÇÃO PRINCIPAL: O ID do botão deve ser "NewTratamentoButton"
    $('#NewTratamentoButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});