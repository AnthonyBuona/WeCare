$(function () {
    var l = abp.localization.getResource('WeCare');
    var createModal = new abp.ModalManager(abp.appPath + 'Tratamentos/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Tratamentos/EditModal');

    // Assumindo que o serviço gerado pelo ABP CLI seja weCare.consultas.Tratamento
    var Tratamentoservice = weCare.consultas.Tratamento;

    var dataTable = $('#TratamentosTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(Tratamentoservice.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'),
                                // visible: abp.auth.isGranted('WeCare.Tratamentos.Edit'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Delete'),
                                // visible: abp.auth.isGranted('WeCare.Tratamentos.Delete'),
                                confirmMessage: function (data) {
                                    return l('DeletionConfirmationMessage', data.record.tipoConsulta);
                                },
                                action: function (data) {
                                    Tratamentoservice.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l('SuccessfullyDeleted'));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    }
                },
                { title: l('TipoConsulta'), data: "tipoConsulta" },
                // Adicionar colunas para Patient e Therapist se necessário
                // { title: l('Patient'), data: "patient.name" }, // Exemplo
                // { title: l('Therapist'), data: "therapist.name" }, // Exemplo
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

    $('#NewTratamentoButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});