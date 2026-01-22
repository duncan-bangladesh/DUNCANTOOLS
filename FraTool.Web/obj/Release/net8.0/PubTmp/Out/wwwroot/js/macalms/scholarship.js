$(document).ready(function () {
    $('#scholarshipDiv').hide();
    initializeDropdowns();
    $('#btnSearch').click(function () {
        let assessmentYear = $('#AssessmentYear option:selected').text();
        $.get('/Macalms/GetScholarshipData', { AssessmentYear: assessmentYear }, function (data) {
            if (data.length > 0) {
                $('#rTable tbody').empty();
                var rows = '<tbody>';
                var total = 0;
                for (var i = 0; i < data.length; i++) {
                    total += parseInt(data[i].amount);
                    rows += `<tr>
                            <td>${data[i].studentName}</td>
                            <td>${data[i].parentName}</td>
                            <td>${data[i].dateOfBirth}</td>
                            <td>${data[i].bankName}</td>
                            <td>${data[i].bankBranch}</td>
                            <td>${data[i].bankAccountNo}</td>
                            <td>${data[i].bankRoutingNo}</td>
                            <td>${numberWithCommas(data[i].amount)}</td>                           
                        </tr>`;
                }
                rows += `<tr style="font-weight: bold;"><td></td><td></td><td></td><td></td><td></td><td></td><td>Total</td><td>${numberWithCommas(total)}</td></tr>`;
                $('#rTable').append(rows);

                var fileType = "excel";
                var url = "/Macalms/DownloadScholarship"
                    + "?AssessmentYear=" + encodeURIComponent(assessmentYear)
                    + "&FileType=" + encodeURIComponent(fileType);
                $("#aExcel").attr("href", url);

                var fileType2 = "pdf";
                var url2 = "/Macalms/DownloadScholarship"
                    + "?AssessmentYear=" + encodeURIComponent(assessmentYear)
                    + "&FileType=" + encodeURIComponent(fileType2);
                $("#aPdf").attr("href", url2);

                $('#scholarshipDiv').show();
            }
            else {
                $('#scholarshipDiv').hide();
            }
        });
    });
    
});
function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
};
function initializeDropdowns() {
    loadAssessmentYear('/Macalms/GetAssessmentYears', '#AssessmentYear', 'yearName');
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