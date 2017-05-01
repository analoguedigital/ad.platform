
module App {
    "use strict";

    interface IOrganisationEditControllerScope extends ng.IScope {
        title: string;
        organisation: Models.IOrganisation;
        isInsertMode: boolean;
        errors: string;
        submit: (form: ng.IFormController) => void;
        clearErrors: () => void;
    }

    interface IOrganisationEditController {
        activate: () => void;
    }

    class OrganisationEditController implements IOrganisationEditController {
        static $inject: string[] = ["$scope", "organisationResource", "$state", "$stateParams"];

        constructor(
            private $scope: IOrganisationEditControllerScope,
            private organisationResource: Resources.IOrganisationResource,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService
        ) {

            $scope.title = "Organisations";

            $scope.submit = (form: ng.IFormController) => { this.submit(form); }
            $scope.clearErrors = () => { this.clearErrors(); }

            this.activate();
        }

        activate() {
            var organisationId = this.$stateParams['id'];
            if (organisationId === '') {
                organisationId = '00000000-0000-0000-0000-000000000000';
                this.$scope.isInsertMode = true;
            }
            this.organisationResource.get({ id: organisationId }).$promise.then((organisation) => {
                this.$scope.organisation = organisation;
            });
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }
            var organisationId = this.$stateParams['id'];
            if (organisationId === '') {
                this.organisationResource.save(
                    this.$scope.organisation,
                    () => { this.$state.go('home.organisations.list'); },
                    (err) => {
                        console.log(err);
                        this.$scope.errors = err.data.exceptionMessage;
                    });
            }
            else {
                this.organisationResource.update(
                    this.$scope.organisation,
                    () => { this.$state.go('home.organisations.list'); },
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

    angular.module("app").controller("organisationEditController", OrganisationEditController);
}