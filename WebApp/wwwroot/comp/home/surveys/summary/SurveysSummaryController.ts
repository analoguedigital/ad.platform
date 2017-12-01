
module App {
    "use strict";

    interface ISurveysSummaryController {
        title: string;
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        activate: () => void;
    }

    class SurveysSummaryController implements ISurveysSummaryController {
        title: string = "Surveys";
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];

        static $inject: string[] = ['$scope', '$stateParams', 'toastr', 'formTemplateResource',
            'surveyResource', 'projectSummaryPrintSessionResource', 'project'];
        constructor(
            public $scope: ng.IScope,
            public $stateParams: ng.ui.IStateParamsService,
            public toastr: any,
            private formTemplateResource: Resources.IFormTemplateResource,
            private surveyResource: Resources.ISurveyResource,
            private projectSummaryPrintSessionResource: Resources.IProjectSummaryPrintSessionResource,
            private project: Models.IProject) {

            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            var projectId = null;
            if (this.project != null) {
                projectId = this.project.id;
            }

            this.formTemplateResource.query({ projectId: projectId }).$promise
                .then((templates) => {
                    this.formTemplates = templates;
                }, (err) => {
                    this.toastr.error(err.data);
                    console.log(err)
                });
            this.surveyResource.query({ projectId: projectId }).$promise
                .then((surveys) => {
                    this.surveys = surveys;
                }, (err) => {
                    this.toastr.error(err.data);
                    console.error(err);
                });
        }

        delete(id: string) {
            this.surveyResource.delete({ id: id },
                () => { this.load(); },
                (err) => {
                    console.error(err);
                    this.toastr.error(err.data);
                });
        }

        getDescriptionHeading(id: string) {
            var template = _.filter(this.formTemplates, (t) => { return t.id === id; });
            if (template.length) {
                var metricTitles = [];
                let descFormat = template[0].descriptionFormat;
                var pattern = /{{\s*([^}]+)\s*}}/g;
                var segment;

                while (segment = pattern.exec(descFormat))
                    metricTitles.push(segment[1]);

                return metricTitles.join(' - ');
            }

            return "Your record";
        }
    }

    angular.module("app").controller("surveysSummaryController", SurveysSummaryController);
}