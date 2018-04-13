module App {
    "use strict";

    interface IVoucherModel {
        code: string;
    }

    interface IRedeemCodeController {
        model: IVoucherModel;

        activate: () => void;
        processCode: (form: ng.IFormController) => void;
        close: () => void;
    }

    class RedeemCodeController implements IRedeemCodeController {
        model: IVoucherModel;

        static $inject: string[] = ["$uibModalInstance", "voucherResource", "toastr"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private voucherResource: Resources.IVoucherResource,
            public toastr: any) {
            this.activate();
        }

        activate() { }

        processCode(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            this.voucherResource.redeem({ code: this.model.code },
                (res) => {
                        this.toastr.success('Voucher redeemed!');
                        this.$uibModalInstance.close(res);
                },
                (err) => {
                    if (err.status == 404) {
                        this.toastr.error('Voucher code is not valid!');
                    }

                    if (err.status == 403) {
                        this.toastr.error(err.data);
                    }
                });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("redeemCodeController", RedeemCodeController);
}