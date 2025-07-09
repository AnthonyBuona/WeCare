(function ($) {
    abp.modals.ViewTreatmentsModal = function () {
        function initModal(modalManager, args) {
            console.log('initModal foi chamada! Argumentos recebidos:', args); // Para depuração
            var l = abp.localization.getResource('WeCare');
            var tratamentoService = weCare.tratamentos.tratamento;
            var patientId = args.patientId;

            var dataTable = $('#TreatmentsTable').DataTable(
                abp.libs.datatables.normalizeConfiguration({
                    serverSide: true,
                    paging: true,
                    order: [[0, "asc"]],
                    searching: false,
                    scrollX: true,
                    ajax: abp.libs.datatables.createAjax(function (request) {
                        console.log('Fazendo requisição para GetListByPatient com o ID:', patientId); // Para depuração
                        return tratamentoService.getListByPatient(patientId, request);
                    }),
                    columnDefs: [
                        { title: l('TipoTratamento'), data: "tipo" },
                        { title: l('Terapeuta'), data: "therapistName" },
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
        };

        return {
            initModal: initModal
        };
    };
})(jQuery);