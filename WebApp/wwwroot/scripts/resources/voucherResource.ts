module App.Resources {
    "use strict";

    export interface IVoucherResource extends ng.resource.IResourceClass<Models.IVoucher> {
        update(voucher: Models.IVoucher, success: Function, error?: Function): Models.IVoucher;
        redeem(params: Object, success: Function, error?: Function);
    }

    PromotionCodeResource.$inject = ["$resource"];
    export function PromotionCodeResource($resource: ng.resource.IResourceService): IVoucherResource {
        return <IVoucherResource>$resource('/api/vouchers/:id', { id: '@id' }, {
            'update': { method: 'PUT' },
            'redeem': { method: 'POST', url: '/api/vouchers/redeem/:code', params: { code: '@code' } }
        });
    }
    
    angular.module("app").factory("voucherResource", PromotionCodeResource);
}