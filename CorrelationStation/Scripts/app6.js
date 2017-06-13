$(document.body).on("click", ".js-show-graphs", function () {
    $(this).next(".graph-div").toggle();
});

var globalData;


var chiTableData = function (data, ref, dataIndex, tableName) {

    var significantKeys = [];

    for (var i = 0; i < data[4].length; i++)
    {
        significantKeys.push(data[4][i]["Key"]);
    }

    console.log("sig keys: ", significantKeys);

    var graphDiv = ref.nextAll(".graph-div:first");
    
    graphDiv.append("<table>");
    var keys = [];
    graphDiv.append("<tr><th colspan='"+ data[2].length + 1 +"'>"+ tableName +"</th></tr>" );
    graphDiv.append("<tr>");
    graphDiv.append("<th></th>");
    for(var i = 0; i < data[2].length; i++)
    {
        graphDiv.append("<th>" + data[2][i]["Key"] + "</th>");
        keys.push(data[2][i]["Key"]);
    }

    var data1IndexCount = 0;
    for (var i = 0; i < data[3].length; i++)
    {
        graphDiv.append("<tr>");
        graphDiv.append("<td class='bold'>" + data[3][i]["Key"] + "</td>");
        
        var count = 0;

        for (var j = 0; j < data[2].length; j++)
        {
            var index = data[dataIndex].findIndex(x => x["Key"] == keys[count] + " " + data[3][i]["Key"]);

            if (significantKeys.indexOf(data[dataIndex][index]["Key"]) > -1)
            {
                graphDiv.append("<td class='sig-table-value'>" + Math.round(data[dataIndex][index]["Value"] * 100) / 100 + "</td>");
            }
            else {
                graphDiv.append("<td>" + Math.round(data[dataIndex][index]["Value"] * 100) / 100 + "</td>");
            }
            
            count += 1;
            data1IndexCount += 1;

        }

        graphDiv.append("</tr>");

    }


    graphDiv.append("</tr>")
    graphDiv.append("</table>");


    //need end tr
    //need end table   

}

var round = function(value, decimals) {
    return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
}


//var chiPercentages = function(data, ref)
//{
    
//    var graphDiv = ref.nextAll(".graph-div:first");
//    graphDiv.empty();
//    graphDiv.append("<div><div class='blue-legend'>Expected %</div></div><div><div class='red-legend'>Observed %</div></div>");

//    for(var i = 0; i < data[0].length; i++)
//    {
//        graphDiv.append("<div class='percent-graph-pair'>");
//        graphDiv.append("<div class='bold'>"+ data[0][i]["Key"] +"</div>");

//        graphDiv.append("<div><div class='inline percent-graph-number'>"+ round(100*data[1][i]["Value"], 2) + "%</div><div class='expected-div inline' style='width:"+ Math.round(data[1][i]["Value"]*800) +"px'></div></div>");
//        graphDiv.append("<div><div class='inline percent-graph-number'>" + round(100*data[0][i]["Value"], 2) + "%</div><div class='observed-div inline' style='width:" + Math.round(data[0][i]["Value"] * 800) + "px'></div></div>");


//        graphDiv.append("</div>");//end percent-graph-pair div
//    }

//}


var chiPercentages = function (data, ref) {
    console.log("chi percne: ", data);
    var graphDiv = ref.nextAll(".graph-div:first");
    graphDiv.empty();
    graphDiv.append("<div><div class='blue-legend'>Expected %</div></div><div><div class='red-legend'>Observed %</div></div>");

    for (var i = 0; i < data.length; i++) {
        graphDiv.append("<div class='percent-graph-pair'>");
        graphDiv.append("<div class='bold'>" + data[i][0]["Key"] + "</div>");

        graphDiv.append("<div><div class='inline percent-graph-number'>" + round(100 * data[i][1]["Value"], 2).toFixed(2) + "%</div><div class='expected-div inline' style='width:" + Math.round(data[i][1]["Value"] * 800) + "px'></div></div>");
        graphDiv.append("<div><div class='inline percent-graph-number'>" + round(100 * data[i][0]["Value"], 2).toFixed(2) + "%</div><div class='observed-div inline' style='width:" + Math.round(data[i][0]["Value"] * 800) + "px'></div></div>");


        graphDiv.append("</div>");//end percent-graph-pair div
    }

}


var scatterPlot = function (data, ref) {
    console.log("from scatter: ", data);
    var target = ref.nextAll(".graph-div:first");
    target.empty();

    var radius = 2;

    if (data.length > 10000)
    {
        radius = 1;
    }

    var w = 500;
    var h = 500;

    var padding = 40;

    var xScale = d3.scale.linear()
                     .domain([0, d3.max(data, function (d) { return d[0]; })])
                     .range([padding, w - padding]);

    var yScale = d3.scale.linear()
                     .domain([0, d3.max(data, function (d) { return d[1]; })])
                     .range([h - padding, padding]);


    var svg = d3.select("#" + ref.nextAll(".graph-div:first").attr("id"))
            .append("svg")
            .attr("width", w)
            .attr("height", h);

   svg.selectAll("circle")
   .data(data)
   .enter()
   .append("circle")
   .attr("cx", function (d) {
       return xScale(d[0]);
    })
   .attr("cy", function (d) {
       return yScale(d[1]);
   })
   .attr("r", radius);
   
   var yAxis = d3.svg.axis()
                  .scale(yScale)
                  .orient("left")
                  .ticks(5);
   svg.append("g")
    .attr("class", "axis")
    .attr("transform", "translate(" + 40 + ",0)")
    .call(yAxis);

    svg.append("text")
      .attr("transform", "rotate(-90)")
      .attr("y", 0 - 40)
      .attr("x",0 - (h / 2))
      .attr("dy", "1em")
      .style("text-anchor", "middle")
      .text(ref.attr("js-data-2"));

   var xAxis = d3.svg.axis()
                 .scale(xScale)
                 .orient("bottom")
                 .ticks(5);

    svg.append("g")
        .attr("class", "axis")
        .attr("transform", "translate(0," + (h - padding) + ")")
        .call(xAxis);

    svg.append("text")
    .attr("transform",
          "translate(" + (w / 2) + " ," +
                         (h ) + ")")
    .style("text-anchor", "middle")
    .text(ref.attr("js-data-1"));

}


//var getAnovaMeans = function(data, ref)
//{
//    var graphDiv = ref.nextAll(".graph-div:first");
//    graphDiv.empty();
//    graphDiv.append("<table>");

//    graphDiv.append("<tr><th colspan='2'>Means</th></tr>");

//    for (var i = 0; i < data.length; i++)
//    {
//        graphDiv.append("<tr>");
//        graphDiv.append("<td>" + data[i]["Key"] + "</td><td>" + data[i]["Value"] + "</td>");
//        graphDiv.append("<tr>");
//    }

//    graphDiv.append("</table>");
//}

var dataLoad = function (ref) {
    var target = ref.nextAll(".graph-div:first");

    target.append("<div id='data-load'></div>");

}


var removeLoader = function (ref) {
    var target = ref.nextAll(".graph-div:first");
    target.empty();
}

var scatterPlotService = function (ref) {
    var variable1 = ref.attr("js-data-1");
    var variable2 = ref.attr("js-data-2");
    var statId = ref.attr("js-stat-id");
    var switchBool = ref.attr("js-switch");
    console.log("stat-Id: ", statId);
    var path = ref.attr("js-data-path");

    nextSelect = $(this);

    $.ajax({
        type: "POST",
        url: "/api/DataPoints/ReturnPearson",
        data: { Path: path, Variable1: variable1, Variable2: variable2, StatId: statId, Switch: switchBool }

    })
    .done(function (data) {
        removeLoader(ref);
        scatterPlot(data, ref);
    })
    .fail(function () {
        alert("Something Went Wrong!!!");
    });

}


$(document.body).on("click", ".js-scatterplot", function () {
    //console.log($(this).attr("js-data-1"));
    var ref = $(this);
    ref.nextAll(".switch-axes-after:first").toggle();
    ref.nextAll(".js-before-axis").toggle();
    dataLoad(ref);
    ref.prev().toggle();
    ref.hide();
    scatterPlotService(ref);

});


$(document.body).on("click", ".hide-show-scatter", function () {
    $(this).nextAll(".js-scatter-div:first").toggle();
    $(this).nextAll(".switch-axes-after").toggle();

});

var switchAxes = function (ref) {
    var scatterVarObject = ref.prevAll(".js-scatterplot:first");
    console.log(scatterVarObject);
    if (scatterVarObject.attr("js-switch") == "false") {
        scatterVarObject.attr("js-switch", "true");
    }
    else {
        scatterVarObject.attr("js-switch", "false");
    }

    var hold = scatterVarObject.attr("js-data-2");



    scatterVarObject.attr("js-data-2", scatterVarObject.attr("js-data-1"));
    scatterVarObject.attr("js-data-1", hold);

    var xAxisElement = ref.prevAll(".x-axis-var:first").text();
    var yAxisElement = ref.prevAll(".y-axis-var:first").text();
    console.log("x axis elemnt: ", xAxisElement);

    ref.prevAll(".x-axis-var:first").text(yAxisElement);
    ref.prevAll(".y-axis-var:first").text(xAxisElement);
}


$(document.body).on("click", ".switch-axes", function () {
    var ref = $(this);
    switchAxes(ref);

});

$(document.body).on("click", ".switch-axes-after", function () {

    var scatterRef = $(this).prevAll(".js-scatterplot:first");
    var target = $(this).nextAll(".graph-div:first");
    target.empty();
    dataLoad($(this));
    switchAxes($(this));

    scatterPlotService(scatterRef);
});


$(document.body).on("click", ".get-chi-tables", function () {
    var ref = $(this);
    dataLoad(ref);
    ref.next(".hide-show-link").toggle();
    ref.toggle();
    var chiId = ref.attr("js-id");
    $.ajax({
        url: "/api/DataPoints/GetChiTablesData/" + chiId,
        cache: false
    })
     .done(function (data) {
         removeLoader(ref);
         chiTableData(data, ref, 0, "Expected Frequency");
         chiTableData(data, ref, 1, "Observed Frequency");
     }).fail(function () {
         alert("yikes, no bueno");
     });

});

$(document.body).on("click", ".get-chi-percentages", function () {
    var ref = $(this);
    dataLoad(ref);
    ref.next(".hide-show-link").toggle();
    ref.toggle();
    var chiId = ref.attr("js-id");
    $.ajax({
        url: "/api/DataPoints/GetChiPercentages/" + chiId,
        cache: false
    })
     .done(function (data) {
         removeLoader(ref);
         chiPercentages(data, ref);
     }).fail(function () {
         alert("yikes, no bueno");
     });

});


var getBarGraph = function (data, ref) {
    //find longest word
    var longest = 0;
    for (var i = 0; i < data.length; i++) {
        if (data[i]["Key"].length > longest) {
            longest = data[i]["Key"].length * 10;
        };
    }

    var classUnique = new Date().valueOf();

    var graphDiv = ref.nextAll(".graph-div:first");
    graphDiv.empty();
    graphDiv.append("<svg class='chart" + classUnique +"'></svg>");



    if (data.length < 5)
    {
        var margin = { top: 20, right: 30, bottom: 30, left: 40 },
        width = 300 - margin.left - margin.right,
        height = 500 - margin.top - margin.bottom;
    }
    else if (data.length > 4 && data.length < 10){
        var margin = { top: 20, right: 30, bottom: 30, left: 40 },
        width = 600 - margin.left - margin.right,
        height = 500 - margin.top - margin.bottom;
    }
    else if (data.length > 9 && data.length < 20) {
        var margin = { top: 20, right: 30, bottom: 30, left: 40 },
        width = 960 - margin.left - margin.right,
        height = 500 - margin.top - margin.bottom;
    }
    else {
        var margin = { top: 20, right: 30, bottom: 30, left: 40 },
        width = data.length*35 - margin.left - margin.right,
        height = 500 - margin.top - margin.bottom;
    }

    var x = d3.scale.ordinal()
    .rangeRoundBands([0, width], 0.1);

    var y = d3.scale.linear()
        .range([height, 0]);

    var xAxis = d3.svg.axis()
    .scale(x)
    .orient("bottom");

    var yAxis = d3.svg.axis()
        .scale(y)
        .orient("left");

    var chart = d3.select(".chart" + classUnique)
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
      .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");


    x.domain(data.map(function (d) { return d["Key"]; }));
    y.domain([0, d3.max(data, function (d) { return d["Value"]; })]);


    chart.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(0," + height + ")")
        .call(xAxis)
    .selectAll("text")
        //this rotates x axis labeling, but doesn't line up well with bars
        .attr("y", 0)
        .attr("x", 9)
        .attr("dy", ".35em")
        .attr("transform", "rotate(90)")
        .style("text-anchor", "start")
    ;

    chart.append("g")
        .attr("class", "y axis")
        .call(yAxis);

    chart.selectAll(".bar")
        .data(data)
      .enter().append("rect")
        .attr("class", "bar")
        .attr("x", function (d) { return x(d["Key"]); })
        .attr("y", function (d) { return y(d["Value"]); })
        .attr("height", function (d) { return height - y(d["Value"]); })
        .attr("width", x.rangeBand());

    d3.select(".chart" + classUnique).attr("height", height + longest);
}


//copied from function above
$(document.body).on("click", ".get-anova-bar", function () {

    var ref = $(this);
    dataLoad(ref);
    ref.next(".hide-show-link").toggle();
    ref.toggle();
    var chiId = ref.attr("js-id");
    $.ajax({
        url: "/api/DataPoints/GetAnovaMeans/" + chiId,
        cache: false
    })
     .done(function (data) {
         removeLoader(ref);
         //getAnovaMeans(data, ref);
         getBarGraph(data, ref);
     }).fail(function () {
         alert("yikes, no bueno");
     });

});


//$(document.body).on("click", ".get-anova-means", function () {
//    var ref = $(this);
//    dataLoad(ref);
//    ref.next(".hide-show-link").toggle();
//    ref.toggle();
//    var chiId = ref.attr("js-id");
//    $.ajax({
//        url: "/api/DataPoints/GetAnovaMeans/" + chiId,
//        cache: false
//    })
//     .done(function (data) {
//         removeLoader(ref);
//         getAnovaMeans(data, ref);
//     }).fail(function () {
//         alert("yikes, no bueno");
//     });

//});


var saveSuccess = function (selector) {
    var target = $(selector);
    target.empty();

    target.append("<div class='bold'>Saved!</div>");
    target.fadeOut(2000);
}

var dataLoadSave = function (selector) {
    var target = $(selector);
    console.log("data load save: ", selector);
    target.append("<div id='data-load'></div>");

}



$(document.body).on("click", ".js-save-report", function () {
    var reportId = $(this).attr("js-report-id");
    dataLoadSave("#js-saved-message");

    $.ajax({
        url: "/api/DataPoints/SaveToReports/" + reportId,
        cache: false
    })
    .done(function () {
        saveSuccess("#js-saved-message");
     }).fail(function () {
         alert("yikes, no bueno");
     });


});



$('#File').bind('change', function () {
    var fileName = '';
    fileName = $(this).val();
    $('#file-selected').html(fileName);
});

$(document.body).on("click", ".login-index", function () {
    $("#login-form").toggle(500);

});

$(document.body).on("click", ".js-show-next", function () {
    $(this).next().toggle(500);

});