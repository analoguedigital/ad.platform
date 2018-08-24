
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
            .state("home.surveys.list", <App.Models.IAppRoute>{
                url: "",
                templateUrl: "comp/home/surveys/surveysView.html",
                controller: "surveysController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Directory of Threads' },
                module: "private"
            })
            .state("home.surveys.list.summary", <App.Models.IAppRoute>{
                url: "/:projectId",
                templateUrl: "comp/home/surveys/summary/surveysSummaryView.html",
                controller: "surveysSummaryController",
                controllerAs: "ctrl",
                ncyBreadcrumb: { label: 'Summary' },
                resolve: {
                    project:
                    ['$stateParams', 'projectResource',
                        ($stateParams, projectResource: App.Resources.IProjectResource) => {
                            var projectId = $stateParams['projectId'];
                            if (projectId == null || projectId.length < 1)
                                return null;

                            return projectResource.get({ id: projectId }).$promise.then((data) => {
                                return data;
                            });
                        }]
                },
                module: "private"
            })
            .state("home.surveys.new", <App.Models.IAppRoute>{
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
                            //return projectResource.get({ id: $stateParams['projectId'] }).$promise.then((data) => {
                            //    return data;
                            //});

                            return projectResource.getDirect({ id: $stateParams['projectId'] }).$promise.then((data) => {
                                return data;
                            });
                        }],
                    survey: () => { return null; }
                },
                ncyBreadcrumb: { label: 'New' },
                module: "private"
            })
            .state("home.surveys.list.all", <App.Models.IAppRoute>{
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
                ncyBreadcrumb: { label: 'All records' },
                module: "private"
            })
            .state("home.surveys.edit", <App.Models.IAppRoute> {
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
                ncyBreadcrumb: { label: 'Edit' },
                module: "private"
            })
            .state("home.surveys.view", <App.Models.IAppRoute>{
                url: "/view/:surveyId",
                templateUrl: "comp/home/surveys/view/survey.html",
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
                ncyBreadcrumb: { label: 'View' },
                module: "private"
            })

            .state("printMaster", {
                abstract: true,
                url: "/surveys/print",
                template: "<ui-view/>"
            })

            .state("home.surveys.print-single", <App.Models.IAppRoute>{
                parent: "printMaster",
                url: "/:surveyId",
                templateUrl: "comp/home/surveys/print/print-single.html",
                controller: "printSurveyController",
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
                        }]
                },
                module: "private"
            })

            .state("home.surveys.print-multiple", <App.Models.IAppRoute>{
                parent: "printMaster",
                params: {
                    selectedSurveys: null
                },
                templateUrl: "comp/home/surveys/print/print-multiple.html",
                controller: "printSurveysController",
                controllerAs: "ctrl",
                module: "private"
            });
    }
})();