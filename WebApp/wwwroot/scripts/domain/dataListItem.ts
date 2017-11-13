
module App.Models {
    "use strict";

    export interface IDataListItem extends ng.resource.IResource<IDataListItem> {
        id: string;
        text: string;
        description: string;
        value: number;
        order: number;
        attributes: IDataListItemAttr[];

        isDeleted: boolean;
    }
}