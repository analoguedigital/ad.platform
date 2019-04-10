var App;
(function (App) {
    "use strict";
    angular.module("app", [
        "ngResource",
        "ngMessages",
        "ui.bootstrap.tooltip",
        "ui.bootstrap.multiMap",
        "ui.bootstrap.dropdown",
        "ui.bootstrap.position",
        "ui.bootstrap.dateparser",
        "ui.bootstrap.datepicker",
        "ui.bootstrap.datepickerPopup",
        "ui.bootstrap.timepicker",
        "ui.bootstrap.tabs",
        "ui.bootstrap.modal",
        "ui.bootstrap.popover",
        "ui.bootstrap.datetimepicker",
        "ui.bootstrap.progressbar",
        "ngAnimate",
        "ngSanitize",
        "ui.router",
        "ui.sortable",
        "smart-table",
        "ngBootbox",
        "ncy-angular-breadcrumb",
        "LocalStorageModule",
        "ngFileUpload",
        "textAngular",
        "rzModule",
        "toastr",
        "angularMoment",
        "chart.js",
        "uiGmapgoogle-maps",
        "minicolors",
        "jtt_angular_xgallerify",
        "angular-loading-bar",
        "ui.select",
        "mwl.calendar",
        "bm.uiTour",
        "mgo-angular-wizard",
        "hmTouchEvents"
    ]);
})(App || (App = {}));

function load() {
    $('.loading').fadeOut(function () {
        $('.loading').remove();
    });
}
window.onload = load;