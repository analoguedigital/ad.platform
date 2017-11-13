module App.Models {
    "use strict";

    export class SearchDTO {
        public projectId: string;
        public formTemplateIds: string[];
        public term: string;
        public startDate?: Date;
        public endDate?: Date;
        public filterValues: Models.IFilterValue[]

        constructor() {
            this.projectId = '';
            this.formTemplateIds = [];
            this.term = '';
            this.startDate = null;
            this.endDate = null;
            this.filterValues = [];
        }
    }
}