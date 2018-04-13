module App {
    "use strict";

    interface ISubscribeController {
        plans: Models.ISubscriptionPlan[];

        activate: () => void;
        close: () => void;
    }

    class SubscribeController implements ISubscribeController {
        plans: Models.ISubscriptionPlan[];

        static $inject: string[] = ["$uibModalInstance", "$uibModal", "toastr", "subscriptionPlans"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private $uibModal: ng.ui.bootstrap.IModalService,
            public toastr: any,
            private subscriptionPlans: Models.ISubscriptionPlan[]) {
            this.activate();
        }

        activate() {
            if (this.subscriptionPlans)
                this.plans = this.subscriptionPlans;
        }

        purchase(plan: Models.ISubscriptionPlan) {
            this.close();

            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/myAccount/addSubscription/addSubscriptionView.html',
                controller: 'addSubscriptionController',
                controllerAs: 'ctrl',
                resolve: {
                    plan: () => { return plan; }
                }
            }).result.then(
                (res) => {
                    location.reload(true);
                },
                (err) => { });
        }

        joinOrganisation() {
            this.close();

            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/myAccount/joinOrganisation/joinOrganisationView.html',
                controller: 'joinOrganisationController',
                controllerAs: 'ctrl'
            }).result.then(
                (res) => {
                    location.reload(true);
                },
                (err) => {
                    console.error(err);
                });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("subscribeController", SubscribeController);
}