$(function () {
    var l = abp.localization.getResource('WeCare');
    var createModal = new abp.ModalManager(abp.appPath + 'Consultations/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Consultations/EditModal'); // Assuming it exists or will be adapted

    var calendarEl = document.getElementById('calendar');

    var calendar = new FullCalendar.Calendar(calendarEl, {
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
        events: function (fetchInfo, successCallback, failureCallback) {
            weCare.consultations.consultation.getList({
                maxResultCount: 1000 // In a real app we'd filter by date range
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
            // Edit modal or View Panel
            editModal.open({ id: info.event.id });
        },
        dateClick: function (info) {
            // New modal with pre-filled date
            createModal.open({
                consultationDate: info.dateStr
            });
        }
    });

    calendar.render();

    $('#NewEventButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    createModal.onResult(function () {
        calendar.refetchEvents();
    });

    editModal.onResult(function () {
        calendar.refetchEvents();
    });

    function getEventColor(id) {
        // Simple hash to color
        var colors = ['#5e72e4', '#2dce89', '#11cdef', '#fb6340', '#f5365c', '#8965e0', '#32325d'];
        var sum = 0;
        for (var i = 0; i < id.length; i++) {
            sum += id.charCodeAt(i);
        }
        return colors[sum % colors.length];
    }
});
