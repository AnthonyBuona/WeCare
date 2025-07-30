(function ($) {
    var l = abp.localization.getResource('WeCare');
    var _modalManager;
    var _$form;
    var trainingItemIndex = 0;

    // Função para adicionar um novo bloco de treino
    function addTrainingItem() {
        var container = $('#performed-trainings-container');

        var template = `
            <div class="border rounded p-3 mb-3 training-item">
                <div class="d-flex justify-content-end">
                    <button type="button" class="btn-close remove-training-button" aria-label="Close"></button>
                </div>
                <div class="mb-3">
                    <label class="form-label">${l('Atividade que foi realizada')}</label>
                    <select name="Consultation.PerformedTrainings[${trainingItemIndex}].Activity" class="form-select">
                        <option value="Atividade 1">Atividade 1</option>
                        <option value="Atividade 2">Atividade 2</option>
                        <option value="Atividade 3">Atividade 3</option>
                    </select>
                </div>
                <div class="mb-3">
                    <label class="form-label">${l('Ajuda necessária')}</label>
                    <select name="Consultation.PerformedTrainings[${trainingItemIndex}].HelpNeeded" class="form-select">
                        <option value="Nenhuma">Nenhuma</option>
                        <option value="Pouca">Pouca</option>
                        <option value="Muita">Muita</option>
                    </select>
                </div>
                <div class="row">
                    <div class="col">
                        <label for="TotalAttempts_${trainingItemIndex}" class="form-label">${l('Número de tentativas')}: <span id="totalAttemptsValue_${trainingItemIndex}">5</span></label>
                        <input type="range" class="form-range" min="0" max="10" id="TotalAttempts_${trainingItemIndex}" name="Consultation.PerformedTrainings[${trainingItemIndex}].TotalAttempts" value="5" oninput="$('#totalAttemptsValue_${trainingItemIndex}').text(this.value);">
                    </div>
                    <div class="col">
                        <label for="SuccessfulAttempts_${trainingItemIndex}" class="form-label">${l('Tentativas bem sucedidas')}: <span id="successfulAttemptsValue_${trainingItemIndex}">3</span></label>
                        <input type="range" class="form-range" min="0" max="10" id="SuccessfulAttempts_${trainingItemIndex}" name="Consultation.PerformedTrainings[${trainingItemIndex}].SuccessfulAttempts" value="3" oninput="$('#successfulAttemptsValue_${trainingItemIndex}').text(this.value);">
                    </div>
                </div>
            </div>`;

        container.append(template);
        trainingItemIndex++;
    }

    // Função para reinicializar os índices dos campos
    function reindexTrainingItems() {
        $('#performed-trainings-container .training-item').each(function (index, element) {
            $(element).find('select, input').each(function () {
                var name = $(this).attr('name');
                if (name) {
                    var newName = name.replace(/\[\d+\]/, '[' + index + ']');
                    $(this).attr('name', newName);
                }
            });
        });
        trainingItemIndex = $('#performed-trainings-container .training-item').length;
    }

    abp.modals.CreateConsultationModal = function () {
        function initModal(modalManager, args) {
            _modalManager = modalManager;
            _$form = _modalManager.getForm();

            // Adiciona o primeiro item de treino ao abrir o modal
            addTrainingItem();

            $('#add-training-button').click(function () {
                addTrainingItem();
            });

            $('#performed-trainings-container').on('click', '.remove-training-button', function () {
                $(this).closest('.training-item').remove();
                reindexTrainingItems();
            });

            $('#SaveConsultationButton').click(function (e) {
                e.preventDefault();
                if (!_$form.valid()) {
                    return;
                }

                var form = _$form.serializeFormToObject();

                // TODO: Chamar o serviço da aplicação aqui
                // weCare.realizedConsultations.consultation.create(form.Consultation).then(function(){
                //     _modalManager.close();
                //     abp.notify.info('Consulta registrada com sucesso!');
                //     // Atualizar a lista na página principal se necessário
                // });

                _modalManager.close(); // Fechar o modal por enquanto
            });
        };

        return {
            initModal: initModal
        };
    }
})(jQuery);