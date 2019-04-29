
module App {
    "use strict";

    interface IOrganisationEditControllerScope extends ng.IScope {
        title: string;
        organisation: Models.IOrganisation;

        safeUsers: Models.IOrgUser[];
        users: Models.IOrgUser[];
        displayedUsers: Models.IOrgUser[];

        accountTypeFilter: string;

        searchTerm: string;
        isInsertMode: boolean;
        errors: string;

        submit: (form: ng.IFormController) => void;
        clearErrors: () => void;
    }

    interface IOrganisationEditController {
        activate: () => void;
    }

    class OrganisationEditController implements IOrganisationEditController {
        static $inject: string[] = ["$scope", "organisationResource", "orgUserResource", "$state", "$stateParams", "toastr"];
        
        constructor(
            private $scope: IOrganisationEditControllerScope,
            private organisationResource: Resources.IOrganisationResource,
            private orgUserResource: Resources.IOrgUserResource,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private toastr: any
        ) {

            $scope.title = "Organisations";
            $scope.accountTypeFilter = 'all';

            $scope.submit = (form: ng.IFormController) => { this.submit(form); }
            $scope.clearErrors = () => { this.clearErrors(); }
            $scope.remove = (id) => { this.remove(id); };

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

                this.orgUserResource.query({ listType: 2, organisationId: organisationId }).$promise.then((users) => {
                    this.$scope.safeUsers = users;
                    this.$scope.users = users;
                    this.$scope.displayedUsers = [].concat(users);

                    this.filterUsers(this.$scope.accountTypeFilter);
                });
            });
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                this.toastr.warning('Fix your validation errors first');
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
                if (!this.$scope.organisation.subscriptionEnabled)
                    this.$scope.organisation.subscriptionMonthlyRate = null;

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

        remove(user: Models.IOrgUser) {
            var params = { id: this.$scope.organisation.id, userId: user.id };
            this.organisationResource.revoke(params, (res) => {
                var item = _.filter(this.$scope.users, (u) => { return u.id == user.id })[0];
                var index = this.$scope.users.indexOf(item);
                this.$scope.users.splice(index, 1);

                this.toastr.success("User removed from organisation");
                this.toastr.info("Cases and threads moved to OnRecord");
            }, (err) => {
                if (err.data.message && err.data.message.length)
                    this.toastr.error(err.data.message);
            });
        }

        filterUsers(type: string) {
            this.$scope.accountTypeFilter = type;

            if (type === 'clients') {
                this.$scope.users = _.filter(this.$scope.safeUsers, (u) => { return u.accountType === 0; });
            } else if (type === 'staff') {
                this.$scope.users = _.filter(this.$scope.safeUsers, (u) => { return u.accountType === 1; });
            } else if (type === 'all') {
                this.$scope.users = this.$scope.safeUsers;
            }

            this.$scope.displayedProjects = [].concat(this.$scope.projects);
        }
    }

    angular.module("app").controller("organisationEditController", OrganisationEditController);
}