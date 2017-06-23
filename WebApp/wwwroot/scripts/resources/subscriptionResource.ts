module App.Resources {
    "use strict";

    export interface ISubscriptionResource extends ng.resource.IResourceClass<Models.ISubscription> {
        getExpiry(params: Object, success: Function, error?: Function): Models.ISubscriptionExpiry;
    }

    SubscriptionResource.$inject = ["$resource"];
    export function SubscriptionResource($resource: ng.resource.IResourceService): ISubscriptionResource {
        return <ISubscriptionResource>$resource('/api/subscriptions/:userId', { userId: '@userId' }, {
            'get': { method: 'GET', isArray: true },
            'getExpiry': { method: 'GET', url: '/api/subscriptions/getExpiry/:userId', params: { userId: '@userId' } }
        });
    }

    angular.module("app").factory("subscriptionResource", SubscriptionResource);
}