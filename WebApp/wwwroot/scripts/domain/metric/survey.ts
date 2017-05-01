module App.Models {
    "use strict";

    export interface ISurvey extends ng.resource.IResource<ISurvey> {
        id: string;
        serial: number;
        error: string;
        locations: IPosition[];
        dateCreated: number;
        dateUpdated: number;
        surveyDate: Date;
        filledById: string;
        filledBy: string;
        projectId: string;
        formTemplateId: string;
        formValues: IFormValue[];
        isSubmitted: boolean;
    }

    export interface IPosition {
        latitude: number;
        longitude: number;
        accuracy: number;
        error: string;
        event: string;
    }

    export interface IFormValue {
        metricId: string;
        rowNumber: number;
        rowDataListItemId: string;
        textValue: string;
        dateValue: Date;
        timeValue: Date;
        boolValue: boolean;
        numericValue: number;
        guidValue: string;
        attachments: IAttachment[];
    }
}