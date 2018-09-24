module App.Models {
    "use strict";

    export interface IEmailRecipient extends ng.resource.IResource<IEmailRecipient> {
        id: string;
        orgUserId: Models.IOrgUser;
        orgUserName: string;
        email: string;
        feedbacks: boolean;
        newMobileUsers: boolean;
        orgRequests: boolean;
        orgConnectionRequests: boolean;
    }
}