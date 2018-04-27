
module App {
    "use strict";

    interface ISubscriptionsControllerScope extends ng.IScope {
        payments: Models.IPaymentRecord[];
        displayedPayments: Models.IPaymentRecord[];
        subscriptionPlans: Models.ISubscriptionPlan[];

        subscriptions: Models.ISubscription[];
        displayedSubscriptions: Models.ISubscription[];

        lastSubscription: Models.ISubscription[];

        searchTerm: string;
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
    }

    interface ISubscriptionsController {
        subscriptions: Models.ISubscription[];
        latestSubscription?: Date;
        isRestricted: boolean;

        activate: () => void;
        redeemCode: () => void;
    }

    class SubscriptionsController implements ISubscriptionsController {
        subscriptions: Models.ISubscription[] = [];
        latestSubscription?: Date;
        isRestricted: boolean;

        static $inject: string[] = ["$scope", "$uibModal", "paymentResource", "userContextService", "subscriptionResource", "subscriptionPlanResource"];
        constructor(
            private $scope: ISubscriptionsControllerScope,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private paymentResource: Resources.IPaymentResource,
            private userContext: Services.IUserContextService,
            private subscriptionResource: Resources.ISubscriptionResource,
            private subscriptionPlanResource: Resources.ISubscriptionPlanResource) {

            $scope.title = "Subscriptions";
            this.activate();
        }

        activate() {
            this.load();
            this.isRestricted = this.userContext.userIsRestricted();
        }

        load() {
            let userId = this.userContext.current.user.id;

            this.paymentResource.query().$promise
                .then((payments) => {
                    this.$scope.payments = payments;
                    this.$scope.displayedPayments = [].concat(this.$scope.payments);
                });

            this.subscriptionResource.getLatest(
                (res) => {
                    this.latestSubscription = res.date;
                });

            this.subscriptionPlanResource.query().$promise.then((plans) => {
                this.$scope.subscriptionPlans = plans;
            });

            //this.subscriptionResource.query().$promise.then((subscriptions) => {
            //    this.$scope.subscriptions = subscriptions;
            //    this.$scope.displayedSubscriptions = [].concat(this.$scope.subscriptions);
            //});

            this.subscriptionResource.getLastSubscription((res) => {
                //console.info(res);
                this.$scope.lastSubscription = res;
            }, (err) => {
                console.error(err);
            });
        }

        redeemCode() {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/myAccount/redeemCode/redeemCode.html',
                controller: 'redeemCodeController',
                controllerAs: 'ctrl'
            }).result.then(
                (res) => { location.reload(true); },
                (err) => { });
        }

        addSubscription() {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/myAccount/subscribe/subscribe.html',
                controller: 'subscribeController',
                controllerAs: 'ctrl',
                resolve: {
                    subscriptionPlans: () => {
                        return this.$scope.subscriptionPlans;
                    }
                }
            }).result.then(
                (res) => {
                    location.reload(true);
                },
                (err) => {
                    console.error(err);
                });
        }

    }

    angular.module("app").controller("subscriptionsController", SubscriptionsController);
}