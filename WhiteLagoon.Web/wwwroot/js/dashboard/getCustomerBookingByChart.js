
$(document).ready(function () {

    loadCustomerBookingPieChartData();
});

function loadCustomerBookingPieChartData() {
    $('.chart-spinner').show();

    $.ajax({
        url: "/Dashboard/GetBookingPieChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {
           
            loadPieChart("customerBookingsPieChart", data);

            $(".chart-spinner").hide();

        }
    })
}

function loadPieChart(id,data) {
    var chartColors = getChartColorsArray(id);

    var options = {
        series: data.series,
        labels: data.labels,
        colors: chartColors,
        chart: {
            type: 'donut',
            width: 440
        },
        stroke: {
            show: false
        },
        legends: {
            position: 'bottom',
            horizontalAlign: 'center',
            labels: {
                colors: '#fff',
                useSeriesColors: true
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}
