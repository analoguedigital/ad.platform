module App.Models {
    "use strict";

    export interface IOrganisation extends ng.resource.IResource<IOrganisation> {
        id: string;
        name: string;
        rootUser: IOrgUser;
        telNumber: string;
        addressLine1: string;
        addressLine2: string;
        town: string;
        county: string;
        postcode: string;
        defaultLanguageId: string;
        defaultCalendarId: string;
        subscriptionEnabled: boolean;
        subscriptionMonthlyRate?: number;
    }
}