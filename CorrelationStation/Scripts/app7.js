$(document.body).on("click", ".js-show-graphs", function () {
    $(this).next(".graph-div").toggle(250);
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


var makeNumeralLinePlot = function (data, ref) {
    var classUnique = new Date().valueOf();

    var graphDiv = ref.nextAll(".graph-div:first");
    graphDiv.empty();

    graphDiv.append("<svg class='chart" + classUnique + "'></svg>");

    var svg = d3.select(".chart" + classUnique),
    margin = { top: 20, right: 80, bottom: 30, left: 50 },
    width = 960 - margin.left - margin.right,
    height = 400 - margin.top - margin.bottom,
    g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    var x = d3.scaleTime().range([0, width]),
    y = d3.scaleLinear().range([height, 0]);


    //*********FIX d.date and d.close******************
    var line = d3.line()
    .x(function (d) { return x(Date.parse(d["Date"])); })
    .y(function (d) { return y(d["Numeral"]); });


    //*********FIX d.date and d.close******************
    //need to get min and max
    //var xMax = d3.max(data, function (d) { return d[0]; })

    x.domain(d3.extent(data, function (d) { return Date.parse(d["Date"]); }));
    y.domain(d3.extent(data, function (d) { return d["Numeral"]; }));


    g.append("g")
    .attr("transform", "translate(0," + height + ")")
    .call(d3.axisBottom(x))
    .select(".domain")
    .remove();

    g.append("g")
    .call(d3.axisLeft(y))
    .append("text")
    .attr("fill", "#000")
    .attr("transform", "rotate(-90)")
    .attr("y", 6)
    .attr("dy", "0.71em")
    .attr("text-anchor", "end")
    //.text("Price ($)");
    g.append("path")
    .datum(data)
    .attr("fill", "none")
    .attr("stroke", "steelblue")
    .attr("stroke-linejoin", "round")
    .attr("stroke-linecap", "round")
    .attr("stroke-width", 1.5)
    .attr("d", line);

    graphDiv.css("height", "500px");
}


var scatterPlot = function (data, ref) {
    globalData = data;
    var classUnique = new Date().valueOf();

    var graphDiv = ref.nextAll(".graph-div:first");
    graphDiv.empty();

    graphDiv.append("<svg class='chart" + classUnique + "'></svg>");

    var radius = 2;

    if (data.length > 10000) {
        radius = 1;
    }

    var svg = d3.select(".chart" + classUnique),
    margin = { top: 20, right: 80, bottom: 30, left: 50 },
    width = 960 - margin.left - margin.right,
    height = 400 - margin.top - margin.bottom,
    g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    var x = d3.scaleLinear()
        .domain([0, d3.max(data, function (d) { return d[0]; })])
        .range([0, width]),

    y = d3.scaleLinear()
        .domain([0, d3.max(data, function (d) { return d[1]; })])
        .range([height, 0]);

    g.append("g")
    .attr("transform", "translate(0," + height + ")")
    .call(d3.axisBottom(x))
    .select(".domain")
    .remove();


    g.append('text')
      .attr("transform",
          "translate(" + (width / 2) + " ," +
                         (height) + ")")
      .attr('dy', 45) // adjust distance from the bottom edge
      .attr('class', 'axis-label')
      .attr('text-anchor', 'middle')
      .text(ref.attr("js-data-1"));

    g.append("g")
    .call(d3.axisLeft(y))
    .append("text")
    .attr("fill", "#000")
    .attr("transform", "rotate(-90)")
    .attr("y", 6)
    .attr("dy", "0.71em")
    .attr("text-anchor", "end")

    g.append('text')
      .attr("transform", "rotate(-90)")
      .attr("y", -100)
      .attr("x", 0 - (height / 2))
      .attr("dy", "1em")
      .style("text-anchor", "middle")
      .text(ref.attr("js-data-2"));

    g.selectAll("circle")
    .data(data)
    .enter()
    .append("circle")
    .attr("cx", function (d) { return x(d[0]); })
    .attr("cy", function (d) { return y(d[1]); })
    .attr("r", radius);


    graphDiv.css("height", "500px");

}



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
    ref.nextAll(".switch-axes-after:first").toggle(200);
    ref.nextAll(".js-before-axis").toggle(200);
    dataLoad(ref);
    ref.prev().toggle(200);
    ref.hide();
    scatterPlotService(ref);

});


$(document.body).on("click", ".hide-show-scatter", function () {
    $(this).nextAll(".js-scatter-div:first").toggle(200);
    $(this).nextAll(".switch-axes-after").toggle(200);

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
    ref.next(".hide-show-link").toggle(200);
    ref.toggle(200);
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
    ref.next(".hide-show-link").toggle(200);
    ref.toggle(200);
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
    globalData = ref;
    
    var longest = 0;
    for (var i = 0; i < data.length; i++) {
        if (data[i]["Key"].length > longest) {
            longest = data[i]["Key"].length * 20;
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
    var svg = d3.select(".chart" + classUnique);
    var g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    //var x = d3.scaleLinear()
    //.domain([0, d3.max(data, function (d) { return d[0]; })])
    //.range([0, width]),

    var y = d3.scaleLinear()
    .range([height, 0]);


    var x = d3.scaleBand()
    .range([0, width])
          .padding(0.1);


    x.domain(data.map(function (d) { return d["Key"]; }));
    y.domain([0, d3.max(data, function (d) { return d["Value"]; })]);


      g.selectAll(".bar")
      .data(data)
    .enter().append("rect")
      .attr("class", "bar")
      .attr("x", function (d) { return x(d["Key"]); })
      .attr("width", x.bandwidth())
      .attr("y", function (d) { return y(d["Value"]); })
      .attr("height", function (d) { return height - y(d["Value"]); })

    // add the x Axis


  g.append("g")
      .attr("transform", "translate(0," + height + ")")
      .call(d3.axisBottom(x))
      .selectAll("text")
        .attr("y", 0)
        .attr("x", 9)
        .attr("dy", ".35em")
        .attr("transform", "rotate(90)")
        .style("text-anchor", "start")
    ;

  // add the y Axis
  g.append("g")
      .call(d3.axisLeft(y));

  //g.axis()
  //g.call().outerTickSize(0)
  //axis.tickSizeOuter(0)
  g.append("text")
      .attr("x", (width / 2))
      .attr("y", 0 - (margin.top / 2))
      .attr("text-anchor", "middle")
      .style("font-size", "16px")
      .style("text-decoration", "underline")
      .text("Average/Mean for each " + ref.attr("js-categorical"));

  d3.select(".chart" + classUnique).attr("height", height + longest);
  console.log(data.length);
  graphDiv.css("width", data.length*35 + "px");
}


//copied from function above
$(document.body).on("click", ".get-anova-bar", function () {

    var ref = $(this);
    dataLoad(ref);
    ref.next(".hide-show-link").toggle(200);
    ref.toggle(200);
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

var childDiv

var toggleIt = function(ref)
{
 
    var targetClass = ref.children()["context"]["textContent"];
    console.log("here", targetClass);
    //$("."+targetClass.replace(/ /g, '') + "category").toggle();
    $("#" + validString(targetClass) + "category").toggle();
}

var changeStroke = function(ref, wide)
{
    var targetClass = ref.children()["context"]["textContent"];
    console.log("here", targetClass);
    //$("."+targetClass.replace(/ /g, '') + "category").toggle();
    if (wide == "wide")
    {
        $("#" + validString(targetClass) + "category").attr("class", "line-wide");
    }
    else {
        $("#" + validString(targetClass) + "category").attr("class", "line");    }
    
}



var validString=  function(string)
{
    string = string.replace(/[^\w\s]/gi, '');
    string = string.replace(/ /g, '');
    return string;
}

var hoverNow = function () {
    $(".legend-div-parent").hover(
        function(){
            changeStroke($(this), "wide");
        },
        function(){
            changeStroke($(this))
        }

        );

}

var makeNumeralLinePlot = function(data, ref)
{
    var classUnique = new Date().valueOf();

    var graphDiv = ref.nextAll(".graph-div:first");
    graphDiv.empty();

    graphDiv.append("<svg class='chart" + classUnique + "'></svg>");

    var svg = d3.select(".chart" + classUnique),
    margin = { top: 20, right: 80, bottom: 30, left: 50 },
    width = 960 - margin.left - margin.right,
    height = 400 - margin.top - margin.bottom,
    g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    var x = d3.scaleTime().range([0, width]),
    y = d3.scaleLinear().range([height, 0]);


    //*********FIX d.date and d.close******************
    var line = d3.line()
    .x(function (d) { return x(Date.parse(d["Date"])); })
    .y(function (d) { return y(d["Numeral"]); });


    //*********FIX d.date and d.close******************
    //need to get min and max
    //var xMax = d3.max(data, function (d) { return d[0]; })

    x.domain(d3.extent(data, function (d) { return Date.parse(d["Date"]); }));
    y.domain(d3.extent(data, function (d) { return d["Numeral"]; }));


    g.append("g")
    .attr("transform", "translate(0," + height + ")")
    .call(d3.axisBottom(x))
    .select(".domain")
    .remove();

    g.append("g")
    .call(d3.axisLeft(y))
    .append("text")
    .attr("fill", "#000")
    .attr("transform", "rotate(-90)")
    .attr("y", 6)
    .attr("dy", "0.71em")
    .attr("text-anchor", "end")
    //.text("Price ($)");
    g.append("path")
    .datum(data)
    .attr("fill", "none")
    .attr("stroke", "steelblue")
    .attr("stroke-linejoin", "round")
    .attr("stroke-linecap", "round")
    .attr("stroke-width", 1.5)
    .attr("d", line);

    graphDiv.css("height", "500px");
}


var makeLinePlot = function (data, ref) {
    //console.log("from makeLinePlot: ", data);

    var classUnique = new Date().valueOf();

    var graphDiv = ref.nextAll(".graph-div:first");
    graphDiv.empty();
    
    graphDiv.append("<div class='div"+ classUnique +"' style='width:960px'></div>");
    graphDiv.append("<svg class='chart" + classUnique + "'></svg>");
    
    var makeALegend = function (name) {
        $(".div" + classUnique).append("<div class='inline-block legend-div-parent'><div class='inline-block legend-div' style='background-color:" + z(name) + ";'></div>" + name + "</div>");
    }

    var svg = d3.select(".chart" + classUnique),
    margin = { top: 20, right: 80, bottom: 30, left: 50 },
    width = 960 - margin.left - margin.right,
    height = 400 - margin.top - margin.bottom,
    g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    //var parseTime = d3.timeParse("%Y%m%d");

    var x = d3.scaleTime().range([0, width]),
        y = d3.scaleLinear().range([height, 0]),
        z = d3.scaleOrdinal(d3.schemeCategory10);

    var line = d3.line()
        //.curve(d3.curveBasis)
        .x(function (d) {  return x(Date.parse(d["MonthAndYear"])); })
        .y(function (d) {  return y(d["Count"]); });

    
    //if (error) throw error;
    var crimes = data;
    var dates = [];
    console.log("dates: ", dates);
    for (var i = 0; i < crimes.length; i++)
    {
        for(var j = 0; j < crimes[i]["DateAndCounts"].length; j++)
        {
            dates.push(crimes[i]["DateAndCounts"][j]["MonthAndYear"]);
        }
    }

    x.domain(d3.extent(dates, function (d) { return Date.parse(d); }));

    y.domain([
        d3.min(crimes, function (c) { return d3.min(c["DateAndCounts"], function (d) { return d["Count"]; }); }),
        d3.max(crimes, function (c) { return d3.max(c["DateAndCounts"], function (d) { return d["Count"]; }); })
    ]);

    z.domain(crimes.map(function (c) { return c["Name"]; }));

    g.append("g")
        .attr("class", "axis axis--x")
        .attr("transform", "translate(0," + height + ")")
        .call(d3.axisBottom(x));

    g.append("g")
        .attr("class", "axis axis--y")
        .call(d3.axisLeft(y))
        .append("text")
        .attr("transform", "rotate(-90)")
        .attr("y", 6)
        .attr("dy", "0.71em")
        .attr("fill", "#000")
        .text("# instances");

    var city = g.selectAll(".city")
        .data(crimes)
        .enter().append("g")
        .attr("class", "city");

    //need to give each path an id and enlarge with when hovering above legends

    city.append("path")
        .attr("class", "line")
        .attr("id", function (d) { return validString(d["Name"]) + "category" })
        .attr("d", function (d) { return line(d["DateAndCounts"]); })
        .style("stroke", function (d) { makeALegend(d["Name"]); return z(d["Name"]); });

    city.append("text")
        .datum(function (d) {  return { id: d["Name"], value: d["DateAndCounts"][d["DateAndCounts"].length - 1] }; })
        .attr("transform", function (d) { return "translate(" + x(Date.parse(d.value["MonthAndYear"])) + "," + y(d.value["Count"]) + ")"; })
        .attr("x", 3)
        .attr("dy", "0.35em")
        .style("font", "10px sans-serif")
        //.attr("id", function (d) { return validString(d["id"]) + "category"; })
        //.text(function (d) {  return d["id"]; });

    //d3.select(".chart" + classUnique).attr("height", height + longest);
    graphDiv.css("height", (480 + $(".div" + classUnique).height()) + "px");
    //function type(d, _, columns) {
    //    d.date = parseTime(d.date);
    //    for (var i = 1, n = columns.length, c; i < n; ++i) d[c = columns[i]] = +d[c];
    //    return d;
    //}
    hoverNow();



}

////get-date-numeral-plot

$(document.body).on("click", ".get-date-numeral-plot", function () {

    var ref = $(this);
    dataLoad(ref);
    ref.next(".hide-show-link").toggle(200);
    ref.toggle(200);
    var chiId = ref.attr("js-id");
    $.ajax({
        url: "/api/DataPoints/GetNumeralLinePlot/" + chiId,
        cache: false
    })
        .done(function (data) {
            console.log("from controller: ", data);
            //removeLoader(ref);
            //getAnovaMeans(data, ref);
            makeNumeralLinePlot(data, ref);

        }).fail(function () {
            alert("yikes, no bueno");
        });

});


$(document.body).on("click", ".get-date-multiline-plot", function () {

    var ref = $(this);
    dataLoad(ref);
    ref.next(".hide-show-link").toggle(200);
    ref.toggle(200);
    var chiId = ref.attr("js-id");
    $.ajax({
        url: "/api/DataPoints/GetDateCategoryLinePlot/" + chiId,
        cache: false
    })
        .done(function (data) {
            console.log("from controller: ", data);
            //removeLoader(ref);
            //getAnovaMeans(data, ref);
            makeLinePlot(data, ref);
             
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


var saveSuccess = function (selector, ref) {
    var target = $(selector);
    target.empty();

    target.append("<div class='bold'>Saved!</div>");
    target.fadeOut(2000);
    ref.fadeOut(2000);
}

var dataLoadSave = function (selector) {
    var target = $(selector);
    console.log("data load save: ", selector);
    target.append("<div id='data-load'></div>");

}



$(document.body).on("click", ".js-save-report", function () {
    var reportId = $(this).attr("js-report-id");
    dataLoadSave("#js-saved-message");
    var ref = $(this);
    $.ajax({
        url: "/api/DataPoints/SaveToReports/" + reportId,
        cache: false
    })
    .done(function () {
        saveSuccess("#js-saved-message", ref);
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


var removeElement = function (ref) {
    ref.parent().parent().fadeOut(300);
}

$(document.body).on("click", ".glyphicon-remove", function () {
    var ref = $(this);
    var id = $(this).attr("js-delete-id");
    $.ajax({
        url: "/api/DataPoints/RemoveStatSummary/" + id,
        cache: false
    })
    .done(function (ref) {
        removeElement(ref);
    }).fail(function () {
        alert("yikes, no bueno");
    });


});


