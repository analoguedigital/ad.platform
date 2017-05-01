module App.Models {
    "use strict";

    export interface IDataListRelationship extends ng.resource.IResource<IDataListRelationship> {
        id: string;
        name: string;
        dataListId: string;
        dataList: IDataList;
        order: number;
    }
}