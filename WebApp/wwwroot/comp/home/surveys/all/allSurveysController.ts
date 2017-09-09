
module App {
    "use strict";

    interface IAllSurveysController {
        title: string;
        searchTerm: string;
        surveys: Models.ISurvey[];
        surveysData: string[];
        surveysDataHeaders: string[];
        displayedSurveys: Models.ISurvey[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
        isDataView: boolean;
        activate: () => void;
        delete: (id: string) => void;
        currentUser: Models.IOrgUser;
        assignment: Models.IProjectAssignment;
    }

    class AllSurveysController implements IAllSurveysController {

        title: string;
        searchTerm: string;
        surveys: Models.ISurvey[];
        surveysData: string[];
        displayedSurveysData: string[];
        surveysDataHeaders: string[];
        displayedSurveys: Models.ISurvey[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
        isDataView: boolean;
        currentUser: Models.IOrgUser;
        assignment: Models.IProjectAssignment;

        static $inject: string[] = ["project", "formTemplate", "surveyResource", "dataResource", "userContextService"];
        constructor(
            public project: Models.IProject,
            private formTemplate: Models.IFormTemplate,
            private surveyResource: Resources.ISurveyResource,
            private dateResource: Resources.IDataResource,
            private userContextService: Services.IUserContextService) {

            this.title = "AllSurveys";
            this.isDataView = false;
            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            var orgUser = this.userContextService.current.orgUser;
            this.currentUser = orgUser;
            this.assignment = _.find(orgUser.assignments, { 'projectId': this.project.id });

            this.surveyResource.query({ projectId: this.project.id }).$promise.then((surveys) => {
                this.surveys = _.filter(surveys, { formTemplateId: this.formTemplate.id });
                this.displayedSurveys = [].concat(this.surveys);

                this.dateResource.query({ projectId: this.project.id, formTemplateId: this.formTemplate.id }).$promise.then((dataRows) => {
                    this.surveysDataHeaders = dataRows[0];
                    this.surveysData = dataRows.slice(1);
                    this.displayedSurveysData = [].concat(this.surveysData);
                })

            });

        }

        delete(id: string) {
            this.surveyResource.delete({ id: id },
                () => { this.load(); },
                (err) => { console.log(err); });
        }
    }

    angular.module("app").controller("allSurveysController", AllSurveysController);
}