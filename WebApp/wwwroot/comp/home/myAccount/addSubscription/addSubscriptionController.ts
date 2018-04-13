module App {
    "use strict";

    interface IAddSubscriptionController {
        subscriptionPlan: Models.ISubscriptionPlan;

        activate: () => void;
        close: () => void;
    }

    class AddSubscriptionController implements IAddSubscriptionController {
        subscriptionPlan: Models.ISubscriptionPlan;

        static $inject: string[] = ["$uibModalInstance", "toastr", "subscriptionPlanResource", "subscriptionResource", "plan"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            public toastr: any,
            private subscriptionPlanResource: Resources.ISubscriptionPlanResource,
            private subscriptionResource: Resources.ISubscriptionResource,
            private plan: Models.ISubscriptionPlan) {
            this.activate();
        }

        activate() {
            this.subscriptionPlan = this.plan;
        }

        checkout() {
            this.subscriptionResource.buy({ id: this.plan.id }, (res) => {
                this.$uibModalInstance.close(res);
            }, (err) => {
                console.error(err);
            });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("addSubscriptionController", AddSubscriptionController);
}