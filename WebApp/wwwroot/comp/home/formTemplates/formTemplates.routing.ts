
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
            .state("home.formtemplates.list", {
                url: "",
                templateUrl: "comp/home/formtemplates/formtemplatesView.html",
                controller: "formTemplatesController",
                ncyBreadcrumb: { label: 'Form templates' }
            })
            .state("home.formtemplates.design", {
                url: "/design/:id",
                templateUrl: "comp/home/formtemplates/design/formDesignView.html",
                controller: "formDesignController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Design', parent: 'home.formtemplates.list' }
            })
            .state("home.formtemplates.edit", {
                url: "/edit/:id",
                templateUrl: "comp/home/formtemplates/edit/formTemplateEditView.html",
                controller: "formTemplateEditController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Edit', parent: 'home.formtemplates.list' }
            });
    }
})();