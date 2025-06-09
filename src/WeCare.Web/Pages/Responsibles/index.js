$(function () {
    console.log('index.js carregado');
    var l = abp.localization.getResource('WeCare');

    var editModal = new abp.ModalManager(abp.appPath + 'Responsibles/EditModal');
    var createModal = new abp.ModalManager(abp.appPath + 'Responsibles/CreateModal');

    var dataTable = $('#ResponsiblesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(weCare.responsibles.responsible.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Editar'),
                                // visible: abp.auth.isGranted('weCare.Responsibles.Edit'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Apagar'),
                                // visible: abp.auth.isGranted('weCare.Responsibles.Delete'),
                                confirmMessage: function (data) {
                                    return l('Essa ação não pode ser revertida', data.record.name);
                                },
                                action: function (data) {
                                    weCare.responsibles.responsible
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
                { title: l('Name'), data: "nameResponsible" },
                { title: l('EmailAdress'), data: "emailAddress" },
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
    var botao = document.getElementById('NewResponsibleButton');
    if (botao) {
        botao.addEventListener('click', function (e) {
            e.preventDefault();
            console.log('Botão clicado para criar paciente');
            createModal.open();
        });
    } else {
        console.warn('Botão NewResponsibleButton não foi encontrado na DOM.');
    }
});
