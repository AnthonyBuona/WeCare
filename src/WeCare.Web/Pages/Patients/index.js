$(function () {
    var l = abp.localization.getResource('WeCare');

    var editModal = new abp.ModalManager(abp.appPath + 'Patients/EditModal');
    var createModal = new abp.ModalManager(abp.appPath + 'Patients/CreateModal');

    var viewTreatmentsModal = new abp.ModalManager(abp.appPath + 'Patients/ViewTreatmentsModal');

    var dataTable = $('#PatientsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(weCare.patients.patient.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Editar'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Apagar'),
                                confirmMessage: function (data) {
                                    return l('Essa ação não pode ser revertida', data.record.name);
                                },
                                action: function (data) {
                                    weCare.patients.patient
                                        .delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l('SuccessfullyDeleted'));
                                            dataTable.ajax.reload();
                                        });
                                }
                            },
                            {
                                text: l('Ver tratamentos'),
                                action: function (data) {
                                    viewTreatmentsModal.open({ patientId: data.record.id });
                                }
                            }
                        ]
                    }
                },
                { title: l('Name'), data: "name" },
                { title: l('BirthDate'), data: "birthDate", dataFormat: "date" },
                { title: l('Address'), data: "address" },
                { title: l('Diag'), data: "diag" },
                {
                    title: l('Data de criação'),
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

    $('#NewPatientButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});