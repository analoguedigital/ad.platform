module App.Models {
    "use strict";

    export interface IOrgInvitation extends ng.resource.IResource<IOrgInvitation> {
        id: string;
        name: string;
        token: string;
        limit: number;
        used: number;
        isActive: boolean;
        organisation: Models.IOrganisation;
    }
}