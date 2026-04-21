$(document).ready(function () {
    $('#scholarshipDiv').hide();
    $('#divPaymentSave').hide();
    initializeDropdowns();
    var dSet = '';
    function getTheRow(rowId) {
        let obj = dSet.find(x => x.sl == rowId);
        let row = $(`#rTable input[id*='_${rowId}']`).closest("tr");
        let scholarshipDuration = parseInt(obj.scholarshipDuration) || 0;
        let allowedMonths = parseInt(row.find("input[id^='allowedMonths_']").val()) || 0;
        let studyMedium = obj.studyMedium;

        if (allowedMonths >= 0) {
            let month = 0;
            if (scholarshipDuration + allowedMonths > 12) {
                month = 12;
            }
            else {
                month = scholarshipDuration + allowedMonths;
            }

            let amount = 0;
            if (studyMedium == "English") {
                amount = month * 2500;
            }
            else if ("Bengali") {
                amount = month * 2000;
            }
            row.find("td:last").text(numberWithCommas(amount.toFixed(2)));

            if (typeof obj != 'undefined') {
                obj.allowedMonths = allowedMonths;
                obj.amount = amount;
            }
            let xTotal = 0;
            $.each(dSet, function (index, item) {
                xTotal += item.amount;
            });
            $("#rTable tr:last td:last").text(numberWithCommas(xTotal.toFixed(2)));
        }
    }
    $('#btnSearch').click(function () {
        let assessmentYear = $('#AssessmentYear option:selected').text();

        $.get('/Macalms/GetScholarshipData',
            { AssessmentYear: assessmentYear },
            function (data) {
                if (!data || data.length === 0) {
                    $('#scholarshipDiv').hide();
                    return;
                }
                dSet = data;
                const isPayment = data[0].isPayment;
                let total = 0;
                let rows = '';

                $('#rTable tbody').empty();
                data.forEach(item => {
                    total += parseFloat(item.amount);

                    // 🔹 Conditional rendering for Allowed Months column
                    let allowedMonthsColumn = '';

                    if (isPayment > 0) {
                        // Show as normal table cell (read-only)
                        allowedMonthsColumn = `
                    <td class="text-center">
                        ${item.allowedMonths}
                    </td>`;
                    } else {
                        // Show editable input field
                        allowedMonthsColumn = `
                    <td style="display:flex; justify-content:center;">
                        <input type="number"
                               class="form-control"
                               id="allowedMonths_${item.sl}"
                               value="${item.allowedMonths}" />
                    </td>`;
                    }

                    rows += `
                        <tr>
                            <td>${item.studentName}</td>
                            <td>${item.parentName}</td>
                            <td>${item.dateOfBirth}</td>
                            <td class="text-center">${item.scholarshipDuration}</td>
                            ${allowedMonthsColumn}
                            <td>${item.bankName}</td>
                            <td>${item.bankBranch}</td>
                            <td>${item.bankAccountNo}</td>
                            <td>${item.bankRoutingNo}</td>
                            <td style="text-align:right;">
                                ${numberWithCommas(parseFloat(item.amount).toFixed(2))}
                            </td>
                        </tr>`;
                    }
                );

                rows += `
                    <tr style="font-weight:bold;">
                        <td colspan="8"></td>
                        <td>Total</td>
                        <td style="text-align:right;">
                            ${numberWithCommas(total.toFixed(2))}
                        </td>
                    </tr>`;

                $('#rTable tbody').append(rows);

                // Bind event ONLY if editable mode
                if (isPayment <= 0) {
                    $('#rTable')
                        .off('input change', 'input')
                        .on('input change', 'input', function () {
                            const rowId = this.id.split('_')[1];
                            getTheRow(rowId);
                        }
                    );
                }

                $('#scholarshipDiv').show();
                $('#divPaymentSave').toggle(isPayment <= 0);
            }
        );

    });
    $('#btnSave').click(function () {
        if (dSet != '' && dSet != null) {
            $.post('/Macalms/SavePayment',
                {
                    model: JSON.stringify(dSet)
                },
                function (data) {
                    console.log(data);
                }
            );
        }
    });
});
function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
};
function initializeDropdowns() {
    loadAssessmentYear('/Macalms/AssessmentYear_dd', '#AssessmentYear', 'yearName');
}
function loadAssessmentYear(url, selector, textField) {
    $.get(url).done(function (data) {
        const ddl = $(selector);
        resetDropdown(ddl);
        $.each(data, function (index, item) {
            ddl.append(
                $('<option>', {
                    value: item.recordId,
                    text: item[textField]
                })
            );
            // Select first item
            if (index === 0) {
                ddl.val(item.recordId);
            }
        });
    });
}
function resetDropdown(selectorOrElement) {
    const ddl = typeof selectorOrElement === 'string'
        ? $(selectorOrElement)
        : selectorOrElement;
    ddl.empty().append('<option value="0">--Select--</option>');
}