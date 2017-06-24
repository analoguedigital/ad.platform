module App.Resources {
    "use strict";

    export interface IPromotionCodeResource extends ng.resource.IResourceClass<Models.IPromotionCode> {
        redeemCode(params: Object, success: Function, error?: Function): Models.IRedeemCodeResponse;
    }

    PromotionCodeResource.$inject = ["$resource"];
    export function PromotionCodeResource($resource: ng.resource.IResourceService): IPromotionCodeResource {
        return <IPromotionCodeResource>$resource('/api/promotionCodes/', null, {
            'get': { method: 'GET', isArray: true },
            'redeemCode': { method: 'POST', url: '/api/promotionCodes/redeemCode/:userId/:code', params: { userId: '@userId', code: '@code' } }
        });
    }

    angular.module("app").factory("promotionCodeResource", PromotionCodeResource);
}