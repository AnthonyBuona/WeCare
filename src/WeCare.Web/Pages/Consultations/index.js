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
                    title: l('Actions'),
                    rowAction: {
                        items:
                            [
                                // Ações de Editar e Apagar podem ser adicionadas aqui posteriormente
                            ]
                    }
                },
                { title: l('Patient'), data: "patientName" },
                { title: l('Therapist'), data: "therapistName" },
                {
                    title: l('DateTime'),
                    data: "dateTime",
                    render: function (data) {
                        // Converte a data string (formato ISO) para o formato local do navegador
                        // Ex: "2025-07-10T14:30:00" -> "10/07/2025, 14:30:00"
                        if (!data) return '';
                        return new Date(data).toLocaleString();
                    }
                },
                { title: l('Description'), data: "description" },
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