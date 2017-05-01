
module App.Models.MBS {
    "use strict";

    export interface ITarget extends ng.resource.IResource<ITarget> {
        weekTargetDate: Date;
        description: string;
        howAchieved: string;
        supervisoryTargets: string;
        howOthersHelp: string;
    }
}