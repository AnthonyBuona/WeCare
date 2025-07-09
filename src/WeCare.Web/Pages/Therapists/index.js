$(function () {
    var l = abp.localization.getResource('WeCare');

    var createModal = new abp.ModalManager(abp.appPath + 'Therapists/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Therapists/EditModal');

    var dataTable = $('#TherapistsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(weCare.therapists.therapist.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'),
                                visible: abp.auth.isGranted('WeCare.Therapists.Edit'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Delete'),
                                visible: abp.auth.isGranted('WeCare.Therapists.Delete'),
                                confirmMessage: function (data) {
                                    return l('TherapistDeletionConfirmationMessage', data.record.name);
                                },
                                action: function (data) {
                                    weCare.therapists.therapist
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
                { title: l('Name'), data: "name" },
                { title: l('Email'), data: "email" },
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

    $('#NewTherapistButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});