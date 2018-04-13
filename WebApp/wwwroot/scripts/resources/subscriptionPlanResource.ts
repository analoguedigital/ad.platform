module App.Resources {
    "use strict";

    export interface ISubscriptionPlanResource extends ng.resource.IResourceClass<Models.ISubscriptionPlan> {
        update(plan: Models.ISubscriptionPlan, success: Function, error?: Function): Models.ISubscriptionPlan;
    }

    SubscriptionPlanResource.$inject = ["$resource"];
    export function SubscriptionPlanResource($resource: ng.resource.IResourceService): ISubscriptionPlanResource {
        return <ISubscriptionPlanResource>$resource('/api/subscriptionplans/:id', { id: '@id' }, {
            'update': { method: 'PUT' },
        });
    }
    
    angular.module("app").factory("subscriptionPlanResource", SubscriptionPlanResource);
}