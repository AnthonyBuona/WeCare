$(function () {
    var l = abp.localization.getResource('WeCare');
    var createModal = new abp.ModalManager(abp.appPath + 'Consultations/CreateModal');

    var dataTable = $('#ConsultationsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[3, "desc"]], // Order by DateTime descending by default
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(weCare.consultations.consultation.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'),
                                iconClass: 'fa fa-pencil',
                                action: function (data) {
                                    // editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Delete'),
                                iconClass: 'fa fa-trash',
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
                {
                    title: l('Paciente'),
                    data: "patientName",
                    render: function (data) {
                        return '<span class="fw-bold text-dark">' + data + '</span>';
                    }
                },
                {
                    title: l('Terapeuta'),
                    data: "therapistName",
                    render: function (data) {
                        return '<span class="text-muted">' + data + '</span>';
                    }
                },
                {
                    title: l('Data/Hora'),
                    data: "dateTime",
                    render: function (data) {
                        if (!data) return '';
                        var date = new Date(data);
                        return '<div class="d-flex align-items-center">' +
                            '<div class="icon-shape icon-sm bg-light text-primary rounded-circle me-2" style="width:30px;height:30px;"><i class="fa fa-calendar"></i></div>' +
                            '<div>' + date.toLocaleDateString() + ' <small class="text-muted">' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) + '</small></div>' +
                            '</div>';
                    }
                },
                {
                    title: l('Descrição'),
                    data: "description",
                    render: function (data) {
                        return '<span class="text-truncate d-inline-block" style="max-width: 200px;">' + (data || '-') + '</span>';
                    }
                },
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
