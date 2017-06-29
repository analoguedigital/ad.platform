module App.Services {
    "use strict";

    export interface IProjectSummaryServiceResultView {
        surveys: Models.ISurvey[];
        templates: Models.IFormTemplate[];
    }

    export interface IProjectSummaryService {
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];

        query: string;
        fromDate: Date;
        toDate: Date;

        filter: () => ng.IPromise<IProjectSummaryServiceResultView>;
    }

    export class ProjectSummaryService implements IProjectSummaryService {
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        query: string;
        fromDate: Date;
        toDate: Date;

        static $inject: string[] = ["$q"];
        constructor(private $q: ng.IQService) { }

        filter() {
            var deferred = this.$q.defer();

            // the recordings table is bound to this collection
            let records: Models.ISurvey[] = [];

            // select checked threads
            let threads: Models.IFormTemplate[] = _.filter(this.formTemplates, (t) => { return t.isChecked == true });

            // extract thread surveys
            let surveys = [];
            angular.forEach(threads, (t) => {
                let filledForms = _.filter(this.surveys, (s) => { return s.formTemplateId == t.id });
                angular.forEach(filledForms, (form) => { surveys.push(form); });
            });

            // apply filters and collect surveys
            let collectedRecords = [];
            let resultRecords = [];

            // search term
            if (this.query == undefined || this.query.length < 1) {
                collectedRecords = surveys;
            } else {
                _.forEach(surveys, (survey: Models.ISurvey) => {
                    if (_.includes(survey.serial.toString(), this.query))
                        collectedRecords.push(survey);

                    if (_.includes(survey.description, this.query))
                        collectedRecords.push(survey);

                    _.forEach(survey.formValues, (fm) => {
                        if (fm.textValue && fm.textValue.length) {
                            if (_.includes(fm.textValue, this.query)) {
                                collectedRecords.push(survey);
                            }
                        }
                    });
                });
            }

            // date range filter
            angular.forEach(collectedRecords, (survey: Models.ISurvey, index) => {
                // has start date
                let _surveyDate = survey.surveyDate.setHours(0, 0, 0, 0);

                if (this.fromDate == undefined && this.toDate == undefined) {
                    resultRecords.push(survey);
                }

                if (this.fromDate && this.toDate == undefined) {
                    if (_surveyDate >= this.fromDate.setHours(0, 0, 0, 0)) {
                        resultRecords.push(survey);
                    }
                }

                // has end date
                if (this.toDate && this.fromDate == undefined) {
                    if (_surveyDate <= this.toDate.setHours(0, 0, 0, 0))
                        resultRecords.push(survey);
                }

                // has date range
                if (this.fromDate && this.toDate) {
                    if (_surveyDate >= this.fromDate.setHours(0, 0, 0, 0) && _surveyDate <= this.toDate.setHours(0, 0, 0, 0))
                        resultRecords.push(survey);
                }
            });

            // this will be the displayed record
            let result = _.uniqBy(resultRecords, (rec: Models.ISurvey) => { return rec.id });

            let resultView: IProjectSummaryServiceResultView = {
                surveys: result,
                templates: threads
            };

            deferred.resolve(resultView);

            return deferred.promise;
        }
    }

    angular.module("app").service("projectSummaryService", ProjectSummaryService);
}