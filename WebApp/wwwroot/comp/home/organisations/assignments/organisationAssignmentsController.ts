
module App {
    "use strict";

    interface IOrganisationAssignmentsControllerScope extends ng.IScope {
        title: string;
        organisation: Models.IOrganisation;
        users: Models.IOrgUser[];
        displayedUsers: Models.IOrgUser[];
        searchTerm: string;
    }

    interface IOrganisationAssignmentsController {
        activate: () => void;
    }

    class OrganisationAssignmentsController implements IOrganisationAssignmentsController {
        static $inject: string[] = ["$scope", "$state", "$stateParams", "$q", "organisationResource", "orgUserResource", "toastr"];

        constructor(
            private $scope: IOrganisationAssignmentsControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $q: ng.IQService,
            private organisationResource: Resources.IOrganisationResource,
            private orgUserResource: Resources.IOrgUserResource,
            private toastr: any
        ) {
            $scope.title = "Organisations";
            $scope.submit = () => { this.submit(); };

            this.activate();
        }

        activate() {
            var organisationId = this.$stateParams['id'];

            var organisationPromise = this.organisationResource.get({ id: organisationId }).$promise;
            organisationPromise.then((organisation) => { this.$scope.organisation = organisation; });

            // refactor this code! we need a better way of getting OnRecord users.
            var usersPromise = this.orgUserResource.query({ listType: 0, organisationId: 'CFA81EB0-9FC7-4932-A3E8-1C822370D034' }).$promise;
            usersPromise.then((users) => {
                this.$scope.users = _.filter(users, (user) => { return !user.isRootUser; });
                this.$scope.displayedUsers = [].concat(this.$scope.users);
            });

            this.$q.all([organisationPromise, usersPromise]).then(() => {
                
            });
        }

        submit() {
            var selected = _.filter(this.$scope.users, (user) => { return user.isSelected; });
            var ids = _.map(selected, (item) => { return item.id; });

            var payload = {
                id: this.$scope.organisation.id,
                orgUsers: ids
            };

            this.organisationResource.assign(payload, (res) => {
                this.toastr.success("Selected users assigned to organisation");
                this.toastr.info("Cases and threads moved to organisation");
            });
        }
    }

    angular.module("app").controller("organisationAssignmentsController", OrganisationAssignmentsController);
}