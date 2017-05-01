
module App.Resources {
    "use strict";

    export interface IMetricGroupResource extends ng.resource.IResourceClass<Models.IMetricGroup> {
        update(group: Models.IMetricGroup, success: Function, error?: Function): Models.IMetricGroup;
    }

    MetricGroupResource.$inject = ["$resource"];
    export function MetricGroupResource($resource: ng.resource.IResourceService): IMetricGroupResource {

        return <IMetricGroupResource>$resource('/api/formtemplates/:formTemplateId/metricgroups/:id',
            { formTemplateId: '@formTemplateId', id: '@id' },
            {
                'update': { method: 'PUT' }
            });
    }

    angular.module("app").factory("metricGroupResource", MetricGroupResource);
}