
module App {
    "use strict";

    interface ISubscriptionsControllerScope extends ng.IScope {

    }

    interface ISubscriptionsController {
        payments: Models.IPaymentRecord[];
        subscriptions: Models.ISubscription[];
        latestSubscription?: Date;

        activate: () => void;
        redeemCode: () => void;
    }

    class SubscriptionsController implements ISubscriptionsController {
        payments: Models.IPaymentRecord[] = [];
        subscriptions: Models.ISubscription[] = [];
        latestSubscription?: Date;

        static $inject: string[] = ["$scope", "$uibModal", "paymentResource", "userContextService", "subscriptionResource"];
        constructor(
            private $scope: ISubscriptionsControllerScope,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private paymentResource: Resources.IPaymentResource,
            private userContext: Services.IUserContextService,
            private subscriptionResource: Resources.ISubscriptionResource) {

            $scope.title = "Subscriptions";
            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            let userId = this.userContext.current.user.id;

            this.paymentResource.query().$promise
                .then((payments) => { this.payments = payments; });

            this.subscriptionResource.getLatest(
                (res) => {
                    this.latestSubscription = res.date;
                });
        }

        redeemCode() {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/myAccount/redeemCode/redeemCode.html',
                controller: 'redeemCodeController',
                controllerAs: 'ctrl'
            }).result.then(
                (res) => { this.load(); },
                (err) => { });
        }
    }

    angular.module("app").controller("subscriptionsController", SubscriptionsController);
}