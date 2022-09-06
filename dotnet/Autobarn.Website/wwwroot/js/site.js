// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(connectToSignalR);

function connectToSignalR() {
    var conn = new signalR.HubConnectionBuilder().withUrl("/hub").build();
    conn.on("ThisIsAnotherSpecialMagicString", showShinyNotification);
    conn.start().then(function () {
        console.log("Connected to SignalR!");
    }).catch(function (err) {
        console.log("SignalR connection failed!");
        console.log(err);
    });
}

function showShinyNotification(user, message) {
    console.log(user);
    console.log(message);
    console.log("SHOWING SHINY NOTIFICATION");
    var data = JSON.parse(message);
    var html = `<div>New vehicle! 
${data.ManufacturerName} ${data.ModelName}, 
${data.Color}, ${data.Year}. Price ${data.Price}${data.Currency}<br />
<a href="/vehicles/details/${data.Registration}">click for more...</a></div>`;
    const $div = $(html);
    $div.css("background-color", data.Color);
    console.log(html);
    const $target = $("div#signalr-notifications");
    $target.prepend($div);
    window.setTimeout(function () {
        $div.fadeOut(500, function () { $div.remove() });
    }, 5000);
}