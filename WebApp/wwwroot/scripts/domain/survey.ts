
module App.Models {
    "use strict";

    export class Survey {

        public id: string;
        public error: string;
        public dateCreated: number;
        public dateUpdated: number;
        public surveyDate: Date;
        public filledById: string;
        public projectId: string;
        public formTemplateId: string;
        public formValues: FormValue[];
        public isSubmitted: boolean;

        constructor(filledById: string, projectId: string, formTemplateId: string) {

            this.id = _.now().toString();
            this.dateCreated = _.now();
            this.dateUpdated = _.now();
            this.surveyDate = new Date();
            this.formValues = [];
            this.isSubmitted = false;
            this.filledById = filledById;
            this.projectId = projectId;
            this.formTemplateId = formTemplateId;
        }
    }
}