module App.Models {
    "use strict";

    export class PaymentRecord {
        public id: string;
        public date: Date;
        public amount: number;
        public reference: string;
        public note: string;
        public orgUserId: string;
        public dateCreated: number;
        public dateUpdated: number;

        constructor() {
            this.id = '';
            this.date = new Date();
            this.amount = 0;
            this.reference = '';
            this.note = '';
            this.orgUserId = '';
            this.dateCreated = _.now();
            this.dateUpdated = _.now();
        }
    }

    export interface IPaymentRecord extends ng.resource.IResource<IPaymentRecord> {
        id: string;
        date: Date;
        amount: number;
        reference: string;
        note: string;
        orgUserId: string;
        dateCreated: number;
        dateUpdated: number;
    }
}