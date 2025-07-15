$(function () {
    var l = abp.localization.getResource('WeCare');

    // Extrai o PatientId da URL da página
    const patientId = patientIdForModal

    // Modal Manager para o modal de criar objetivo
    var createObjectiveModal = new abp.ModalManager({
        viewUrl: 'RealizedConsultations/CreateObjectiveModal'
    });

    // Função para carregar/recarregar a lista de objetivos
    function loadObjectives() {
        var container = $('#ObjectiveListContainer');
        container.html('<div class="d-flex justify-content-center"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>');
        abp.ajax({
            url: '/RealizedConsultations?handler=ObjectiveList&patientId=' + patientId,
            type: 'GET',
            dataType: 'html',
            success: function (content) {
                container.html(content);
            },
            error: function (e) {
                container.html('<div class="alert alert-danger">Ocorreu um erro ao carregar os objetivos.</div>');
            }
        });
    }

    // Evento de clique no botão "Adicionar novo objetivo"
    $('#NewObjectiveButton').click(function (e) {
        e.preventDefault();
        createObjectiveModal.open({ patientId: patientId });
    });

    // Evento disparado quando o modal de objetivo é fechado com sucesso
    createObjectiveModal.onResult(function () {
        // Recarrega a lista de objetivos na página principal
        abp.notify.info('Novo objetivo adicionado com sucesso!');
        loadObjectives();
    });

    // Carrega os objetivos pela primeira vez
    loadObjectives();
});