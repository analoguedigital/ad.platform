
module App {
    "use strict";

    interface ISubscriptionPlansControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        organisations: Models.IOrganisation[];
        displayedOrganisations: Models.IOrganisation[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        plans: Models.ISubscriptionPlan[];
        displayedPlans: Models.ISubscriptionPlan[];

        delete: (id: string) => void;
    }

    interface ISubscriptionPlansController {
        activate: () => void;
        delete: (id: string) => void;
    }

    class SubscriptionPlansController implements ISubscriptionPlansController {
        static $inject: string[] = ["$scope", "organisationResource", "subscriptionPlanResource", "toastr"];

        constructor(
            private $scope: ISubscriptionPlansControllerScope,
            private organisationResource: Resources.IOrganisationResource,
            private subscriptionPlanResource: Resources.ISubscriptionPlanResource,
            private toastr: any) {

            $scope.title = "Subscription Plans";
            $scope.delete = (id) => { this.delete(id); };

            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            this.subscriptionPlanResource.query().$promise.then((plans) => {
                this.$scope.plans = plans;
                this.$scope.displayedPlans = [].concat(this.$scope.plans);
            });
        }

        delete(id: string) {
            this.subscriptionPlanResource.delete({ id: id },
                () => {
                    this.load();
                },
                (err) => {
                    console.error(err);
                    this.toastr.error(err.data.message);
                });
        }
    }

    angular.module("app").controller("subscriptionPlansController", SubscriptionPlansController);
}