
module App {
    "use strict";

    interface SubscriptionLengthModel {
        value: number;
        text: string;
    }

    interface ISubscriptionPlanEditControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        isInsertMode: boolean;
        errors: string;
        model: Models.ISubscriptionPlan;
        subscriptionLengths: SubscriptionLengthModel[];

        submit: (form: ng.IFormController) => void;
        clearErrors: () => void;
    }

    interface ISubscriptionPlanEditController {
        activate: () => void;
    }

    class SubscriptionPlanEditController implements ISubscriptionPlanEditController {
        static $inject: string[] = ["$scope", "subscriptionPlanResource", "$state", "$stateParams", "toastr"];

        constructor(
            private $scope: ISubscriptionPlanEditControllerScope,
            private subscriptionPlanResource: Resources.ISubscriptionPlanResource,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private toastr: any
        ) {

            $scope.title = "Subscription Plans";

            $scope.submit = (form: ng.IFormController) => { this.submit(form); }
            $scope.clearErrors = () => { this.clearErrors(); }

            $scope.subscriptionLengths = [];
            for (var i = 1; i <= 12; i++) {
                var itemText = i === 1 ? 'month' : 'months';
                this.$scope.subscriptionLengths.push({
                    value: i,
                    text: `${i} ${itemText}`
                });
            }

            this.activate();
        }

        activate() {
            var planId = this.$stateParams['id'];
            if (planId !== '') {
                this.subscriptionPlanResource.get({ id: planId }).$promise.then((plan) => {
                    this.$scope.model = plan;
                });
            } else {
                this.$scope.model = <Models.ISubscriptionPlan>{};
            }
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            console.info(this.$scope.model);

            var planId = this.$stateParams['id'];
            if (planId === '') {
                this.subscriptionPlanResource.save(
                    this.$scope.model,
                    () => { this.$state.go('home.subscriptionplans.list'); },
                    (err) => {
                        console.log(err);
                        this.$scope.errors = err.data.exceptionMessage;
                    });
            }
            else {
                if (!this.$scope.model.isLimited)
                    this.$scope.model.monthlyQuota = null;

                this.subscriptionPlanResource.update(
                    this.$scope.model,
                    () => { this.$state.go('home.subscriptionplans.list'); },
                    (err) => {
                        console.log(err);
                        this.$scope.errors = err.data.exceptionMessage;
                    });
            }
        }

        clearErrors() {
            this.$scope.errors = undefined;
        }

    }

    angular.module("app").controller("subscriptionPlanEditController", SubscriptionPlanEditController);
}