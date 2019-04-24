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
        date: Date;
        filledById: string;
        filledBy: string;
        projectId: string;
        formTemplateId: string;
        formValues: IFormValue[];
        isSubmitted: boolean;
        isChecked: boolean;
        description: string;
        isRead?: boolean;
    }

    export interface IPosition {
        latitude: number;
        longitude: number;
        accuracy: number;
        address: string;
        error: string;
        event: string;
    }

    export interface ISurveyLocation extends App.Models.IPosition {
        id: string;
        color: string;
        description: string;
        serial: number;
        date: Date;
    }

    export interface IFormValue {
        id: string;
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