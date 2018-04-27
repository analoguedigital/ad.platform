module App.Resources {
    "use strict";

    export interface ISubscriptionResource extends ng.resource.IResourceClass<Models.ISubscription> {
        getLatest(success: Function, error?: Function);
        getLastSubscription(success: Function, error?: Function);
        buy(params: Object, success: Function, error?: Function);
    }

    SubscriptionResource.$inject = ["$resource"];
    export function SubscriptionResource($resource: ng.resource.IResourceService): ISubscriptionResource {
        return <ISubscriptionResource>$resource('/api/subscriptions', null, {
            'get': { method: 'GET', isArray: true },
            'getLatest': { method: 'GET', url: '/api/subscriptions/getLatest' },
            'getLastSubscription': { method: 'GET', url: '/api/subscriptions/last' },
            'buy': { method: 'POST', url: '/api/subscriptions/buy/:id', params: { id: '@id' } }
        });
    }

    angular.module("app").factory("subscriptionResource", SubscriptionResource);
}