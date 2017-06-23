﻿module App.Models {
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
}