
module App.Resources {
    "use strict";

    export interface IMetricResource extends ng.resource.IResourceClass<Models.IMetric> {
        update(metric: Models.IMetric, success: Function, error?: Function): Models.IMetric;
    }

    export interface INewMetricResource extends ng.resource.IResourceClass<Models.IMetric> {
        createFreeText(success: Function, error?: Function): Models.IFreeTextMetric;
        createNumeric(success: Function, error?: Function): Models.INumericMetric;
        createRate(success: Function, error?: Function): Models.IRateMetric;
        createDate(success: Function, error?: Function): Models.IDateMetric;
        createTime(success: Function, error?: Function): Models.ITimeMetric;
        createDichotomous(success: Function, error?: Function): Models.IDichotomousMetric;
        createMultipleChoice(success: Function, error?: Function): Models.IMultipleChoiceMetric;
        createAttachment(success: Function, error?: Function): Models.IAttachmentMetric;
    }

    MetricResource.$inject = ["$resource"];
    export function MetricResource($resource: ng.resource.IResourceService): IMetricResource {

        return <IMetricResource>$resource('/api/formtemplates/:formTemplateId/:metricType/:id',
            { formTemplateId: '@formTemplateId', id: '@id', metricType: 'metrics' });
    }

    NewMetricResource.$inject = ["$resource"];
    export function NewMetricResource($resource: ng.resource.IResourceService): IMetricResource {

        return <IMetricResource>$resource('/api/metrics/:metricType',
            { metricType: 'metrics' },
            {
                'createFreeText': { method: 'GET', params: { metricType: 'freeTextMetric' } },
                'createNumeric': { method: 'GET', params: { metricType: 'numericMetric' } },
                'createRate': { method: 'GET', params: { metricType: 'rateMetric' } },
                'createDate': { method: 'GET', params: { metricType: 'dateMetric' } },
                'createTime': { method: 'GET', params: { metricType: 'timeMetric' } },
                'createDichotomous': { method: 'GET', params: { metricType: 'dichotomousMetric' } },
                'createMultipleChoice': { method: 'GET', params: { metricType: 'multipleChoiceMetric' } },
                'createAttachment': { method: 'GET', params: { metricType: 'attachmentMetric' } }
            });
    }

    angular.module("app").factory("metricResource", MetricResource);
    angular.module("app").factory("newMetricResource", NewMetricResource);
}

