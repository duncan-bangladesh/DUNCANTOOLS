$(document).ready(function () {
    $('#vData').hide();
    my_datepicker(FromDate);
    my_datepicker(ToDate);
    LoadCompany();
    $('#btnSearch').click(function () {
        var companyCode = $('#CompanyCode').val();
        var fromDate = $('#FromDate').val();
        var toDate = $('#ToDate').val();
        if ($('#CompanyCode').val() != 0) {
            $.get("/SOE/GetWagesReportData", { CompanyCode: companyCode, FromDate: fromDate, ToDate: toDate }, function (data) {
                var response = data.data || [];
                //console.log(response);
                buildTeaReport(response);
            }).fail(function () {                
                $('#rTable tbody').html(`<tr><td colspan="16" class="text-center text-danger">Error loading data</td></tr>`);
                toastr.error('Error loading report data.');
            });
        } else {
            toastr.error('Please select a Tea Estate.');
        }
    });
});
function LoadCompany() {
    $.get('/Company/GetTeaEstates', function (data) {
        if (data.length > 0) {
            $('#CompanyCode').empty();
            $("#CompanyCode").append('<option value="0">--Select--</option>');
            for (var i = 0; i < data.length; i++) {
                $('<option/>',
                    {
                        value: data[i].companyCode,
                        html: data[i].companyName
                    }
                ).appendTo("#CompanyCode");
            }
        }
    });
}
//function formatNumber(value) {
//    if (value === null || value === undefined || value === '') {
//        return '0';
//    }
//    var number = Number(value);
//    if (isNaN(number)) {
//        return '0';
//    }
//    return number.toLocaleString('en-US', {
//        minimumFractionDigits: 0,
//        maximumFractionDigits: 0
//    });
//}
//function formatWages(value) {
//    if (value === null || value === undefined || value === '') {
//        return '0.00';
//    }
//    var number = Number(value);
//    if (isNaN(number)) {
//        return '0.00';
//    }
//    return number.toLocaleString('en-US', {
//        minimumFractionDigits: 2,
//        maximumFractionDigits: 2
//    });
//}

function buildTeaReport(data) {

    const $tbody = $('#teaReportBody');

    $tbody.empty();

    if (!data || data.length === 0) {

        $tbody.append(`
            <tr>
                <td colspan="16" class="text-center">
                    No data found
                </td>
            </tr>
        `);

        return;
    }


    // ============================================================
    // 1. GROUP BY ACCOUNTS CATEGORY
    //    Category = DESC
    // ============================================================

    const categories = groupBy(data, 'accountsCategory');

    const sortedCategories = Object.keys(categories).sort(function (a, b) {

        return b.localeCompare(a);

    });


    // ============================================================
    // 2. PROCESS EACH CATEGORY
    // ============================================================

    $.each(sortedCategories, function (categoryIndex, categoryName) {

        const categoryItems = categories[categoryName];


        // --------------------------------------------------------
        // CATEGORY HEADER
        // --------------------------------------------------------

        $tbody.append(`
            <tr class="category-row">
                <td colspan="16">
                    ${escapeHtml(categoryName)}
                </td>
            </tr>
        `);


        // --------------------------------------------------------
        // CATEGORY TOTAL
        // --------------------------------------------------------

        let categoryTotal = createEmptyTotal();


        // ========================================================
        // 3. GROUP BY ACCOUNTS CODE
        // ========================================================

        const accountGroups = groupBy(
            categoryItems,
            'accountsCode'
        );


        // ========================================================
        // 4. SORT ACCOUNTS CODE GROUPS BY AccountsOrder ASC
        // ========================================================

        const sortedAccountCodes = Object.keys(accountGroups).sort(
            function (a, b) {

                const orderA = toNumber(
                    accountGroups[a][0].AccountsOrder
                );

                const orderB = toNumber(
                    accountGroups[b][0].AccountsOrder
                );


                // AccountsOrder ASC

                if (orderA !== orderB) {
                    return orderA - orderB;
                }


                // Optional fallback:
                // If two accounts have the same AccountsOrder,
                // sort them by accountsCode.

                return String(a).localeCompare(
                    String(b),
                    undefined,
                    {
                        numeric: true,
                        sensitivity: 'base'
                    }
                );

            }
        );


        // ========================================================
        // 5. PROCESS ACCOUNTS CODE IN AccountsOrder
        // ========================================================

        $.each(sortedAccountCodes, function (
            accountIndex,
            accountsCode
        ) {

            const accountItems = accountGroups[accountsCode];


            // ====================================================
            // ACCOUNT CODE TOTAL
            // ====================================================

            let accountTotal = createEmptyTotal();


            // ====================================================
            // FIRST ROW OF ACCOUNTS CODE
            //
            // ONLY:
            //     accountsCode
            //     accountsDescription
            //
            // NO NUMERIC VALUES
            // ====================================================

            const firstItem = accountItems[0];


            $tbody.append(`

                <tr class="account-header-row">

                    <!-- MANDAYS - EMPTY -->

                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>


                    <!-- ACCOUNTS CODE -->

                    <td class="account-code account-main-code">
                        ${escapeHtml(firstItem.accountsCode)}
                    </td>


                    <!-- DESCRIPTION -->

                    <td class="description account-main-description">
                        ${escapeHtml(firstItem.accountsDescription)}
                    </td>


                    <!-- WAGES - EMPTY -->

                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>

                </tr>

            `);

            // 6. DISPLAY ALL subCode RECORDS

            $.each(accountItems, function (index, item) {
                const rowTotal = getRowValues(item);
                // ADD TO ACCOUNT SUBTOTAL
                addTotals(accountTotal, rowTotal);
                // ADD TO CATEGORY TOTAL
                addTotals(categoryTotal, rowTotal);
                // SUB CODE DATA ROW
                $tbody.append(`
                    <tr class="data-row sub-code-row">
                        <td>${formatNumber(item.permanentAttendance)}</td>
                        <td>${formatNumber(item.temporaryAttendance)}</td>
                        <td>${formatNumber(item.doubleHazira)}</td>
                        <td>${formatNumber(item.totalAttendance)}</td>
                        <td>${formatNumber(item.ytdPreviousMonth)}</td>
                        <td>${formatNumber(item.ytdAttendanceThisYear)}</td>
                        <td>${formatNumber(item.ytdAttendanceLastYear)}</td>
                        <!-- SUB CODE -->
                        <td class="account-code">${escapeHtml(item.subCode)}</td>
                        <!-- SUB CODE DESCRIPTION -->
                        <td class="description">${escapeHtml(item.subCodeDescription)}</td>
                        <!-- LABOUR WAGES -->
                        <td>${formatNumber(item.permanentAttendanceWages)}</td>
                        <td>${formatNumber(item.temporaryAttendanceWages)}</td>
                        <td>${formatNumber(item.doubleHaziraWages)}</td>
                        <td>${formatNumber(item.totalAttendanceWages)}</td>
                        <td>${formatNumber(item.ytdWagesPreviousMonth)}</td>
                        <td>${formatNumber(item.ytdWagesThisYear)}</td>
                        <td>${formatNumber(item.ytdWagesLastYear)}</td>
                    </tr>
                `);
            });
            // 7. SUB TOTAL FOR ACCOUNTS CODE
            $tbody.append(createTotalRow(`Sub Total (${firstItem.accountsDescription})`, accountTotal, 'subtotal-row'));
        });
        // 8. TOTAL FOR ACCOUNTS CATEGORY
        $tbody.append(createTotalRow(`Total (${categoryName})`, categoryTotal, 'category-total-row'));
    });

}
function groupBy(array, property) {

    return array.reduce(function (groups, item) {

        const key = item[property] || '';

        if (!groups[key]) {
            groups[key] = [];
        }

        groups[key].push(item);

        return groups;

    }, {});

}
function createEmptyTotal() {

    return {

        permanentAttendance: 0,
        temporaryAttendance: 0,
        doubleHazira: 0,
        totalAttendance: 0,

        ytdPreviousMonth: 0,
        ytdAttendanceThisYear: 0,
        ytdAttendanceLastYear: 0,

        permanentAttendanceWages: 0,
        temporaryAttendanceWages: 0,
        doubleHaziraWages: 0,
        totalAttendanceWages: 0,

        ytdWagesPreviousMonth: 0,
        ytdWagesThisYear: 0,
        ytdWagesLastYear: 0

    };

}
function getRowValues(item) {

    return {

        permanentAttendance:
            toNumber(item.permanentAttendance),

        temporaryAttendance:
            toNumber(item.temporaryAttendance),

        doubleHazira:
            toNumber(item.doubleHazira),

        totalAttendance:
            toNumber(item.totalAttendance),

        ytdPreviousMonth:
            toNumber(item.ytdPreviousMonth),

        ytdAttendanceThisYear:
            toNumber(item.ytdAttendanceThisYear),

        ytdAttendanceLastYear:
            toNumber(item.ytdAttendanceLastYear),


        permanentAttendanceWages:
            toNumber(item.permanentAttendanceWages),

        temporaryAttendanceWages:
            toNumber(item.temporaryAttendanceWages),

        doubleHaziraWages:
            toNumber(item.doubleHaziraWages),

        totalAttendanceWages:
            toNumber(item.totalAttendanceWages),

        ytdWagesPreviousMonth:
            toNumber(item.ytdWagesPreviousMonth),

        ytdWagesThisYear:
            toNumber(item.ytdWagesThisYear),

        ytdWagesLastYear:
            toNumber(item.ytdWagesLastYear)

    };

}
function addTotals(total, row) {

    total.permanentAttendance += row.permanentAttendance;
    total.temporaryAttendance += row.temporaryAttendance;
    total.doubleHazira += row.doubleHazira;
    total.totalAttendance += row.totalAttendance;

    total.ytdPreviousMonth += row.ytdPreviousMonth;
    total.ytdAttendanceThisYear += row.ytdAttendanceThisYear;
    total.ytdAttendanceLastYear += row.ytdAttendanceLastYear;


    total.permanentAttendanceWages +=
        row.permanentAttendanceWages;

    total.temporaryAttendanceWages +=
        row.temporaryAttendanceWages;

    total.doubleHaziraWages +=
        row.doubleHaziraWages;

    total.totalAttendanceWages +=
        row.totalAttendanceWages;

    total.ytdWagesPreviousMonth +=
        row.ytdWagesPreviousMonth;

    total.ytdWagesThisYear +=
        row.ytdWagesThisYear;

    total.ytdWagesLastYear +=
        row.ytdWagesLastYear;

}
function createTotalRow(title, total, cssClass) {

    return `
        <tr class="${cssClass}">

            <!-- MANDAYS -->

            <td>${formatNumber(total.permanentAttendance)}</td>

            <td>${formatNumber(total.temporaryAttendance)}</td>

            <td>${formatNumber(total.doubleHazira)}</td>

            <td>${formatNumber(total.totalAttendance)}</td>

            <td>${formatNumber(total.ytdPreviousMonth)}</td>

            <td>${formatNumber(total.ytdAttendanceThisYear)}</td>

            <td>${formatNumber(total.ytdAttendanceLastYear)}</td>


            <!-- ACCOUNT CODE -->

            <td></td>


            <!-- DESCRIPTION -->

            <td class="description total-label">
                ${escapeHtml(title)}
            </td>


            <!-- WAGES -->

            <td>${formatNumber(total.permanentAttendanceWages)}</td>

            <td>${formatNumber(total.temporaryAttendanceWages)}</td>

            <td>${formatNumber(total.doubleHaziraWages)}</td>

            <td>${formatNumber(total.totalAttendanceWages)}</td>

            <td>${formatNumber(total.ytdWagesPreviousMonth)}</td>

            <td>${formatNumber(total.ytdWagesThisYear)}</td>

            <td>${formatNumber(total.ytdWagesLastYear)}</td>

        </tr>
    `;

}
function toNumber(value) {

    const number = parseFloat(value);

    return isNaN(number) ? 0 : number;

}
function formatNumber(value) {

    return toNumber(value).toLocaleString('en-US', {

        minimumFractionDigits: 0,
        maximumFractionDigits: 2

    });

}
function escapeHtml(value) {

    if (value === null || value === undefined) {
        return '';
    }

    return $('<div>')
        .text(value)
        .html();

}