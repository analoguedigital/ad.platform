module App.Resources {
    "use strict";

    export interface IPromotionCodeResource extends ng.resource.IResourceClass<Models.IPromotionCode> {
        redeem(params: Object, success: Function, error?: Function);
    }

    PromotionCodeResource.$inject = ["$resource"];
    export function PromotionCodeResource($resource: ng.resource.IResourceService): IPromotionCodeResource {
        return <IPromotionCodeResource>$resource('/api/promotionCodes/', null, {
            'redeem': { method: 'POST', url: '/api/promotionCodes/redeem/:code', params: { code: '@code' } }
        });
    }
    
    angular.module("app").factory("promotionCodeResource", PromotionCodeResource);
}