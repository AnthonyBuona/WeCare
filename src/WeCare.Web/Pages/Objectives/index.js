$(function () {
    var l = abp.localization.getResource('WeCare');
    var createModal = new abp.ModalManager(abp.appPath + 'Pages/Objectives/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Pages/Objectives/EditModal');

    var dataTable = $('#ObjectivesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(weCare.objectives.objective.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items:
                            [
                                {
                                    text: l('Edit'),
                                    visible: abp.auth.isGranted('WeCare.Objectives.Edit'),
                                    action: function (data) {
                                        editModal.open({ id: data.record.id });
                                    }
                                },
                                {
                                    text: l('Delete'),
                                    visible: abp.auth.isGranted('WeCare.Objectives.Delete'),
                                    confirmMessage: function (data) {
                                        return l('ObjectiveDeletionConfirmationMessage', data.record.name);
                                    },
                                    action: function (data) {
                                        weCare.objectives.objective
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
                    title: l('Status'),
                    data: "status",
                    render: function (data) {
                        return l('Enum:ObjectiveStatus:' + data) || data;
                    }
                },
                {
                    title: l('StartDate'),
                    data: "startDate",
                    render: function (data) {
                        return luxon
                            .DateTime
                            .fromISO(data, { locale: abp.localization.currentCulture.name })
                            .toLocaleString();
                    }
                },
                {
                    title: l('EndDate'),
                    data: "endDate",
                    render: function (data) {
                        if (!data) return "";
                        return luxon
                            .DateTime
                            .fromISO(data, { locale: abp.localization.currentCulture.name })
                            .toLocaleString();
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

    $('#NewObjectiveButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});
