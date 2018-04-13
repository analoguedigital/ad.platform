module App.Models {
    "use strict";

    export class SubscriptionPlan {
        public id: string;
        public name: string;
        public description: string;
        public price: number;
        public length: number;
        public isLimited: boolean;
        public monthlyQuota?: number;
        public pdfExport: boolean;
        public zipExport: boolean;
        public dateCreated: number;
        public dateUpdated: number;

        constructor() {
            this.id = '';
            this.name = '';
            this.description = '';
            this.price = 0;
            this.length = 1;
            this.isLimited = false;
            this.monthlyQuota = null;
            this.pdfExport = false;
            this.zipExport = false;
            this.dateCreated = _.now();
            this.dateUpdated = _.now();
        }
    }

    export interface ISubscriptionPlan extends ng.resource.IResource<ISubscriptionPlan> {
        id: string;
        name: string;
        description: string;
        price: number;
        length: number;
        isLimited: boolean;
        monthlyQuota?: number;
        pdfExport: boolean;
        zipExport: boolean;
        dateCreated: number;
        dateUpdated: number;
    }
}