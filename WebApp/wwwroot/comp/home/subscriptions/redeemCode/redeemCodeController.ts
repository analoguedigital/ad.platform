module App {
    "use strict";

    interface IRedeemCodeController {
        user: Models.IUser;
        activate: () => void;
        processCode: (form: ng.IFormController) => void;
        close: () => void;
    }

    interface IRedeemCodeModel {
        userId: string;
        code: string;
    }

    class RedeemCodeController implements IRedeemCodeController {
        model: IRedeemCodeModel;

        static $inject: string[] = ["$uibModalInstance", "$resource", "userContextService", "user"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private $resource: ng.resource.IResourceService,
            private userContext: Services.IUserContextService,
            public user: Models.IUser) {

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
            console.info(this.model);
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("redeemCodeController", RedeemCodeController);
}