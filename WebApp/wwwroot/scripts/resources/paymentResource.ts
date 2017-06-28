module App.Resources {
    "use strict";

    export interface IPaymentResource extends ng.resource.IResourceClass<Models.IPaymentRecord> {
        
    }

    PaymentResource.$inject = ["$resource"];
    export function PaymentResource($resource: ng.resource.IResourceService): IPaymentResource {
        return <IPaymentResource>$resource('/api/payments', null, {
            'get': { method: 'GET', isArray: true }
        });
    }

    angular.module("app").factory("paymentResource", PaymentResource);
}