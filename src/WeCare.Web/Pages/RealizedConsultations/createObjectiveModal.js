(function ($) {
    abp.ModalManager.extend(function (modalManager) {

        function CreateObjectiveModal() {
            var _modalManager;
            var _objectiveAppService = weCare.objectives.objective;
            var _$form;

            this.init = function (modalManager) {
                _modalManager = modalManager;
                _$form = _modalManager.getForm();

                var therapistSelect = _$form.find('#Objective_TherapistId');
                var specialtyInput = _$form.find('#Objective_Specialty');

                if (therapistSelect.length === 0) {
                    console.error("ERRO: O seletor de terapeuta '#Objective_TherapistId' não foi encontrado.");
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
                updateSpecialty();
            };

            this.save = function () {
                if (!_$form.valid()) {
                    return;
                }

                var objective = _$form.serializeFormToObject().Objective;

                _objectiveAppService.create(objective)
                    .done(function () {
                        _modalManager.close();
                        abp.notify.success('Objetivo criado com sucesso!');
                        abp.event.trigger('app.objective.created');
                    });
            };
        }

        return new CreateObjectiveModal();
    });
})(jQuery);