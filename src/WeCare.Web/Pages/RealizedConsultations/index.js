$(function () {
    var l = abp.localization.getResource('WeCare');

    // --- Gerenciadores de modais ---
    // Modal para registrar consulta em um objetivo que já existe
    var createConsultationModal = new abp.ModalManager(abp.appPath + 'RealizedConsultations/CreateModal');
    // Modal para criar um novo objetivo
    var createObjectiveModal = new abp.ModalManager(abp.appPath + 'RealizedConsultations/CreateObjectiveModal');

    // Pega o ID do paciente da URL
    var patientId = new URLSearchParams(window.location.search).get('patientId');

    // --- Gatilhos para os botões dos modais ---
    $('#NewConsultationButton').click(function (e) {
        e.preventDefault();
        // Este modal ainda não tem uma função clara no novo layout, mas mantemos o código
        createConsultationModal.open({ patientId: patientId });
    });

    // Esta é a parte que faltava: o gatilho para o botão "Adicionar novo objetivo"
    $('#NewObjectiveButton').click(function (e) {
        e.preventDefault();
        createObjectiveModal.open({ patientId: patientId });
    });

    // --- Funções de callback (o que acontece depois que o modal fecha com sucesso) ---
    createConsultationModal.onResult(function () {
        location.reload(); // Recarrega a página para mostrar a nova consulta
    });

    createObjectiveModal.onResult(function () {
        location.reload(); // Recarrega a página para mostrar o novo objetivo
    });

    // --- LÓGICA DE FILTRAGEM ---
    $('.filter-buttons .btn').click(function (e) {
        e.preventDefault();
        var filter = $(this).data('filter');

        // Atualiza o estado visual (classe 'active') dos botões de filtro
        $('.filter-buttons .btn').removeClass('active');
        $(this).addClass('active');

        // Mostra todos os cards de objetivo antes de filtrar os itens internos
        $('.objective-card').show();

        if (filter === 'all') {
            // Se o filtro for 'todos', mostra todos os itens de consulta
            $('.consultation-item').show();
        } else {
            // Primeiro, esconde todos os itens de consulta
            $('.consultation-item').hide();
            // Depois, mostra apenas os itens que correspondem ao filtro
            $('.consultation-item[data-specialization="' + filter + '"]').show();
        }

        // Após filtrar, verifica se algum card de objetivo ficou sem nenhum item visível
        $('.objective-card').each(function () {
            var $card = $(this);
            // Se não houver nenhum .consultation-item visível dentro deste card...
            if ($card.find('.consultation-item:visible').length === 0) {
                $card.hide(); // ...esconde o card inteiro.
            }
        });
    });
});