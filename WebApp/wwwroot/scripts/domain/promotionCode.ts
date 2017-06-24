module App.Models {
    "use strict";

    export class PromotionCode {
        public id: string;
        public title: string;
        public code: string;
        public amount: number;
        public isRedeemed: boolean;
        public paymentRecordId: string;
        public organisationId: string;
        public dateCreated: number;
        public dateUpdated: number;

        constructor() {
            this.id = '';
            this.title = '';
            this.code = '';
            this.amount = 0;
            this.isRedeemed = false;
            this.paymentRecordId = '';
            this.organisationId = '';
            this.dateCreated = _.now();
            this.dateUpdated = _.now();
        }
    }

    export interface IPromotionCode extends ng.resource.IResource<IPromotionCode> {
        id: string;
        title: string;
        code: string;
        amount: number;
        isRedeemed: boolean;
        paymentRecordId: string;
        organisationId: string;
        dateCreated: number;
        dateUpdated: number;
    }

    export interface IRedeemCodeResponse {
        success: boolean;
        message: string;
    }
}