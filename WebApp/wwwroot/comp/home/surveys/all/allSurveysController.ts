
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
        startDate: Date;
        endDate: Date;
        startDateCalendar: any;
        endDateCalendar: any;

        activate: () => void;
        delete: (id: string) => void;
        search: () => void;
        openStartDateCalendar: () => void;
        openEndDateCalendar: () => void;
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
        startDate: Date;
        endDate: Date;
        startDateCalendar: any;
        endDateCalendar: any;

        static $inject: string[] = ["project", "formTemplate", "surveyResource", "dataResource"];
        constructor(
            public project: Models.IProject,
            private formTemplate: Models.IFormTemplate,
            private surveyResource: Resources.ISurveyResource,
            private dateResource: Resources.IDataResource) {

            this.title = "AllSurveys";
            this.isDataView = false;
            this.activate();
        }

        activate() {
            this.startDateCalendar = { isOpen: false };
            this.endDateCalendar = { isOpen: false };

            this.load();
        }

        openStartDateCalendar() {
            this.startDateCalendar.isOpen = true;
        }

        openEndDateCalendar() {
            this.endDateCalendar.isOpen = true;
        }

        load() {
            console.log('template', this.formTemplate);

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

        search() {
            // not implemented
        }
    }

    angular.module("app").controller("allSurveysController", AllSurveysController);
}