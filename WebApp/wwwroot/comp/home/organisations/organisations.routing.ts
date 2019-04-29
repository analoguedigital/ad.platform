
((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.organisations", {
                abstract: true,
                url: "/organisations",
                template: "<ui-view />"
            })
            .state("home.organisations.list", <App.Models.IAppRoute>{
                url: "",
                templateUrl: "comp/home/organisations/organisationsView.html",
                controller: "organisationsController",
                ncyBreadcrumb: { label: 'Organisations' },
                module: "private"
            })
            .state("home.organisations.edit", <App.Models.IAppRoute>{
                url: "/edit/:id",
                templateUrl: "comp/home/organisations/edit/organisationEditView.html",
                controller: "organisationEditController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Edit', parent: 'home.organisations.list' },
                module: "private"
            })
            .state("home.organisations.assignments", <App.Models.IAppRoute>{
                url: "/assignments/:id",
                templateUrl: "comp/home/organisations/assignments/organisationAssignmentsView.html",
                controller: "organisationAssignmentsController",
                ncyBreadcrumb: { label: 'Assignments', parent: 'home.organisations.list' },
                module: 'private'
            })
            .state("home.organisations.vouchers", <App.Models.IAppRoute>{
                url: "/vouchers",
                templateUrl: "comp/home/organisations/vouchers/vouchersView.html",
                controller: "organisationVouchersController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Directory' },
                module: 'private'
            })
            .state("home.organisations.vouchersEdit", <App.Models.IAppRoute>{
                url: "/vouchers/edit/:id",
                templateUrl: "comp/home/organisations/vouchers/edit/vouchersEditView.html",
                controller: "organisationVouchersEditController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Edit Voucher', parent: 'home.organisations.vouchers' },
                module: 'private'
            })
            .state("home.organisations.invitations", <App.Models.IAppRoute>{
                url: "/invitations/:organisationId",
                templateUrl: "comp/home/organisations/invitations/invitationsView.html",
                controller: "organisationInvitationsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Invitation Tokens' },
                module: 'private'
            })
            .state("home.organisations.invitationsEdit", <App.Models.IAppRoute>{
                url: "/invitations/edit/:id",
                templateUrl: "comp/home/organisations/invitations/edit/invitationsEditView.html",
                controller: "organisationInvitationsEditController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Edit Invitation', parent: 'home.organisations.invitations' },
                module: 'private'
            })
            .state("home.organisations.connectionRequests", <App.Models.IAppRoute>{
                url: "/connection-requests/:organisationId",
                templateUrl: "comp/home/organisations/connectionRequests/connectionRequestsView.html",
                controller: "orgConnectionRequestsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Connection Requests' },
                module: 'private'
            })
            .state("home.organisations.requests", <App.Models.IAppRoute>{
                url: "/requests",
                templateUrl: "comp/home/organisations/requests/orgRequestsView.html",
                controller: "orgRequestsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Requests' },
                module: 'private'
            });
    }
})();