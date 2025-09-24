// Usamos o padrão abp.widgets, que é o correto para a versão do framework
abp.widgets.CreateConsultationModal = function ($wrapper) {
    // Variáveis essenciais para o funcionamento do modal
    var widgetManager = $wrapper.data('abp-widget-manager');
    var modal = $wrapper.find('.modal');
    var form = $wrapper.find('form');
    var l = abp.localization.getResource('WeCare');
    var objectiveTrainings = [];

    // Serviços da aplicação que vamos usar
    var objectiveService = weCare.objectives.objective;
    var consultationService = weCare.consultations.consultation;

    // Instancia um gerenciador para o modal de "Criar Treino"
    var createTrainingModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'Trainings/CreateModal'
    });

    // Função para buscar os treinos de um objetivo específico na API
    function fetchTrainingsForObjective(objectiveId, callback) {
        objectiveTrainings = []; // Limpa a lista
        if (!objectiveId) {
            updateAllTrainingSelects();
            if (callback) callback();
            return;
        }
        objectiveService.getTrainingsForObjectiveAsync(objectiveId).done(function (result) {
            objectiveTrainings = result.items;
            updateAllTrainingSelects();
            if (callback) callback();
        });
    }

    // Função para popular um dropdown <select> com os treinos buscados
    function populateTrainingSelect(selectElement) {
        var $select = $(selectElement);
        var selectedValue = $select.val();
        $select.empty().append($('<option>', { value: '', text: 'Selecione um treino...' }));
        if (objectiveTrainings && objectiveTrainings.length > 0) {
            objectiveTrainings.forEach(function (training) {
                $select.append($('<option>', {
                    value: training.id,
                    text: training.displayName
                }));
            });
        }
        $select.val(selectedValue);
    }

    // Função para garantir que todos os dropdowns de treino na tela sejam atualizados
    function updateAllTrainingSelects() {
        form.find('#performed-trainings-container .training-select').each(function () {
            populateTrainingSelect(this);
        });
    }

    // Função que cria o HTML para um novo bloco de "Treino Realizado"
    function addTrainingItem(newlyCreatedTrainingId) {
        var container = form.find('#performed-trainings-container');
        var index = container.children('.training-item').length;

        var template = `
            <div class="border rounded p-3 mb-3 training-item position-relative">
                <button type="button" class="btn-close remove-training-button position-absolute top-0 end-0 m-2" aria-label="Close"></button>
                <div class="mb-3">
                    <label class="form-label required">${l('Atividade que foi realizada')}</label>
                    <div class="input-group">
                        <select name="Consultation.PerformedTrainings[${index}].TrainingId" class="form-select training-select" required></select>
                        <button class="btn btn-outline-primary add-new-training-button" type="button" title="${l('Adicionar novo tipo de treino')}"><i class="fa fa-plus"></i></button>
                    </div>
                </div>
                <div class="mb-3">
                    <label class="form-label">${l('Ajuda necessária')}</label>
                    <select name="Consultation.PerformedTrainings[${index}].HelpNeeded" class="form-select">
                        <option value="I">Independente</option>
                        <option value="SV">Suporte Verbal</option>
                        <option value="SG">Suporte Gestual</option>
                        <option value="SP">Suporte Posicional</option>
                        <option value="ST">Suporte Total</option>
                    </select>
                </div>
                <div class="row">
                    <div class="col">
                        <label class="form-label">${l('Número de tentativas')}</label>
                        <input type="number" class="form-control" value="5" name="Consultation.PerformedTrainings[${index}].TotalAttempts">
                    </div>
                    <div class="col">
                        <label class="form-label">${l('Tentativas bem sucedidas')}</label>
                        <input type="number" class="form-control" value="3" name="Consultation.PerformedTrainings[${index}].SuccessfulAttempts">
                    </div>
                </div>
            </div>`;

        container.append(template);
        populateTrainingSelect(container.find('.training-item:last .training-select')[0]);

        if (newlyCreatedTrainingId) {
            container.find('.training-item:last .training-select').val(newlyCreatedTrainingId);
        }
    }

    // Função para renumerar os campos do formulário quando um item é removido
    function reindexTrainingItems() {
        var index = 0;
        form.find('#performed-trainings-container .training-item').each(function () {
            $(this).find('[name]').each(function () {
                var newName = $(this).attr('name').replace(/\[\d+\]/, '[' + index + ']');
                $(this).attr('name', newName);
            });
            index++;
        });
    }

    // Função que inicializa todos os eventos de clique
    function init() {
        form.find('#Consultation_ObjectiveId').on('change', function () {
            fetchTrainingsForObjective($(this).val());
        });

        form.find('#add-training-button').click(function () {
            addTrainingItem();
        });

        form.find('#performed-trainings-container').on('click', '.remove-training-button', function () {
            $(this).closest('.training-item').remove();
            reindexTrainingItems();
        });

        form.find('#performed-trainings-container').on('click', '.add-new-training-button', function () {
            var objectiveId = form.find('#Consultation_ObjectiveId').val();
            if (!objectiveId) {
                abp.notify.warn('Por favor, selecione um objetivo primeiro.');
                return;
            }
            createTrainingModal.open({ objectiveId: objectiveId });
        });

        createTrainingModal.onResult(function (ajaxResult) {
            var createdTraining = ajaxResult.responseJSON;
            var objectiveId = form.find('#Consultation_ObjectiveId').val();

            fetchTrainingsForObjective(objectiveId, function () {
                addTrainingItem(createdTraining.id);
            });
        });

        // Garante que a tela sempre comece com pelo menos um item de treino
        if (form.find('#performed-trainings-container').children().length === 0) {
            addTrainingItem();
        }
        fetchTrainingsForObjective(form.find('#Consultation_ObjectiveId').val());
    }

    // O código de inicialização é chamado quando o modal é exibido
    modal.on('shown.bs.modal', function () {
        init();
    });

    // Lógica para salvar o formulário
    form.on('submit', function (e) {
        e.preventDefault();
        if (!form.valid()) {
            return;
        }
        var data = form.serializeFormToObject();

        if (!data.Consultation.PerformedTrainings) {
            data.Consultation.PerformedTrainings = [];
        }

        modal.find('.modal-footer .btn-primary').buttonBusy();

        consultationService.create(data.Consultation)
            .then(function (result) {
                modal.modal('hide');
                // A NOTIFICAÇÃO FOI REMOVIDA DAQUI
                abp.event.trigger('app.consultation.created', result); // Apenas dispara o evento
            }).always(function () {
                modal.find('.modal-footer .btn-primary').buttonIdle();
            });
    });
};
