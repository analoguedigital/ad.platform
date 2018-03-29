module App.Models {
    "use strict";

    export interface IOrgUserType extends ng.resource.IResource<IOrgUserType> {
        id: string;
        name: string;
    }
}