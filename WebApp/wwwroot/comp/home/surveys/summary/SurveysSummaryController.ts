
module App {
    "use strict";

    interface ISurveysSummaryController {
        title: string;
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        currentUser: Models.IOrgUser;
        assignment: Models.IProjectAssignment;
        projectId: string;
        printSelected: () => void;
        activate: () => void;
    }

    class SurveysSummaryController implements ISurveysSummaryController {
        title: string = "Surveys";
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        currentUser: Models.IOrgUser;
        assignment: Models.IProjectAssignment;
        projectId: string;

        static $inject: string[] = ['$state', '$stateParams', 'toastr', 'formTemplateResource', 'surveyResource', 'userContextService'];

        constructor(
            public $state: ng.ui.IStateService,
            public $stateParams: ng.ui.IStateParamsService,
            public toastr: any,
            private formTemplateResource: Resources.IFormTemplateResource,
            private surveyResource: Resources.ISurveyResource,
            private userContextService: Services.IUserContextService) {

            this.activate();
        }

        activate() {
            this.projectId = this.$stateParams['projectId'];
            this.load();
        }

        load() {
            if (!this.projectId)
                return;

            var orgUser = this.userContextService.current.orgUser;
            this.currentUser = orgUser;
            this.assignment = _.find(orgUser.assignments, { 'projectId': this.projectId });

            this.formTemplateResource.query({ projectId: this.projectId }).$promise
                .then((templates) => {
                    this.formTemplates = templates;
                }, (err) => {
                    console.log(err)
                });
            this.surveyResource.query({ projectId: this.projectId }).$promise
                .then((surveys) => {
                    this.surveys = surveys;
                }, (err) => {
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

        printSelected() {
            let selectedSurveys = this.surveys.filter((survey) => survey.isChecked == true);
            let result = selectedSurveys.map((survey) => { return survey.id; });

            this.$state.go('home.surveys.print-multiple', { selectedSurveys: result });
        }

        getTemplateColour(id: string) {
            let template = _.find(this.formTemplates, (t) => { return t.id == id; });
            if (template && template.colour && template.colour.length)
                return template.colour;

            return '';
        }

    }

    angular.module("app").controller("surveysSummaryController", SurveysSummaryController);
}