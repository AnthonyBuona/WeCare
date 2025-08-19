$(function () {
    var l = abp.localization.getResource('WeCare');

    // Extrai o PatientId da URL da página
    const patientId = patientIdForModal

    // Modal Manager para o modal de criar objetivo
    var createObjectiveModal = new abp.ModalManager({
        viewUrl: 'RealizedConsultations/CreateObjectiveModal'
    });

    var createConsultationModal = new abp.ModalManager({
        viewUrl: 'RealizedConsultations/CreateModal'
    });

    createObjectiveModal.onOpen(function () {
        // Seleciona os elementos DO MODAL que acabou de abrir
        var therapistSelect = $('#Objective_TherapistId');
        var specialtyInput = $('#Objective_Specialty');

        // Função para atualizar a especialidade
        function updateSpecialty() {
            var selectedTherapistId = therapistSelect.val();

            if (selectedTherapistId) {
                // Monta a URL para o handler do backend
                var url = '/RealizedConsultations/CreateObjectiveModal?handler=Specialty&therapistId=' + selectedTherapistId;

                // Faz a chamada AJAX para buscar a especialidade
                $.get(url, function (response) {
                    specialtyInput.val(response.specialty);
                });
            } else {
                // Limpa o campo se nenhum terapeuta for selecionado
                specialtyInput.val('');
            }
        }

        // Adiciona o evento 'change' ao dropdown de terapeutas
        therapistSelect.on('change', updateSpecialty);

        // Chama a função uma vez no carregamento para preencher o campo
        // caso um terapeuta já venha pré-selecionado (não é o caso aqui, mas é uma boa prática)
        updateSpecialty();
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

    $('#NewConsultationButton').click(function (e) {
        e.preventDefault();
        createConsultationModal.open({ patientId: patientId });
    });

    // Evento disparado quando o modal de objetivo é fechado com sucesso
    createObjectiveModal.onResult(function () {
        // Recarrega a lista de objetivos na página principal
        abp.notify.info('Novo objetivo adicionado com sucesso!');
        loadObjectives();
    });

    createConsultationModal.onResult(function () {
        // Recarrega a lista de objetivos na página principal
        abp.notify.info('Nova consulta registrada com sucesso!');
        loadObjectives();
    });

    // Carrega os objetivos pela primeira vez
    loadObjectives();
});