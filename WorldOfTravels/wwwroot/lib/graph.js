// graph.js

function createPopularCountriesGraph(data, id) {
    var width = 400,
        height = 400,
        radius = Math.min(width, height) / 2;

    var color = d3.scaleOrdinal()
        .range(["#a05d56", "#6b486b", "#ff8c00", "#98abc5", "#8a89a6", "#d0743c", "#7b6888"]);

    var arc = d3.arc()
        .outerRadius(radius - 10)
        .innerRadius(0);

    var pie = d3.pie()
        .sort(null)
        .value(function (d) { return d.totalPosts; });

    var svg = d3.select("#" + id).append("svg")
        .attr("width", width)
        .attr("height", height)
        .append("g")
        .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

    data.forEach(function (d) {
        d.totalPosts = +d.totalPosts;
    });

    var g = svg.selectAll(".arc")
        .data(pie(data))
        .enter().append("g")
        .attr("class", "arc");

    g.append("path")
        .attr("d", arc)
        .style("fill", function (d) { return color(d.data.countryName); });

    g.append("text")
        .attr("transform", function (d) { return "translate(" + arc.centroid(d) + ")"; })
        .attr("dy", ".35em")
        .style("text-anchor", "middle")
        .text(function (d) { return d.data.countryName; });
}