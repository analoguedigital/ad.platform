module App.Resources {
    "use strict";

    export interface ISubscriptionResource extends ng.resource.IResourceClass<Models.ISubscription> {
        getQuota(success: Function, error?: Function);
        getLatest(success: Function, error?: Function);
        getLastSubscription(success: Function, error?: Function);
        getUserSubscriptions(params: Object, success: Function, error?: Function);
        buy(params: Object, success: Function, error?: Function);
    }

    SubscriptionResource.$inject = ["$resource"];
    export function SubscriptionResource($resource: ng.resource.IResourceService): ISubscriptionResource {
        return <ISubscriptionResource>$resource('/api/subscriptions', null, {
            'get': { method: 'GET', isArray: true },
            'getUserSubscriptions': { method: 'GET', url: '/api/subscriptions/user/:id', params: { id: '@id' }, isArray: true },
            'getLatest': { method: 'GET', url: '/api/subscriptions/getLatest' },
            'getLastSubscription': { method: 'GET', url: '/api/subscriptions/last' },
            'getQuota': { method: 'GET', url: '/api/subscriptions/quota' },
            'buy': { method: 'POST', url: '/api/subscriptions/buy/:id', params: { id: '@id' } }
        });
    }

    angular.module("app").factory("subscriptionResource", SubscriptionResource);
}