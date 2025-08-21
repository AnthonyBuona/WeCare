alert("VERSÃO NOVA DO SCRIPT CARREGADA!"); 

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
        console.log("Modal de Criar Objetivo ABERTO. Tentando configurar os scripts...");

        // --- CORREÇÃO AQUI ---
        // Os seletores foram ajustados para corresponder aos IDs gerados pela <abp-form>
        // quando não se usa abp-model no form.
        var therapistSelect = $('#Objective_TherapistId'); // O ID correto é Objective_TherapistId
        var specialtyInput = $('#Objective_Specialty');   // O ID correto é Objective_Specialty

        if (therapistSelect.length === 0) {
            console.error("ERRO: O seletor de terapeuta '#Objective_TherapistId' não foi encontrado. Verifique o ID no HTML do modal.");
            return;
        }

        function updateSpecialty() {
            var selectedTherapistId = therapistSelect.val();

            if (selectedTherapistId) {
                var url = '/RealizedConsultations/CreateObjectiveModal?handler=Specialty&therapistId=' + selectedTherapistId;
                $.get(url, function (response) {
                    specialtyInput.val(response.specialty);
                });
            } else {
                specialtyInput.val('');
            }
        }

        therapistSelect.on('change', updateSpecialty);

        // Chama a função imediatamente para o caso de um valor já estar selecionado
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