$(function () {
    var l = abp.localization.getResource('WeCare');

    // 1. O patientId é pego da URL da página. Ele é a referência principal.
    const urlParams = new URLSearchParams(window.location.search);
    const patientId = urlParams.get('patientId');

    // 2. CONFIGURAÇÃO DOS MODAIS
    // Modal principal para registrar uma consulta/sessão.
    var createConsultationModal = new abp.ModalManager({
        viewUrl: '/RealizedConsultations/CreateModal'
    });

    // Modal secundário para criar um novo objetivo, chamado a partir do modal de consulta.
    var createObjectiveModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'RealizedConsultations/CreateObjectiveModal',
        scriptUrl: abp.appPath + 'Pages/RealizedConsultations/createObjectiveModal.js', // Adicione esta linha
        modalClass: 'objectiveCreate'
    });


    // 3. FUNÇÃO PARA CARREGAR A LISTA DE OBJETIVOS E CONSULTAS
    // Esta função é chamada sempre que a página precisa ser atualizada.
    function loadObjectives() {
        var container = $('#ObjectiveListContainer');
        if (container.length === 0 || !patientId) return;

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


    // 4. EVENTOS E MANIPULADORES (HANDLERS)

    // --- Lógica do Modal de Registrar Consulta ---

    // Quando o botão principal "Registrar última consulta" é clicado.
    $('#NewConsultationButton').click(function (e) {
        e.preventDefault();
        // Abre o modal passando o patientId para ele carregar os dados corretos (ex: lista de objetivos).
        createConsultationModal.open({ patientId: patientId });
    });

    // Evento disparado QUANDO o modal de consulta é aberto.
    createConsultationModal.onOpen(function () {
        var modal = createConsultationModal.getModal();

        // Procura o botão "+ Novo Objetivo" DENTRO do modal de consulta e anexa um evento de clique.
        modal.find('#NewObjectiveButton').on('click', function (e) {
            e.preventDefault();
            // Abre o modal de criar objetivo.
            createObjectiveModal.open({ patientId: patientId });
        });
    });

    // Quando o modal de consulta é fechado com SUCESSO.
    createConsultationModal.onResult(function () {
        abp.notify.info('Nova consulta registrada com sucesso!');
        loadObjectives(); // Apenas recarrega a lista na página principal.
    });


    // --- Lógica do Modal de Novo Objetivo ---

    // Evento disparado QUANDO o modal de criar objetivo é aberto.
    createObjectiveModal.onOpen(function () {
        var therapistSelect = $('#Objective_TherapistId');
        var specialtyInput = $('#Objective_Specialty');

        if (therapistSelect.length === 0) {
            console.error("ERRO: O seletor de terapeuta '#Objective_TherapistId' não foi encontrado.");
            return;
        }

        function updateSpecialty() {
            var selectedTherapistId = therapistSelect.val();
            if (selectedTherapistId) {
                // Chama o handler no PageModel para buscar a especialidade do terapeuta.
                var url = '/RealizedConsultations/CreateObjectiveModal?handler=Specialty&therapistId=' + selectedTherapistId;
                $.get(url, function (response) {
                    specialtyInput.val(response.specialty);
                });
            } else {
                specialtyInput.val('');
            }
        }

        therapistSelect.on('change', updateSpecialty);
        updateSpecialty(); // Chama uma vez para preencher o campo se já houver um terapeuta selecionado.
    });

    // Quando o modal de criar objetivo é fechado com SUCESSO.
    createObjectiveModal.onResult(function () {
        abp.notify.info('Novo objetivo criado com sucesso!');

        // ** O FLUXO INTELIGENTE ACONTECE AQUI **
        // 1. Fecha o modal de consulta que estava aberto no fundo.
        createConsultationModal.close();

        // 2. Espera um instante e abre o modal de consulta novamente.
        // Isso força o modal a recarregar seus dados (o OnGetAsync), fazendo com que
        // o novo objetivo apareça na lista do dropdown.
        setTimeout(function () {
            createConsultationModal.open({ patientId: patientId });
        }, 500); // Meio segundo de espera para garantir que tudo seja processado.
    });


    // 5. CARGA INICIAL
    // Chama a função para carregar a lista de objetivos assim que a página é aberta.
    loadObjectives();
});