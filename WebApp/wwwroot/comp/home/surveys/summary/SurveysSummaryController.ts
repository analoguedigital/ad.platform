
module App {
    "use strict";

    interface ISurveysSummaryController {
        title: string;
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        currentUser: Models.IOrgUser;
        assignment: Models.IProjectAssignment;
        activate: () => void;
    }

    class SurveysSummaryController implements ISurveysSummaryController {
        title: string = "Surveys";
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        currentUser: Models.IOrgUser;
        assignment: Models.IProjectAssignment;

        static $inject: string[] = ['$scope', '$stateParams', 'toastr', 'formTemplateResource',
            'surveyResource', 'projectSummaryPrintSessionResource', 'userContextService', 'project'];
        constructor(
            public $scope: ng.IScope,
            public $stateParams: ng.ui.IStateParamsService,
            public toastr: any,
            private formTemplateResource: Resources.IFormTemplateResource,
            private surveyResource: Resources.ISurveyResource,
            private projectSummaryPrintSessionResource: Resources.IProjectSummaryPrintSessionResource,
            private userContextService: Services.IUserContextService,
            private project: Models.IProject) {

            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            var orgUser = this.userContextService.current.orgUser;
            if (orgUser != null) {
                this.currentUser = orgUser;
                this.assignment = _.find(orgUser.assignments, { 'projectId': this.project.id });
            } else {
                this.assignment = <Models.IProjectAssignment>{
                    orgUserId: this.userContextService.current.user.id,
                    canView: true,
                    canAdd: true,
                    canEdit: true,
                    canDelete: true
                };
            }

            if (this.project != null) {
                this.formTemplateResource.query({ projectId: this.project.id }).$promise
                    .then((templates) => {
                        this.formTemplates = templates;
                    }, (err) => {
                        this.toastr.error(err.data);
                        console.log(err)
                    });
                this.surveyResource.query({ projectId: this.project.id }).$promise
                    .then((surveys) => {
                        this.surveys = surveys;
                    }, (err) => {
                        this.toastr.error(err.data);
                        console.error(err);
                    });
            }
            else {
                this.formTemplateResource.query({ projectId: null }).$promise
                    .then((templates) => {
                        this.formTemplates = templates;
                    }, (err) => {
                        this.toastr.error(err.data);
                        console.log(err)
                    });
                this.surveyResource.query({ projectId: null }).$promise
                    .then((surveys) => {
                        this.surveys = surveys;
                    }, (err) => {
                        this.toastr.error(err.data);
                        console.error(err);
                    });
            }
        }

        delete(id: string) {
            this.surveyResource.delete({ id: id },
                () => { this.load(); },
                (err) => {
                    console.error(err);
                    this.toastr.error(err.data);
                });
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