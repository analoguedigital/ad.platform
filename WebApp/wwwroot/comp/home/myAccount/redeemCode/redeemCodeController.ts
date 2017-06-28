module App {
    "use strict";

    interface IRedeemCodeModel {
        code: string;
    }

    interface IRedeemCodeController {
        model: IRedeemCodeModel;

        activate: () => void;
        processCode: (form: ng.IFormController) => void;
        close: () => void;
    }

    class RedeemCodeController implements IRedeemCodeController {
        model: IRedeemCodeModel;

        static $inject: string[] = ["$uibModalInstance", "promotionCodeResource", "toastr"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private promotionCodeResource: Resources.IPromotionCodeResource,
            public toastr: any) {
            this.activate();
        }

        activate() { }

        processCode(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            this.promotionCodeResource.redeem({ code: this.model.code },
                (res) => {
                        this.toastr.success('Promotion Code redeemed!');
                        this.$uibModalInstance.close(res);
                },
                (err) => {
                    if (err.status == 404) {
                        this.toastr.error('Promotion Code is not valid!');
                    }
                });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("redeemCodeController", RedeemCodeController);
}