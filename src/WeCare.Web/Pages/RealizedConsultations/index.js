$(function () {
    var l = abp.localization.getResource('WeCare');

    const urlParams = new URLSearchParams(window.location.search);
    const patientId = urlParams.get('patientId');

    // --- 1. CONFIGURAÇÃO DOS MODAIS ---
    // Todos os modais que a página pode abrir são definidos aqui.

    var createConsultationModal = new abp.ModalManager({
        viewUrl: '/RealizedConsultations/CreateModal',
        scriptUrl: '/Pages/RealizedConsultations/createModal.js' // Garante que o script do modal seja carregado
    });

    var createObjectiveModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'RealizedConsultations/CreateObjectiveModal',
        scriptUrl: abp.appPath + 'Pages/RealizedConsultations/createObjectiveModal.js',
        modalClass: 'objectiveCreate'
    });

    // NOVO: Adicionamos a definição do modal de criar treino aqui.
    var createTrainingModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'Trainings/CreateModal'
        // Não precisa de scriptUrl se o modal for simples (só HTML e post)
    });


    // --- 2. FUNÇÃO PARA CARREGAR CONTEÚDO DA PÁGINA ---
    function loadObjectives() {
        var container = $('#ObjectiveListContainer');
        if (container.length === 0 || !patientId) return;

        container.html('<div class="d-flex justify-content-center p-5"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>');

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

    // --- 3. EVENTOS E MANIPULADORES ---

    // Abre o modal principal para registrar uma consulta
    $('#NewConsultationButton').click(function (e) {
        e.preventDefault();
        createConsultationModal.open({ patientId: patientId });
    });

    // Anexa os eventos DEPOIS que o modal de consulta for aberto
    createConsultationModal.onOpen(function () {
        var modal = createConsultationModal.getModal();

        // Evento para o botão "+ Novo Objetivo"
        modal.find('#NewObjectiveButton').on('click', function (e) {
            e.preventDefault();
            createObjectiveModal.open({ patientId: patientId });
        });

        // Evento para o botão "+ Novo Treino" (delegado, pois os botões são criados dinamicamente)
        modal.find('#performed-trainings-container').on('click', '.add-new-training-button', function () {
            var objectiveId = modal.find('#Consultation_ObjectiveId').val();
            if (!objectiveId) {
                abp.notify.warn('Por favor, selecione um objetivo primeiro.');
                return;
            }
            createTrainingModal.open({ objectiveId: objectiveId });
        });
    });

    // Ouve o evento global que o createModal.js dispara quando uma consulta é salva
    abp.event.on('app.consultation.created', function () {
        abp.notify.success('Nova consulta registrada com sucesso!');
        loadObjectives(); // Recarrega a lista na página principal.
    });

    // Lógica quando um NOVO OBJETIVO é criado com sucesso
    createObjectiveModal.onResult(function () {
        abp.notify.info('Novo objetivo criado com sucesso!');
        // Fecha e reabre o modal de consulta para atualizar a lista de objetivos
        createConsultationModal.close();
        setTimeout(function () {
            createConsultationModal.open({ patientId: patientId });
        }, 500);
    });

    // Lógica quando um NOVO TREINO é criado com sucesso
    createTrainingModal.onResult(function (ajaxResult) {
        abp.notify.info('Novo treino criado com sucesso!');
        // Dispara um evento personalizado para que o createModal.js possa ouvi-lo
        abp.event.trigger('app.training.created', ajaxResult.responseJSON);
    });

    // --- 4. CARGA INICIAL ---
    loadObjectives();
});
