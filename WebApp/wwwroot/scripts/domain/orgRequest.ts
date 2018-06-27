module App.Models {
    "use strict";

    export interface IOrgRequest extends ng.resource.IResource<IOrgRequest> {
        id: string;
        name: string;
        address: string;
        contactName: string;
        email: string;
        telNumber: string;
        postcode: string;
        orgUser: Models.IOrgUser;
    }
}