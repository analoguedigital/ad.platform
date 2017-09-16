
module App {
    "use strict";

    interface ISurveysSummaryController {
        title: string;
        project: Models.IProject;
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        currentUser: Models.IOrgUser;
        assignment: Models.IProjectAssignment;

        printSelected: () => void;
        activate: () => void;
    }

    class SurveysSummaryController implements ISurveysSummaryController {
        title: string = "Surveys";
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        currentUser: Models.IOrgUser;
        assignment: Models.IProjectAssignment;

        static $inject: string[] = ['$state', 'toastr', 'project', 'formTemplateResource', 'surveyResource', 'userContextService'];

        constructor(
            public $state: ng.ui.IStateService,
            public toastr: any,
            public project: Models.IProject,
            private formTemplateResource: Resources.IFormTemplateResource,
            private surveyResource: Resources.ISurveyResource,
            private userContextService: Services.IUserContextService) {

            this.activate();
        }

        activate() {
            this.load();
        }

        load() {

            if (this.project == null)
                return;

            var orgUser = this.userContextService.current.orgUser;
            this.currentUser = orgUser;
            this.assignment = _.find(orgUser.assignments, { 'projectId': this.project.id });

            this.assignment.canAdd = true;
            this.assignment.canEdit = true;
            this.assignment.canDelete = true;
            this.assignment.canView = true;

            this.formTemplates = this.formTemplateResource.query({ projectId: this.project.id });
            this.surveyResource.query({ projectId: this.project.id }).$promise
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

    }

    angular.module("app").controller("surveysSummaryController", SurveysSummaryController);
}