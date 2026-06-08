$(function () {
    var l = abp.localization.getResource('WeCare');
    var createModal = new abp.ModalManager(abp.appPath + 'Trainings/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Trainings/EditModal');

    var dataTable = $('#TrainingsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(weCare.trainings.training.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items:
                            [
                                {
                                    text: l('Edit'),
                                    visible: abp.auth.isGranted('WeCare.Trainings.Edit'),
                                    action: function (data) {
                                        editModal.open({ id: data.record.id });
                                    }
                                },
                                {
                                    text: l('Delete'),
                                    visible: abp.auth.isGranted('WeCare.Trainings.Delete'),
                                    confirmMessage: function (data) {
                                        return l('TrainingDeletionConfirmationMessage', data.record.name);
                                    },
                                    action: function (data) {
                                        weCare.trainings.training
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
                    title: l('Description'),
                    data: "description"
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

    $('#NewTrainingButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});
