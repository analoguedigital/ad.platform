
module App {
    "use strict";

    export interface INewSurveyScope extends ng.IScope {
        ctrl: INewSurveyController;
    }

    interface INewSurveyController {
        title: string;
        pages: _.Dictionary<Models.IMetricGroup[]>;
        tabs: any[];
        survey: Models.ISurvey;
        activeTabIndex: number;
        activate: () => void;
        addFormValue: (metric, rowDataListItem, rowNumber) => Models.IFormValue;
    }

    class NewSurveyController implements INewSurveyController {
        title: string = "Forms";
        pages: _.Dictionary<Models.IMetricGroup[]>;
        tabs: any[];
        //survey: Models.ISurvey;
        activeTabIndex: number;

        static $inject: string[] = ["$state", "surveyResource", "formTemplate", "project", "survey"];

        constructor(
            private $state: ng.ui.IStateService,
            private surveyResource: Resources.ISurveyResource,
            public formTemplate: Models.IFormTemplate,
            public project: Models.IProject,
            public survey: Models.ISurvey) {

            if (survey == null) {
                this.survey = <Models.ISurvey>{};
                this.survey.serial = null;
                this.survey.formValues = [];
                this.survey.surveyDate = new Date();
                this.survey.formTemplateId = formTemplate.id;
                this.survey.projectId = project.id;

                //TODO: should not be based on the formtemplate
            }
            this.activate();
        }

        activate() {
            var pageGroups = _.groupBy(this.formTemplate.metricGroups, (mg) => { return mg.page });
            this.tabs = _.map(Object.keys(pageGroups), (pageNumber) => { return { number: pageNumber, title: "Page " + pageNumber, active: false }; });
            this.tabs[0].active = true;
        }

        addFormValue(metric, rowDataListItem, rowNumber) {
            var formValue = <Models.IFormValue>{};
            formValue.textValue = '';
            formValue.metricId = metric.id;
            formValue.rowNumber = rowNumber;
            if (rowDataListItem)
                formValue.rowDataListItemId = rowDataListItem.id;
            this.survey.formValues.push(formValue);
            return formValue;
        };

        next() {
            if (this.tabs[this.tabs.length - 1].active)
                return;

            var activeTab = _.find(this.tabs, { "active": true });
            this.tabs[activeTab.number].active = true;
        }

        previous() {
            if (this.tabs[0].active)
                return;

            var activeTab = _.find(this.tabs, { "active": true });
            this.tabs[activeTab.number - 2].active = true;
        }

        submit(form: ng.IFormController) {
            if (form.$invalid)
                return;

            if (this.survey.id == null) {
                this.surveyResource.save(this.survey).$promise
                    .then(
                    () => {
                        this.$state.go('home.surveys.list.summary', { projectId: this.project.id });
                    },
                    (err) => { console.log(err); });
            }
            else {
                this.surveyResource.update(this.survey,
                    () => {
                        this.$state.go('home.surveys.list.summary', { projectId: this.survey.projectId });
                    },
                    (err) => { console.log(err); });
            }

        }
    }

    angular.module("app").controller("newSurveyController", NewSurveyController);
}