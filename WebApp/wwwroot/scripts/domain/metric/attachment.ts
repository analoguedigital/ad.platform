module App.Models {
    "use strict";

    export interface IAttachment {

        id: string;
        oneTimeAccessId: string;
        fileName: string;
        typeString: string;
        fileSize: number;
        isDeleted: boolean;
    }
}