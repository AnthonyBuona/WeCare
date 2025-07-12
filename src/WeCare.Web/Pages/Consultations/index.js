$(function () {
    var l = abp.localization.getResource('WeCare');
    var createModal = new abp.ModalManager(abp.appPath + 'Consultations/CreateModal');

    var dataTable = $('#ConsultationsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(weCare.consultations.consultation.getList),
            columnDefs: [
                {
                    title: l('Ações'),
                    rowAction: {
                        items: [
                            {
                                text: l('Editar'), // Example Edit Action
                                action: function (data) {
                                    // Add your edit logic here, e.g.:
                                    // editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Apagar'), // Example Delete Action
                                confirmMessage: function (data) {
                                    return l('ConsultationDeletionConfirmationMessage', data.record.patientName);
                                },
                                action: function (data) {
                                    weCare.consultations.consultation
                                        .delete(data.record.id)
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
                {
                    title: l('Data/hora'),
                    data: "dateTime",
                    render: function (data) {
                        if (!data) return '';
                        return new Date(data).toLocaleString();
                    }
                },
                { title: l('Descrição'), data: "description" },
            ]
        })
    );

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#NewConsultationButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});