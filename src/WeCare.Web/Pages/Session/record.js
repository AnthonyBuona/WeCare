$(function () {
    var config = window.__sessionConfig;
    var l = abp.localization.getResource('WeCare');
    var objectiveService = weCare.objectives.objective;
    var objectiveTrainings = [];

    // ─── SESSION TIMER ───
    var sessionStartTime = new Date();
    var timerEl = document.getElementById('sessionTimer');

    function updateTimer() {
        var elapsed = Math.floor((new Date() - sessionStartTime) / 1000);
        var h = Math.floor(elapsed / 3600);
        var m = Math.floor((elapsed % 3600) / 60);
        var s = elapsed % 60;
        timerEl.textContent =
            String(h).padStart(2, '0') + ':' +
            String(m).padStart(2, '0') + ':' +
            String(s).padStart(2, '0');

        // Update hidden duration field
        var totalMinutes = Math.floor(elapsed / 60);
        document.getElementById('sessionDuration').value = totalMinutes + ' min';
    }

    setInterval(updateTimer, 1000);
    updateTimer();

    // ─── OBJECTIVE CHANGE → LOAD TRAININGS ───
    var objectiveSelect = document.getElementById('objectiveSelect');

    function fetchTrainingsForObjective(objectiveId) {
        objectiveTrainings = [];
        if (!objectiveId) {
            updateAllTrainingSelects();
            return;
        }
        objectiveService.getTrainingsForObjectiveAsync(objectiveId).done(function (result) {
            objectiveTrainings = result.items;
            updateAllTrainingSelects();
        });
    }

    function updateAllTrainingSelects() {
        var selects = document.querySelectorAll('#trainingsContainer .training-select');
        selects.forEach(function (select) {
            populateTrainingSelect(select);
        });
    }

    function populateTrainingSelect(selectEl) {
        var currentValue = selectEl.value;
        selectEl.innerHTML = '<option value="">Selecione um treino...</option>';
        if (objectiveTrainings && objectiveTrainings.length > 0) {
            objectiveTrainings.forEach(function (t) {
                var opt = document.createElement('option');
                opt.value = t.id;
                opt.textContent = t.displayName;
                selectEl.appendChild(opt);
            });
        }
        selectEl.value = currentValue;
    }

    objectiveSelect.addEventListener('change', function () {
        fetchTrainingsForObjective(this.value);
    });

    // Load initial trainings if objective is pre-selected
    if (objectiveSelect.value) {
        fetchTrainingsForObjective(objectiveSelect.value);
    }

    // ─── DYNAMIC TRAINING ITEMS ───
    var trainingsContainer = document.getElementById('trainingsContainer');
    var addTrainingBtn = document.getElementById('addTrainingBtn');
    var trainingIndex = 0;

    function createTrainingItem() {
        var idx = trainingIndex++;
        var div = document.createElement('div');
        div.className = 'training-entry';
        div.dataset.index = idx;

        div.innerHTML = `
            <span class="training-number">${idx + 1}</span>
            <button type="button" class="remove-btn" title="Remover treino">&times;</button>

            <div style="margin-bottom: 0.85rem;">
                <label class="session-label">Atividade realizada</label>
                <select name="SessionData.PerformedTrainings[${idx}].TrainingId"
                        class="session-select training-select" required>
                    <option value="">Selecione um treino...</option>
                </select>
            </div>

            <div style="margin-bottom: 0.85rem;">
                <label class="session-label">Ajuda necessária</label>
                <select name="SessionData.PerformedTrainings[${idx}].HelpNeeded" class="session-select">
                    <option value="I">Independente</option>
                    <option value="SV">Suporte Verbal</option>
                    <option value="SG">Suporte Gestual</option>
                    <option value="SP">Suporte Posicional</option>
                    <option value="ST">Suporte Total</option>
                </select>
            </div>

            <div style="display: flex; gap: 1.5rem;">
                <div style="flex: 1;">
                    <label class="session-label">Tentativas</label>
                    <div class="counter-group">
                        <button type="button" class="counter-btn counter-dec" data-target="total-${idx}">−</button>
                        <input type="number" class="counter-value session-input" style="width:60px; text-align:center; padding: 0.4rem;"
                               name="SessionData.PerformedTrainings[${idx}].TotalAttempts"
                               id="total-${idx}" value="5" min="0" />
                        <button type="button" class="counter-btn counter-inc" data-target="total-${idx}">+</button>
                    </div>
                </div>
                <div style="flex: 1;">
                    <label class="session-label">Sucessos</label>
                    <div class="counter-group">
                        <button type="button" class="counter-btn counter-dec" data-target="success-${idx}">−</button>
                        <input type="number" class="counter-value session-input" style="width:60px; text-align:center; padding: 0.4rem;"
                               name="SessionData.PerformedTrainings[${idx}].SuccessfulAttempts"
                               id="success-${idx}" value="3" min="0" />
                        <button type="button" class="counter-btn counter-inc" data-target="success-${idx}">+</button>
                    </div>
                </div>
            </div>
        `;

        trainingsContainer.appendChild(div);

        // Populate select with current trainings
        var select = div.querySelector('.training-select');
        populateTrainingSelect(select);

        // Remove button
        div.querySelector('.remove-btn').addEventListener('click', function () {
            div.style.opacity = '0';
            div.style.transform = 'translateY(-10px)';
            div.style.transition = 'all 0.25s ease';
            setTimeout(function () {
                div.remove();
                reindexTrainings();
            }, 250);
        });
    }

    function reindexTrainings() {
        var items = trainingsContainer.querySelectorAll('.training-entry');
        items.forEach(function (item, i) {
            // Update number badge
            item.querySelector('.training-number').textContent = i + 1;

            // Update name attributes
            item.querySelectorAll('[name]').forEach(function (input) {
                var newName = input.name.replace(/\[\d+\]/, '[' + i + ']');
                input.setAttribute('name', newName);
            });

            // Update counter ids and data-targets
            var totalInput = item.querySelector('[name$=".TotalAttempts"]');
            var successInput = item.querySelector('[name$=".SuccessfulAttempts"]');
            if (totalInput) {
                totalInput.id = 'total-' + i;
            }
            if (successInput) {
                successInput.id = 'success-' + i;
            }
            item.querySelectorAll('.counter-dec, .counter-inc').forEach(function (btn) {
                var target = btn.dataset.target;
                if (target && target.startsWith('total-')) {
                    btn.dataset.target = 'total-' + i;
                } else if (target && target.startsWith('success-')) {
                    btn.dataset.target = 'success-' + i;
                }
            });
        });
    }

    addTrainingBtn.addEventListener('click', function () {
        if (!objectiveSelect.value) {
            abp.notify.warn('Por favor, selecione um objetivo primeiro.');
            return;
        }
        createTrainingItem();
    });

    // ─── COUNTER BUTTONS (EVENT DELEGATION) ───
    trainingsContainer.addEventListener('click', function (e) {
        var btn = e.target.closest('.counter-btn');
        if (!btn) return;

        var targetId = btn.dataset.target;
        var input = document.getElementById(targetId);
        if (!input) return;

        var val = parseInt(input.value) || 0;
        if (btn.classList.contains('counter-inc')) {
            input.value = val + 1;
        } else if (btn.classList.contains('counter-dec') && val > 0) {
            input.value = val - 1;
        }
    });

    // ─── FORM SUBMIT ───
    var form = document.getElementById('sessionForm');
    var finishBtn = document.getElementById('finishSessionBtn');

    form.addEventListener('submit', function (e) {
        e.preventDefault();

        // Validate at least one objective selected
        if (!objectiveSelect.value) {
            abp.notify.warn('Por favor, selecione um objetivo antes de finalizar.');
            return;
        }

        abp.message.confirm(
            'Tem certeza que deseja finalizar esta sessão? Os dados serão salvos permanentemente.',
            'Finalizar Sessão'
        ).then(function (confirmed) {
            if (confirmed) {
                finishBtn.disabled = true;
                finishBtn.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Salvando...';
                form.submit();
            }
        });
    });

    // ─── AUTO-ADD FIRST TRAINING ───
    // Wait until objective is selected, then add first item
    objectiveSelect.addEventListener('change', function () {
        if (this.value && trainingsContainer.children.length === 0) {
            // Small delay for trainings to load
            setTimeout(function () {
                createTrainingItem();
            }, 500);
        }
    });
});
