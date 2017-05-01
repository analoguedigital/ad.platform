
module App.Models {
    "use strict";

    export interface IDataListItemAttr extends ng.resource.IResource<IDataListItemAttr> {
        id: string;
        relationshipId: string;
        valueId: string;
    }
}