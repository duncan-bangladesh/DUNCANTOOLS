$(document).ready(function () {
    my_datepicker(FromDate);
    my_datepicker(ToDate);
    $.get('/Company/GetEstateCodeFromScale').done(function (data) {
        if (data.length > 0) {
            $('#EstateCodeForScale').empty();
            $("#EstateCodeForScale").append('<option value="0">--Select--</option>');
            for (var i = 0; i < data.length; i++) {
                $('<option/>',
                    {
                        value: data[i].estateCodeForScale,
                        html: data[i].companyName
                    }
                ).appendTo("#EstateCodeForScale");
            }
        }
    });
    $('#btnSearch').click(function () {
        let estateCode = $('#EstateCodeForScale').val();
        let fromDate = $('#FromDate').val();
        let toDate = $('#ToDate').val();

        $.post('/Weighbridge/SearchScaleData',
            { EstateCode: estateCode, FromDate: fromDate, ToDate: toDate },
            function (data) {
                if (!data || data.length === 0) {
                    $('#rTable tbody').html(
                        '<tr><td colspan="6" class="text-center text-danger">No data found</td></tr>'
                    );
                    return;
                }
                let tbody = '';
                let grandTotal = 0;
                const estateGroups = groupBy(data, x => x.teaEstate);
                Object.keys(estateGroups).forEach(estate => {
                    let estateTotal = 0;

                    // Estate Header (Green Tone)
                    tbody += `
                    <tr style="background-color:#f0f5f5">
                        <td colspan="6">
                            <strong>${estate}</strong>
                        </td>
                    </tr>
                `;

                    const vehicleGroups = groupBy(estateGroups[estate], x => x.vehicle);
                    Object.keys(vehicleGroups).forEach(vehicle => {
                        let vehicleTotal = 0;
                        vehicleGroups[vehicle].forEach(item => {
                            const net = parseFloat(item.netWeight);
                            vehicleTotal += net;
                            estateTotal += net;
                            grandTotal += net;
                            tbody += `
                            <tr>
                                <td>${item.recordDate}</td>
                                <td>${item.recordTime}</td>
                                <td>${item.vehicle}</td>
                                <td class="text-end">${item.loadedVehicle.toFixed(2)}</td>
                                <td class="text-end">${item.tareWeight.toFixed(2)}</td>
                                <td class="text-end">${item.netWeight.toFixed(2)}</td>
                            </tr>
                        `;
                        });

                        // Vehicle Subtotal (Light Green)
                        tbody += `
                        <tr style="background-color:#f5f5f0">
                            <td colspan="5" class="text-end">
                                <strong>Subtotal</strong>
                            </td>
                            <td class="text-end">
                                <strong>${vehicleTotal.toFixed(2)}</strong>
                            </td>
                        </tr>
                    `;
                    });

                    // Estate Total (Stronger Green)
                    tbody += `
                    <tr style="background-color:#b3ffb3">
                        <td colspan="5" class="text-end">
                            <strong>Total (${estate})</strong>
                        </td>
                        <td class="text-end">
                            <strong>${estateTotal.toFixed(2)}</strong>
                        </td>
                    </tr>
                `;
                });

                // ✅ Show Grand Total ONLY when estateCode == 0
                if (parseInt(estateCode) == 0) {
                    tbody += `
                    <tr style="background-color:#00ff55">
                        <td colspan="5" class="text-end">
                            <strong>Grand Total</strong>
                        </td>
                        <td class="text-end">
                            <strong>${grandTotal.toFixed(2)}</strong>
                        </td>
                    </tr>
                `;
                }
                $('#rTable tbody').html(tbody);
            });
    });
});
function groupBy(array, keySelector) {
    return array.reduce((result, item) => {
        const key = keySelector(item) || 'Unknown';
        if (!result[key]) result[key] = [];
        result[key].push(item);
        return result;
    }, {});
}