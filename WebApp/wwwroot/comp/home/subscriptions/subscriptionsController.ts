﻿
module App {
    "use strict";

    interface ISubscriptionsControllerScope extends ng.IScope {

    }

    interface ISubscriptionsController {
        payments: Models.IPaymentRecord[];
        subscriptions: Models.ISubscription[];
        subscriptionExpiry: Models.ISubscriptionExpiry;

        activate: () => void;
        redeemCode: () => void;
    }

    class SubscriptionsController implements ISubscriptionsController {
        payments: Models.IPaymentRecord[] = [];
        subscriptions: Models.ISubscription[] = [];
        subscriptionExpiry: Models.ISubscriptionExpiry;

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
            let userId = this.userContext.current.user.id;
            this.paymentResource.query({ userId: userId }).$promise
                .then((payments) => { this.payments = payments; });

            this.subscriptionResource.query({ userId: userId }).$promise
                .then((subscriptions) => { this.subscriptions = subscriptions; });

            this.subscriptionResource.getExpiry({ userId: userId },
                (res: Models.ISubscriptionExpiry) => {
                    this.subscriptionExpiry = res;
                },
                (err) => {
                    console.error(err);
                });
        }

        redeemCode() {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/subscriptions/redeemCode/redeemCode.html',
                controller: 'redeemCodeController',
                controllerAs: 'ctrl',
                resolve: {
                    user: () => { return null; }
                }
            }).result.then(() => { }, (err) => { });
        }
    }

    angular.module("app").controller("subscriptionsController", SubscriptionsController);
}