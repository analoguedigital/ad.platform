
module App.Models.MBS {
    "use strict";

    export interface IUpdate extends ng.resource.IResource<IUpdate> {
        date: Date;
        description: string;
    }
}