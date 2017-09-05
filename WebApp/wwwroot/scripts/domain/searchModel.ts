module App.Models {
    "use strict";

    export class SearchModel {
        public projectId: string;
        public formTemplateId: string;
        public term: string;
        public startDate?: Date;
        public endDate?: Date;
        public filterValues: Models.IFilterValue[]

        constructor(filledById: string, projectId: string, formTemplateId: string) {
            this.projectId = '';
            this.formTemplateId = '';
            this.term = '';
            this.startDate = null;
            this.endDate = null;
            this.filterValues = [];
        }
    }

    export class SummarySearchModel {
        public projectId: string;
        public formTemplateIds: string[];
        public term: string;
        public startDate?: Date;
        public endDate?: Date;
        public filterValues: Models.IFilterValue[]

        constructor(filledById: string, projectId: string, formTemplateId: string) {
            this.projectId = '';
            this.formTemplateIds = [];
            this.term = '';
            this.startDate = null;
            this.endDate = null;
            this.filterValues = [];
        }
    }
}