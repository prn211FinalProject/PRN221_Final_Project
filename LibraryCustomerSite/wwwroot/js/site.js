// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
"use strict";

var con = new signalR.HubConnectionBuilder().withUrl("/hub").build();
con.on("LoadAll", function (bookId) {
    var currentBookId = window.location.pathname.split('/').pop();
    console.log("Received LoadAll event for bookId:", bookId);
    console.log("Current bookId:", currentBookId);
    if (currentBookId == bookId) {
        location.reload();
    }
});
con.start().then(function () {
    console.log("SignalR Connected.");
}).catch(function (err) {
    return console.error(err.toString());
});