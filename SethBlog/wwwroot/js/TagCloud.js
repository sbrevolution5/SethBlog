﻿//This code was altered, but the original was found on https://stackoverflow.com/questions/20705036/how-do-i-create-link-of-each-word-in-d3-cloud

//var fill = d3.scale.category20();
var myColors = d3.scaleSequential().domain([1,10]).range(["purple", "gray"])
var words = cloudArray
var width = 800;
var height = 300;
for (var i = 0; i < words.length; i++) {
    words[i].size = 10 + words[i].count * 9;
}

d3.layout.cloud()
    .size([width, height])
    .words(words)
    .padding(5)
    .rotate(function () { return ~~(Math.random() * 2) * 90; })
    .font("Impact")
    .fontSize(function (d) { return d.size; })
    .on("end", draw)
    .start();

function draw(words) {
    d3.select("#tagCloud")
        .append("svg")
        .attr("width", width)
        .attr("height", height)
        .attr("class", "tagCloud")
        .append("g")
        .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")")
        .selectAll("text")
        .data(words)
        .enter()
        .append("text")
        .style("font-size", function (d) { return d.size + "px"; })
        .style("font-family", "Impact")
        .style("fill", function (d, i) { return myColors(i); })
        .attr("text-anchor", "middle")
        .attr("transform", function (d) {
            return "translate(" + [d.x, d.y] + ")rotate(" + d.rotate + ")";
        })
        .text(function (d) { return d.text; })
        .on("click", function (d, i) {
            window.open(i.url, "_blank");
        });
}