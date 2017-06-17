module App.ProjectSummaryController.Models {
    export interface Error {
        key: string;
        value: string;
    }

    export interface ISurveyLocation extends App.Models.IPosition {
        color: string;
        description: string;
    }
}