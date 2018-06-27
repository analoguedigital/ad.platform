module App.Models {
    "use strict";

    export interface IOrgConnectionRequest extends ng.resource.IResource<IOrgConnectionRequest> {
        id: string;
        orgUser: Models.IOrgUser;
        organisation: Models.IOrganisation;
        isApproved: boolean;
        approvalDate?: Date;
    }
}