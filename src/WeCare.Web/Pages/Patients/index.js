$(function () {
    console.log('index.js carregado');
    var l = abp.localization.getResource('WeCare');

    var editModal = new abp.ModalManager(abp.appPath + 'Patients/EditModal');
    var createModal = new abp.ModalManager(abp.appPath + 'Patients/CreateModal');

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
                                // visible: abp.auth.isGranted('weCare.patients.Edit'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Apagar'),
                                // visible: abp.auth.isGranted('weCare.patients.Delete'),
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
                            text: l('Ver tratamentos'),
                            action: function (data) {
                                viewTreatmentsModal.open({ patientId: data.record.id });
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

    // Recarrega a tabela quando fechar o modal de criação
    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    // Recarrega a tabela quando fechar o modal de edição
    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    // Configura o botão de criar paciente
    var botao = document.getElementById('NewPatientButton');
    if (botao) {
        botao.addEventListener('click', function (e) {
            e.preventDefault();
            console.log('Botão clicado para criar paciente');
            createModal.open();
        });
    } else {
        console.warn('Botão NewPatientButton não foi encontrado na DOM.');
    }
});
