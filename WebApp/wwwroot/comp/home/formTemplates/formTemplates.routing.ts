
((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.formtemplates", {
                abstract: true,
                url: "/formtemplates",
                template: "<ui-view />"
            })
            .state("home.formtemplates.list", <App.Models.IAppRoute>{
                url: "",
                templateUrl: "comp/home/formTemplates/formTemplatesView.html",
                controller: "formTemplatesController",
                controllerAs: 'ctrl',
                ncyBreadcrumb: { label: 'Form templates' },
                module: "private"
            })
            .state("home.formtemplates.design", <App.Models.IAppRoute>{
                url: "/design/:id",
                templateUrl: "comp/home/formTemplates/design/formDesignView.html",
                controller: "formDesignController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Design', parent: 'home.formtemplates.list' },
                module: "private"
            })
            .state("home.formtemplates.edit", <App.Models.IAppRoute>{
                url: "/edit/:id",
                templateUrl: "comp/home/formTemplates/edit/formTemplateEditView.html",
                controller: "formTemplateEditController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Edit', parent: 'home.formtemplates.list' },
                module: "private"
            })
            .state("home.formtemplates.assignments", <App.Models.IAppRoute>{
                url: "/assignments/:id",
                templateUrl: "comp/home/formTemplates/assignments/formTemplateAssignmentsView.html",
                controller: "formTemplateAssignmentsController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Assign', parent: 'home.caseManagement.threadAssignments' },
                module: 'private'
            });
    }
})();