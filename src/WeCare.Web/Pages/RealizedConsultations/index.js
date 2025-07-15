$(function () {
    var l = abp.localization.getResource('WeCare');

    // Extrai o PatientId da URL da página
    const urlParams = new URLSearchParams(window.location.search);
    const patientId = urlParams.get('PatientId');

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

    // =================================================================
    // =========== INÍCIO DO CÓDIGO DE CORREÇÃO ADICIONADO ============
    // =================================================================
    // Este código é executado sempre que o modal de criar objetivo for aberto e estiver pronto
    createObjectiveModal.onOpen(function () {
        // Encontra o formulário e o botão Salvar dentro do modal que acabou de abrir
        var modal = createObjectiveModal.getModal();
        var form = modal.find('form');
        var saveButton = modal.find('.modal-footer .btn-primary'); // Encontra o botão "Save"

        // Remove quaisquer "ouvintes" de clique antigos para evitar duplicação
        saveButton.off('click');

        // Adiciona o nosso próprio "ouvinte" de clique
        saveButton.on('click', function (e) {
            e.preventDefault();
            // Dispara manualmente o evento de submissão do formulário do ABP
            form.submit();
        });
    });
    // =================================================================
    // ============= FIM DO CÓDIGO DE CORREÇÃO ADICIONADO ==============
    // =================================================================

    // Evento disparado quando o modal de objetivo é fechado com sucesso
    createObjectiveModal.onResult(function () {
        // Recarrega a lista de objetivos na página principal
        abp.notify.info('Novo objetivo adicionado com sucesso!');
        loadObjectives();
    });

    // Carrega os objetivos pela primeira vez
    loadObjectives();
});