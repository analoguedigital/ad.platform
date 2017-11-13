module App.Models {
    "use strict";

    export interface IAttachment {

        id: string;
        url: string;
        fileName: string;
        typeString: string;
        fileSize: number;
        isDeleted: boolean;
    }
}