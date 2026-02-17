$(function () {
    var l = abp.localization.getResource('WeCare');
    var createModal = new abp.ModalManager(abp.appPath + 'Clinics/CreateModal');

    var dataTable = $('#ClinicsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(weCare.clinics.clinicManagement.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items:
                            [
                                {
                                    text: l('Edit'),  // TODO: Add Edit Modal
                                    visible: abp.auth.isGranted('WeCare.Clinics.Edit'),
                                    action: function (data) {
                                        // editModal.open({ id: data.record.id });
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
                    title: l('CNPJ'),
                    data: "cnpj"
                },
                {
                    title: l('Email'),
                    data: "email"
                },
                {
                    title: l('Status'),
                    data: "status",
                    render: function (data) {
                        return l('Enum:ClinicStatus.' + data);
                    }
                }
            ]
        })
    );

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#NewClinicButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});
