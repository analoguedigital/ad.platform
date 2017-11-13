
module App.Models {
    "use strict";

    export interface IDataList extends ng.resource.IResource<IDataList> {
        id: string;
        name: string;
        items: IDataListItem[];

        relationships: IDataListRelationship[];
    }

    export interface IDataListBasic {
        id: string;
        name: string;
    }
}