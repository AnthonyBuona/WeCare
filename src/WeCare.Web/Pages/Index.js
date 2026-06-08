$(function () {
    var l = abp.localization.getResource('WeCare');
    var viewModal = new abp.ModalManager(abp.appPath + 'Pages/PeriodicReports/ViewModal');

    $('.ViewReportDetailsButton').click(function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        viewModal.open({ id: id });
    });

    $('.OpenSignatureModalButton').click(function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $('#SignatureReportId').val(id);
        $('#SignatureCpf').val('');
        $('#parentSignatureModal').modal('show');
    });

    $('.DownloadPdfButton').click(function (e) {
        e.preventDefault();
        abp.notify.info('Iniciando o download do PDF premium assinado...');
        // Simulação do download premium
        window.open('https://localhost:44373/images/logo/leptonxlite/logo-light.png', '_blank');
    });

    // Parent Signature submission
    $('#ParentSignatureForm').submit(function (e) {
        e.preventDefault();
        var id = $('#SignatureReportId').val();
        var cpf = $('#SignatureCpf').val();
        var ip = $('#SignatureIp').val();

        if (!cpf) {
            alert('Por favor, informe seu CPF.');
            $('#SignatureCpf').addClass('is-invalid');
            return;
        }

        weCare.periodicReports.periodicReport
            .parentSign(id, {
                responsibleSignatureCPF: cpf,
                responsibleSignatureIP: ip
            })
            .then(function () {
                $('#parentSignatureModal').modal('hide');
                abp.notify.success('Ciente formal registrado e hash criptográfico de validação gerado com sucesso!');
                setTimeout(function() {
                    window.location.reload();
                }, 1500);
            });
    });

    // --- Onboarding Wizard (E4) Logic ---
    var currentStep = 1;
    var totalSteps = 3;

    function updateWizard() {
        // Update progress bar
        var progressPercent = ((currentStep - 1) / (totalSteps - 1)) * 100;
        $('#wizard-progress-bar').css('width', progressPercent + '%');

        // Update steps visual nodes
        $('.wizard-step-node').each(function () {
            var step = $(this).data('step');
            var numNode = $(this).find('.step-num');
            var textNode = $(this).find('span');

            if (step <= currentStep) {
                $(this).addClass('active');
                numNode.css({
                    'background-color': 'var(--color-mint-teal-base)',
                    'border-color': 'var(--color-mint-teal-base)',
                    'color': '#fff'
                });
                textNode.removeClass('text-muted').addClass('text-dark').css('font-weight', 'bold');
            } else {
                $(this).removeClass('active');
                numNode.css({
                    'background-color': '#fff',
                    'border-color': '#dee2e6',
                    'color': '#6c757d'
                });
                textNode.removeClass('text-dark').addClass('text-muted').css('font-weight', 'normal');
            }
        });

        // Show/hide panels
        $('.wizard-panel').addClass('d-none');
        $('.wizard-panel[data-step="' + currentStep + '"]').removeClass('d-none');

        // Update buttons
        if (currentStep === 1) {
            $('#btn-prev').addClass('d-none');
            $('#btn-next').removeClass('d-none');
            $('#btn-submit').addClass('d-none');
        } else if (currentStep === totalSteps) {
            $('#btn-prev').removeClass('d-none');
            $('#btn-next').addClass('d-none');
            $('#btn-submit').removeClass('d-none');
        } else {
            $('#btn-prev').removeClass('d-none');
            $('#btn-next').removeClass('d-none');
            $('#btn-submit').addClass('d-none');
        }
    }

    function validateStep(step) {
        var valid = true;
        $('.wizard-panel[data-step="' + step + '"] [required]').each(function () {
            if (!$(this).val() || $(this).val().trim() === '') {
                $(this).addClass('is-invalid');
                valid = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        return valid;
    }

    $('#btn-next').click(function () {
        if (validateStep(currentStep)) {
            if (currentStep < totalSteps) {
                currentStep++;
                updateWizard();
            }
        } else {
            abp.notify.warn('Por favor, preencha todos os campos obrigatórios deste passo antes de prosseguir.');
        }
    });

    $('#btn-prev').click(function () {
        if (currentStep > 1) {
            currentStep--;
            updateWizard();
        }
    });

    // Form submission validation
    $('#onboardingForm').submit(function (e) {
        if (!validateStep(3)) {
            e.preventDefault();
            abp.notify.warn('Por favor, preencha todos os campos obrigatórios antes de finalizar.');
        } else {
            abp.ui.setBusy('#onboardingForm');
        }
    });

    // Color Pickers Synchronizers
    $('#OnboardingInput_PrimaryColor').on('input', function () {
        $('#primary-color-hex').val($(this).val());
    });
    $('#primary-color-hex').on('input', function () {
        var val = $(this).val();
        if (val.match(/^#[0-9A-F]{6}$/i)) {
            $('#OnboardingInput_PrimaryColor').val(val);
        }
    });

    $('#OnboardingInput_SecondaryColor').on('input', function () {
        $('#secondary-color-hex').val($(this).val());
    });
    $('#secondary-color-hex').on('input', function () {
        var val = $(this).val();
        if (val.match(/^#[0-9A-F]{6}$/i)) {
            $('#OnboardingInput_SecondaryColor').val(val);
        }
    });

    // CPF Formatting Mask
    $('.cpf-mask').on('input', function () {
        var val = $(this).val().replace(/\D/g, '');
        if (val.length > 11) {
            val = val.substring(0, 11);
        }
        if (val.length > 9) {
            val = val.replace(/^(\d{3})(\d{3})(\d{3})(\d{2})$/, "$1.$2.$3-$4");
        } else if (val.length > 6) {
            val = val.replace(/^(\d{3})(\d{3})(\d{1,3})$/, "$1.$2.$3");
        } else if (val.length > 3) {
            val = val.replace(/^(\d{3})(\d{1,3})$/, "$1.$2");
        }
        $(this).val(val);
    });

    // Initial wizard setup trigger
    if ($('#onboardingForm').length > 0) {
        updateWizard();
    }

    // --- SaaS Subscriptions & Landing Page JS ---
    $('.OpenRegisterTrialModalButton').click(function (e) {
        e.preventDefault();
        var planName = $(this).data('plan');
        if (planName) {
            $('#TrialInput_SelectedPlan').val(planName);
        }
        $('#RegisterTrialModal').modal('show');
    });

    $('.upgrade-plan-select-card').click(function () {
        $('.upgrade-plan-select-card').removeClass('active').css('border-color', 'transparent');
        $(this).addClass('active').css('border-color', 'var(--color-mint-teal-base)');
        var planName = $(this).data('plan');
        $('#UpgradePlanNameInput').val(planName);
    });
});
