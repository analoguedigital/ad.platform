
((): void => {
    "use strict";

    angular.module("app")
        .config(ConfigRoutes);

    ConfigRoutes.$inject = ["$stateProvider", "$urlRouterProvider"];
    function ConfigRoutes($stateProvider: angular.ui.IStateProvider,
        $urlRouterProvider: angular.ui.IUrlRouterProvider) {

        $stateProvider
            .state("home.surveys", {
                abstract: true,
                url: "/surveys",
                template: "<ui-view />"
            })
            .state("home.surveys.list", {
                url: "",
                templateUrl: "comp/home/surveys/surveysView.html",
                controller: "surveysController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Forms' }
            })
            .state("home.surveys.list.summary", {
                url: "/:projectId",
                templateUrl: "comp/home/surveys/summary/surveysSummaryView.html",
                controller: "surveysSummaryController",
                controllerAs: "ctrl",
                resolve: {
                    project:
                    ['$stateParams', 'projectResource',
                        ($stateParams, projectResource: App.Resources.IProjectResource) => {
                            return projectResource.get({ id: $stateParams['projectId'] }).$promise.then((data) => {
                                return data;
                            });
                        }]
                },
                ncyBreadcrumb: { label: 'Summary' }
            })
            .state("home.surveys.new", {
                url: "/:projectId/new/:formTemplateId",
                templateUrl: "comp/home/surveys/new/newSurveyView.html",
                controller: "newSurveyController",
                controllerAs: "ctrl",
                resolve: {
                    formTemplate:
                    ['$stateParams', 'formTemplateResource',
                        ($stateParams, formTemplateResource: App.Resources.IFormTemplateResource) => {
                            return formTemplateResource.get({ id: $stateParams['formTemplateId'] }).$promise.then((data) => {
                                return data;
                            });
                        }], project:
                    ['$stateParams', 'projectResource',
                        ($stateParams, projectResource: App.Resources.IProjectResource) => {
                            return projectResource.get({ id: $stateParams['projectId'] }).$promise.then((data) => {
                                return data;
                            });
                        }],
                    survey: () => { return null; }
                },
                ncyBreadcrumb: { label: 'New' }
            })
            .state("home.surveys.list.all", {
                url: "/:projectId/all/:formTemplateId",
                templateUrl: "comp/home/surveys/all/allSurveysView.html",
                controller: "allSurveysController",
                controllerAs: "ctrl",
                resolve: {
                    formTemplate:
                    ['$stateParams', 'formTemplateResource',
                        ($stateParams, formTemplateResource: App.Resources.IFormTemplateResource) => {
                            return formTemplateResource.get({ id: $stateParams['formTemplateId'] }).$promise.then((data) => {
                                return data;
                            });
                        }],
                    project:
                    ['$stateParams', 'projectResource',
                        ($stateParams, projectResource: App.Resources.IProjectResource) => {
                            return projectResource.get({ id: $stateParams['projectId'] }).$promise.then((data) => {
                                return data;
                            });
                        }]

                },
                ncyBreadcrumb: { label: 'All' }
            })
            .state("home.surveys.edit", {
                url: "/edit/:surveyId",
                templateUrl: "comp/home/surveys/new/newSurveyView.html",
                controller: "newSurveyController",
                controllerAs: "ctrl",
                resolve: {
                    formTemplate:
                    ['$stateParams', 'formTemplateResource', 'surveyResource',
                        ($stateParams,
                            formTemplateResource: App.Resources.IFormTemplateResource,
                            surveyResource: App.Resources.ISurveyResource) => {

                            return surveyResource.get({ id: $stateParams['surveyId'] }).$promise
                                .then((survey) => {
                                    return formTemplateResource.get({ id: survey.formTemplateId }).$promise.then((data) => {
                                        return data;
                                    });
                                });
                        }],
                    survey:
                    ['$stateParams', 'surveyResource',
                        ($stateParams,
                            surveyResource: App.Resources.ISurveyResource) => {

                            return surveyResource.get({ id: $stateParams['surveyId'] }).$promise.then((data) => {
                                return data;
                            });
                        }],
                    project: () => { return null; }

                },
                ncyBreadcrumb: { label: 'Edit' }
            });
    }
})();