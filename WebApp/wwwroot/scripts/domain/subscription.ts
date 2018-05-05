module App.Models {
    "use strict";

    export class Subscription {
        public id: string;
        public startDate: Date;
        public endDate: Date;
        public note: string;
        public paymentRecordId: string;
        public orgUserId: string;
        public dateCreated: number;
        public dateUpdated: number;

        constructor() {
            this.id = '';
            this.startDate = new Date();
            this.endDate = new Date();
            this.note = '';
            this.paymentRecordId = '';
            this.orgUserId = '';
            this.dateCreated = _.now();
            this.dateUpdated = _.now();
        }
    }

    export interface ISubscription extends ng.resource.IResource<ISubscription> {
        id: string;
        startDate: Date;
        endDate: Date;
        note: string;
        paymentRecordId: string;
        orgUserId: string;
        dateCreated: number;
        dateUpdated: number;
    }

    export interface IMonthlyQuota extends ng.resource.IResource<IMonthlyQuota> {
        quota?: number;
        used: number;
    }

    export interface ISubscriptionEntry extends ng.resource.IResource<ISubscriptionEntry> {
        startDate: Date;
        endDate?: Date;
        price?: number;
        note: string;
        reference: string;
        type: number;
        isActive: boolean;
        subscriptionPlan: ISubscriptionPlan;
    }
}