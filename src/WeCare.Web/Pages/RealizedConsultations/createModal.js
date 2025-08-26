(function ($) {
    abp.ModalManager.extend(function (modalManager) {

        function CreateConsultationModal() {
            var _modalManager;
            var _$form;
            var l = abp.localization.getResource('WeCare');
            var trainingItemIndex = 0;
            var objectiveTrainings = [];
            var objectiveService = weCare.objectives.objective;
            var consultationService = weCare.consultations.consultation;

            this.init = function (modalManager) {
                _modalManager = modalManager;
                _$form = _modalManager.getForm();

                function fetchTrainingsForObjective(objectiveId) {
                    if (!objectiveId) {
                        objectiveTrainings = [];
                        updateAllTrainingSelects();
                        return;
                    }
                    objectiveService.getTrainingsForObjective(objectiveId).done(function (result) {
                        objectiveTrainings = result.items;
                        updateAllTrainingSelects();
                    });
                }

                function populateTrainingSelect(selectElement) {
                    var $select = $(selectElement);
                    $select.empty().append($('<option>', { value: '', text: 'Selecione um treino...' }));
                    if (objectiveTrainings && objectiveTrainings.length > 0) {
                        objectiveTrainings.forEach(function (training) {
                            $select.append($('<option>', {
                                value: training.id,
                                text: training.displayName
                            }));
                        });
                    }
                }

                function updateAllTrainingSelects() {
                    $('#performed-trainings-container .training-select').each(function () {
                        populateTrainingSelect(this);
                    });
                }

                function addTrainingItem() {
                    var container = $('#performed-trainings-container');
                    var template = `
                        <div class="border rounded p-3 mb-3 training-item">
                            <div class="d-flex justify-content-end">
                                <button type="button" class="btn-close remove-training-button" aria-label="Close"></button>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">${l('Atividade que foi realizada')}</label>
                                <select name="Consultation.PerformedTrainings[${trainingItemIndex}].TrainingId" class="form-select training-select" required></select>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">${l('Ajuda necessária')}</label>
                                <select name="Consultation.PerformedTrainings[${trainingItemIndex}].HelpNeeded" class="form-select">
                                    <option value="Total">Total</option>
                                    <option value="Parcial">Parcial</option>
                                    <option value="Nenhuma">Nenhuma</option>
                                </select>
                            </div>
                            <div class="row">
                                <div class="col">
                                    <label for="TotalAttempts_${trainingItemIndex}" class="form-label">${l('Número de tentativas')}: <span id="totalAttemptsValue_${trainingItemIndex}">5</span></label>
                                    <input type="range" class="form-range" min="0" max="10" value="5" id="TotalAttempts_${trainingItemIndex}" name="Consultation.PerformedTrainings[${trainingItemIndex}].TotalAttempts" oninput="$('#totalAttemptsValue_${trainingItemIndex}').text(this.value);">
                                </div>
                                <div class="col">
                                    <label for="SuccessfulAttempts_${trainingItemIndex}" class="form-label">${l('Tentativas bem sucedidas')}: <span id="successfulAttemptsValue_${trainingItemIndex}">3</span></label>
                                    <input type="range" class="form-range" min="0" max="10" value="3" id="SuccessfulAttempts_${trainingItemIndex}" name="Consultation.PerformedTrainings[${trainingItemIndex}].SuccessfulAttempts" oninput="$('#successfulAttemptsValue_${trainingItemIndex}').text(this.value);">
                                </div>
                            </div>
                        </div>`;
                    container.append(template);
                    var newSelect = container.find('.training-item:last .training-select');
                    populateTrainingSelect(newSelect[0]);
                    trainingItemIndex++;
                }

                function reindexTrainingItems() {
                    var index = 0;
                    $('#performed-trainings-container .training-item').each(function () {
                        $(this).find('select, input').each(function () {
                            var name = $(this).attr('name');
                            if (name) {
                                var newName = name.replace(/\[\d+\]/, '[' + index + ']');
                                $(this).attr('name', newName);
                            }
                        });
                        index++;
                    });
                    trainingItemIndex = index;
                }

                $('#Consultation_ObjectiveId').on('change', function () {
                    fetchTrainingsForObjective($(this).val());
                });
                $('#add-training-button').click(addTrainingItem);

                $('#performed-trainings-container').on('click', '.remove-training-button', function () {
                    $(this).closest('.training-item').remove();
                    reindexTrainingItems();
                });

                addTrainingItem();
                $('#Consultation_ObjectiveId').trigger('change');
            };

            this.save = function () {
                if (!_$form.valid()) {
                    return;
                }
                var formData = _$form.serializeFormToObject();
                consultationService.create(formData.Consultation)
                    .then(function (result) {
                        _modalManager.close();
                        abp.notify.success('Consulta registrada com sucesso!');
                        abp.event.trigger('app.consultation.created', result);
                    });
            };
        }

        return new CreateConsultationModal();
    });
})(jQuery);