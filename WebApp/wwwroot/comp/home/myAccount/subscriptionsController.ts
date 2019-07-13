
module App {
    "use strict";

    interface ISubscriptionsControllerScope extends ng.IScope {
        payments: Models.IPaymentRecord[];
        displayedPayments: Models.IPaymentRecord[];
        subscriptionPlans: Models.ISubscriptionPlan[];

        subscriptions: Models.ISubscriptionEntry[];
        displayedSubscriptions: Models.ISubscriptionEntry[];
        lastSubscription: Models.ISubscription;
        quota: Models.IMonthlyQuota;

        searchTerm: string;
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
    }

    interface ISubscriptionsController {
        latestSubscription?: Date;
        isRestricted: boolean;
        isAdmin: boolean;
        accountType: number;
        canUnlinkFromOrganization: boolean;

        activate: () => void;
        redeemCode: () => void;
    }

    class SubscriptionsController implements ISubscriptionsController {
        latestSubscription?: Date;
        isRestricted: boolean;
        isAdmin: boolean;
        accountType: number;
        canUnlinkFromOrganization: boolean;

        static $inject: string[] = ["$scope", "$uibModal", "paymentResource", "userContextService",
            "subscriptionResource", "subscriptionPlanResource", "userContextService", "organisationResource"];
        constructor(
            private $scope: ISubscriptionsControllerScope,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private paymentResource: Resources.IPaymentResource,
            private userContext: Services.IUserContextService,
            private subscriptionResource: Resources.ISubscriptionResource,
            private subscriptionPlanResource: Resources.ISubscriptionPlanResource,
            private userContextService: Services.IUserContextService,
            private organisationResource: Resources.IOrganisationResource) {

            $scope.title = "Subscriptions";
            this.activate();
        }

        activate() {
            var roles = ["System administrator", "Platform administrator", "Organisation administrator"];
            this.isAdmin = this.userContextService.userIsInAnyRoles(roles);
            this.isRestricted = this.userContext.userIsRestricted();

            if (this.userContext.current.orgUser !== null)
                this.accountType = this.userContext.current.orgUser.accountType;

            this.load();
        }

        load() {
            if (!this.isAdmin && this.accountType === 0) {
                let userId = this.userContext.current.user.id;

                this.subscriptionResource.getLatest(
                    (res) => {
                        this.latestSubscription = res.date;
                    });

                //this.subscriptionPlanResource.query().$promise.then((plans) => {
                //    this.$scope.subscriptionPlans = plans;
                //});

                this.subscriptionResource.getLastSubscription((res) => {
                    this.$scope.lastSubscription = res;

                    // users cannot unlink from OnRecord
                    if (res.type == 1 && res.organisationId !== 'cfa81eb0-9fc7-4932-a3e8-1c822370d034') {
                        this.canUnlinkFromOrganization = true;
                    }
                }, (err) => {
                    console.error(err);
                });

                this.subscriptionResource.getQuota((res) => {
                    this.$scope.quota = res;
                }, (err) => {
                    console.error(err);
                });

                this.subscriptionResource.query().$promise.then((subscriptions: any) => {
                    this.$scope.subscriptions = subscriptions;
                    this.$scope.displayedSubscriptions = subscriptions;
                });
            }
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

        openOrganisationsModal() {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/myAccount/organisations/organisationsModal.html',
                controller: 'organisationsModalController',
                controllerAs: 'ctrl'
            }).result.then(
                (res) => { },
                (err) => { });
        }

        unlinkFromOrganisation() {
            var orgUser = this.userContextService.current.orgUser;
            var payload = {
                id: orgUser.organisation.id,
                userId: orgUser.id
            };

            this.organisationResource.revoke(payload, (result) => {
                location.reload();
            }, (err) => {
                console.error(err);
            });
        }
    }

    angular.module("app").controller("subscriptionsController", SubscriptionsController);
}