$(function () {
    var l = abp.localization.getResource('WeCare');
    var createModal = new abp.ModalManager(abp.appPath + 'Consultations/CreateModal');
    var currentDurationMinutes = 30; // Default, updated from clinic settings

    var calendarEl = document.getElementById('calendar');
    var detailsModal = new bootstrap.Modal(document.getElementById('consultationDetailsModal'));

    // Fetch clinic settings first
    weCare.clinics.clinic.getCurrentClinicSettings().done(function (settings) {
        var businessHours = (settings && settings.operatingHours)
            ? getBusinessHours(settings.operatingHours)
            : null;

        // Compute slot duration from appointment duration (default 30 min)
        var slotDuration = '00:30:00';
        if (settings && settings.appointmentDurationMinutes) {
            var mins = settings.appointmentDurationMinutes;
            currentDurationMinutes = mins; // Update outer-scope variable
            var h = Math.floor(mins / 60);
            var m = mins % 60;
            slotDuration = padTime(h) + ':' + padTime(m) + ':00';
        }

        // Compute visible time range from operating hours
        var timeRange = getTimeRange(settings ? settings.operatingHours : null);

        var calendarOptions = {
            initialView: 'dayGridMonth',
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
            },
            locale: 'pt-br',
            buttonText: {
                today: 'Hoje',
                month: 'Mês',
                week: 'Semana',
                day: 'Dia'
            },
            slotDuration: slotDuration,
            slotMinTime: timeRange.min,
            slotMaxTime: timeRange.max,
            expandRows: true,
            height: 'auto',
            slotLabelFormat: {
                hour: '2-digit',
                minute: '2-digit',
                hour12: false
            },
            events: function (fetchInfo, successCallback, failureCallback) {
                weCare.consultations.consultation.getList({
                    maxResultCount: 1000
                }).done(function (result) {
                    var events = result.items.map(function (item) {
                        return {
                            id: item.id,
                            title: item.patientName + ' (' + item.therapistName + ')',
                            start: item.dateTime,
                            extendedProps: item,
                            backgroundColor: getEventColor(item.therapistId),
                            borderColor: getEventColor(item.therapistId)
                        };
                    });
                    successCallback(events);
                });
            },
            eventClick: function (info) {
                showConsultationDetails(info.event.extendedProps);
            },
            dateClick: function (info) {
                createModal.open({
                    consultationDate: info.dateStr
                });
            }
        };

        if (businessHours && businessHours.length > 0) {
            calendarOptions.businessHours = businessHours;
            calendarOptions.selectConstraint = 'businessHours';
        }

        var calendar = new FullCalendar.Calendar(calendarEl, calendarOptions);

        calendar.render();

        createModal.onResult(function () {
            calendar.refetchEvents();
        });
    });

    $('#NewEventButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    function showConsultationDetails(data) {
        var date = new Date(data.dateTime);
        $('#detailPatientName').text(data.patientName || '-');
        $('#detailTherapistName').text(data.therapistName || '-');
        $('#detailDate').text(date.toLocaleDateString('pt-BR'));
        $('#detailTime').text(date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' }));
        $('#detailDuration').text(data.duration || '-');
        $('#detailMainTraining').text(data.mainTraining || '-');

        if (data.description) {
            $('#detailDescriptionSection').show();
            $('#detailDescription').text(data.description);
        } else {
            $('#detailDescriptionSection').hide();
        }

        // Status badge
        var statusBadge = $('#detailStatusBadge');
        if (data.status === 1) {
            // Realizada
            statusBadge.text('Realizada').css('background', '#2dce89').show();
            $('#startSessionBtn').hide();
        } else {
            // Agendada — check if within time window
            statusBadge.text('Agendada').css('background', '#5e72e4').show();

            var now = new Date();
            var consultStart = new Date(data.dateTime);
            var durationMinutes = currentDurationMinutes || 30;
            var consultEnd = new Date(consultStart.getTime() + durationMinutes * 60 * 1000);

            if (now >= consultStart && now <= consultEnd) {
                // Consultation is happening right now
                $('#startSessionBtn')
                    .attr('href', '/Session/Record?consultationId=' + data.id)
                    .show();
            } else {
                $('#startSessionBtn').hide();
            }
        }

        detailsModal.show();
    }

    function getEventColor(id) {
        var colors = ['#5e72e4', '#2dce89', '#11cdef', '#fb6340', '#f5365c', '#8965e0', '#32325d'];
        var sum = 0;
        for (var i = 0; i < id.length; i++) {
            sum += id.charCodeAt(i);
        }
        return colors[sum % colors.length];
    }

    function getBusinessHours(operatingHours) {
        var businessHours = [];
        if (!operatingHours) return businessHours;

        operatingHours.forEach(function (oh) {
            if (oh.isClosed) return;

            // Handle break times
            if (oh.breakStart && oh.breakEnd) {
                // Morning slot
                businessHours.push({
                    daysOfWeek: [getDayOfWeekInt(oh.dayOfWeek)],
                    startTime: oh.startTime,
                    endTime: oh.breakStart
                });
                // Afternoon slot
                businessHours.push({
                    daysOfWeek: [getDayOfWeekInt(oh.dayOfWeek)],
                    startTime: oh.breakEnd,
                    endTime: oh.endTime
                });
            } else {
                // Single continuous slot
                businessHours.push({
                    daysOfWeek: [getDayOfWeekInt(oh.dayOfWeek)],
                    startTime: oh.startTime,
                    endTime: oh.endTime
                });
            }
        });
        return businessHours;
    }

    function getDayOfWeekInt(day) {
        if (typeof day === 'number') return day;
        switch (String(day).toLowerCase()) {
            case 'sunday': return 0;
            case 'monday': return 1;
            case 'tuesday': return 2;
            case 'wednesday': return 3;
            case 'thursday': return 4;
            case 'friday': return 5;
            case 'saturday': return 6;
            default: return 0;
        }
    }

    function padTime(n) {
        return n < 10 ? '0' + n : '' + n;
    }

    function getTimeRange(operatingHours) {
        var defaultMin = '07:00:00';
        var defaultMax = '19:00:00';
        if (!operatingHours || operatingHours.length === 0) {
            return { min: defaultMin, max: defaultMax };
        }

        var minTime = null;
        var maxTime = null;

        operatingHours.forEach(function (oh) {
            if (oh.isClosed) return;

            var start = parseTimeToMinutes(oh.startTime);
            var end = parseTimeToMinutes(oh.endTime);

            if (minTime === null || start < minTime) minTime = start;
            if (maxTime === null || end > maxTime) maxTime = end;
        });

        if (minTime === null || maxTime === null) {
            return { min: defaultMin, max: defaultMax };
        }

        // Add a small buffer (1 hour before/after) for better UX
        minTime = Math.max(0, minTime - 60);
        maxTime = Math.min(24 * 60, maxTime + 60);

        return {
            min: minutesToTimeStr(minTime),
            max: minutesToTimeStr(maxTime)
        };
    }

    function parseTimeToMinutes(timeStr) {
        if (!timeStr) return 0;
        // Handle "HH:MM:SS" or "HH:MM" format
        var parts = String(timeStr).split(':');
        return parseInt(parts[0], 10) * 60 + parseInt(parts[1], 10);
    }

    function minutesToTimeStr(totalMinutes) {
        var h = Math.floor(totalMinutes / 60);
        var m = totalMinutes % 60;
        return padTime(h) + ':' + padTime(m) + ':00';
    }
});
