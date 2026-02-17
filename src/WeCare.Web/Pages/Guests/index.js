$(function () {
    var l = abp.localization.getResource('WeCare');
    var createModal = new abp.ModalManager(abp.appPath + 'Guests/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Guests/EditModal'); // Todo: Implement EditModal later

    var dataTable = $('#GuestsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(weCare.guests.guest.getList), // Check this namespace
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items:
                            [
                                {
                                    text: l('Edit'),
                                    visible: abp.auth.isGranted('WeCare.Guests.Edit'),
                                    action: function (data) {
                                        // createModal.open({ id: data.record.id }); 
                                        // Use editModal when implemented
                                        abp.message.info(l('NotImplementedYet'));
                                    }
                                },
                                {
                                    text: l('Delete'),
                                    visible: abp.auth.isGranted('WeCare.Guests.Delete'),
                                    confirmMessage: function (data) {
                                        return l('GuestDeletionConfirmationMessage', data.record.name);
                                    },
                                    action: function (data) {
                                        weCare.guests.guest
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
                    title: l('Name'),
                    data: "name"
                },
                {
                    title: l('Email'),
                    data: "email"
                },
                {
                    title: l('Relationship'),
                    data: "relationship"
                },
                {
                    title: l('Patient'),
                    data: "patientName"
                }
            ]
        })
    );

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#NewGuestButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});
