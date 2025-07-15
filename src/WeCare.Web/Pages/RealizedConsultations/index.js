$(function () {
    var l = abp.localization.getResource('WeCare');

    var createConsultationModal = new abp.ModalManager(abp.appPath + 'RealizedConsultations/CreateModal');
    var createObjectiveModal = new abp.ModalManager(abp.appPath + 'RealizedConsultations/CreateObjectiveModal');

    var patientId = new URLSearchParams(window.location.search).get('patientId');
    var objectiveListContainer = $('#ObjectiveListContainer');

    // Função para carregar/recarregar a lista de objetivos
    function loadObjectives() {
        objectiveListContainer.html('<div class="d-flex justify-content-center"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>');

        // Chamada AJAX para o novo Page Handler
        abp.ajax({
            url: abp.appPath + 'RealizedConsultations?handler=ObjectiveList&patientId=' + patientId,
            type: 'GET',
            dataType: 'html',
            success: function (content) {
                objectiveListContainer.html(content);
            },
            error: function (e) {
                objectiveListContainer.html('<div class="alert alert-danger" role="alert">Erro ao carregar os objetivos.</div>');
            }
        });
    }

    // Gatilhos para os botões dos modais
    $('#NewConsultationButton').click(function (e) {
        e.preventDefault();
        createConsultationModal.open({ patientId: patientId });
    });

    $('#NewObjectiveButton').click(function (e) {
        e.preventDefault();
        createObjectiveModal.open({ patientId: patientId });
    });

    // Funções de callback que agora chamam a função de recarregar via AJAX
    createConsultationModal.onResult(function () {
        abp.notify.info('Consulta registrada com sucesso!');
        loadObjectives(); // Recarrega apenas a lista
    });

    createObjectiveModal.onResult(function () {
        abp.notify.info('Novo objetivo adicionado com sucesso!');
        loadObjectives(); // Recarrega apenas a lista
    });

    // Lógica de filtragem (permanece a mesma, mas agora usa 'delegate' para funcionar com conteúdo dinâmico)
    $(document).on('click', '.filter-buttons .btn', function (e) {
        e.preventDefault();
        var filter = $(this).data('filter');

        $('.filter-buttons .btn').removeClass('active');
        $(this).addClass('active');

        if (filter === 'all') {
            $('.objective-card, .consultation-item').show();
        } else {
            $('.objective-card').show();
            $('.consultation-item').hide();
            $('.consultation-item[data-specialization*="' + filter + '"]').show();
        }

        $('.objective-card').each(function () {
            var $card = $(this);
            if ($card.find('.consultation-item:visible').length === 0) {
                $card.hide();
            }
        });
    });

    // Carrega a lista de objetivos na primeira vez que a página é acessada
    loadObjectives();
});