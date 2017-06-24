module App {
    "use strict";

    interface IRedeemCodeModel {
        userId: string;
        code: string;
    }

    interface IRedeemCodeController {
        user: Models.IUser;
        model: IRedeemCodeModel;

        activate: () => void;
        processCode: (form: ng.IFormController) => void;
        close: () => void;
    }

    class RedeemCodeController implements IRedeemCodeController {
        model: IRedeemCodeModel;

        static $inject: string[] = ["$uibModalInstance", "promotionCodeResource", "userContextService", "user", "toastr"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private promotionCodeResource: Resources.IPromotionCodeResource,
            private userContext: Services.IUserContextService,
            public user: Models.IUser,
            public toastr: any) {

            if (user === null) {
                this.user = userContext.current.user;
            }
            this.activate();
        }

        activate() { }

        processCode(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            this.model.userId = this.user.id;

            this.promotionCodeResource.redeemCode({ userId: this.model.userId, code: this.model.code },
                (res: Models.IRedeemCodeResponse) => {
                    if (res.success) {
                        this.toastr.success(res.message);
                        this.$uibModalInstance.close(res);
                    }

                    this.toastr.error(res.message);
                },
                (err) => {
                    console.error(err);
                });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("redeemCodeController", RedeemCodeController);
}