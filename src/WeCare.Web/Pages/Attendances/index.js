$(function () {
    var l = abp.localization.getResource('WeCare');
    var createModal = new abp.ModalManager(abp.appPath + 'Pages/Attendances/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Pages/Attendances/EditModal');

    var dataTable = $('#AttendancesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[2, "desc"]], // Sort by SessionDate by default (descending)
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(weCare.attendances.attendance.getList),
            columnDefs: [
                {
                    title: l('Actions'),
                    rowAction: {
                        items: [
                            {
                                text: l('Edit'),
                                visible: abp.auth.isGranted('WeCare.Attendances.Edit'),
                                action: function (data) {
                                    editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: l('Delete'),
                                visible: abp.auth.isGranted('WeCare.Attendances.Delete'),
                                confirmMessage: function (data) {
                                    return l('AttendanceDeletionConfirmationMessage');
                                },
                                action: function (data) {
                                    weCare.attendances.attendance
                                        .delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l('SuccessfullyDeleted'));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                    }
                },
                {
                    title: l('Patient'),
                    data: "patientName"
                },
                {
                    title: l('SessionDate'),
                    data: "sessionDate",
                    render: function (data) {
                        if (!data) return "";
                        return luxon
                            .DateTime
                            .fromISO(data, { locale: abp.localization.currentCulture.name })
                            .toLocaleString(luxon.DateTime.DATE_MED);
                    }
                },
                {
                    title: l('Status'),
                    data: "status",
                    render: function (data) {
                        var badgeClass = 'badge bg-secondary';
                        if (data === 0) {
                            badgeClass = 'badge bg-success badge-success'; // Present
                        } else if (data === 1) {
                            badgeClass = 'badge bg-danger badge-danger'; // Absent
                        } else if (data === 2) {
                            badgeClass = 'badge bg-warning badge-warning text-dark'; // Cancelled
                        }
                        var text = l('Enum:AttendanceStatus.' + data) || data;
                        return '<span class="' + badgeClass + '">' + text + '</span>';
                    }
                },
                {
                    title: l('Notes'),
                    data: "notes"
                }
            ]
        })
    );

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#NewAttendanceButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});
