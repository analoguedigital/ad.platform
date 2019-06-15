module App {
    "use strict";

    interface IInactiveUsersControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        accountTypeFilter: string;

        users: Models.IOrgUser[];
        safeUsers: Models.IOrgUser[];
        displayedUsers: Models.IOrgUser[];

        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        clientsCount: number;
        staffCount: number;
        totalCount: number;

        currentUserIsSuperUser: boolean;
        selectedUser: Models.IOrgUser;

        delete: (id: string) => void;
        resetPassword: (user: Models.IUser) => void;
        revoke: (user) => void;
    }

    interface IInactiveUsersController {
        activate: () => void;
        delete: (id: string) => void;
        resetPassword: (user: Models.IUser) => void;
        revokeUser: (user) => void;
    }

    class InactiveUsersController implements IInactiveUsersController {
        static $inject: string[] = ["$scope", "$uibModal", "toastr", "orgUserResource", "organisationResource",
            "userContextService", "$stateParams", "localStorageService"];

        constructor(
            private $scope: IInactiveUsersControllerScope,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private toastr: any,
            private orgUserResource: Resources.IOrgUserResource,
            private organisationResource: Resources.IOrganisationResource,
            private userContextService: Services.IUserContextService,
            private $stateParams: ng.ui.IStateParamsService,
            private localStorageService: ng.local.storage.ILocalStorageService) {

            $scope.title = "Inactive Accounts";
            $scope.delete = (id) => { this.delete(id); };
            $scope.resetPassword = (user) => { this.resetPassword(user); }
            $scope.revoke = (user) => { this.revokeUser(user); }

            this.activate();
        }

        activate() {
            var self = this;

            this.$scope.customDialogButtons = {
                deleteUser: {
                    label: "Delete user",
                    className: "btn-primary",
                    callback: function (args) {
                        if (self.$scope.selectedUser) {
                            self.delete(self.$scope.selectedUser.id);
                        }
                    }
                },
                deleteAccount: {
                    label: "Delete account",
                    className: "btn-danger",
                    callback: function () {
                        if (self.$scope.selectedUser) {
                            self.deleteAccount(self.$scope.selectedUser.id);
                        }
                    }
                },
                cancel: {
                    label: "Cancel",
                    className: "btn-default",
                    callback: function () {
                        // do nothing
                    }
                }
            };

            this.load();
        }

        selectedUserChanged(user: Models.IOrgUser) {
            this.$scope.selectedUser = user;
        }

        load() {
            var accountTypeFilter = <string>this.localStorageService.get('inactive_accounts_type_filter');
            if (accountTypeFilter == null || !accountTypeFilter.length) {
                accountTypeFilter = 'clients';
            }

            var roles = ["System administrator", "Platform administrator"];
            this.$scope.currentUserIsSuperUser = this.userContextService.userIsInAnyRoles(roles);

            if (this.userContextService.current.orgUser !== null) {
                var orgId = this.userContextService.current.orgUser.organisation.id;
                this.orgUserResource.query({ listType: 3, organisationId: orgId }).$promise.then((users) => {
                    this.$scope.users = users;
                    this.$scope.safeUsers = users;
                    this.$scope.displayedUsers = [].concat(this.$scope.users);

                    this.countUserTypes();
                    this.filterAccounts(accountTypeFilter);
                });
            } else {
                this.orgUserResource.query({ listType: 3, organisationId: null }).$promise.then((users) => {
                    this.$scope.users = users;
                    this.$scope.safeUsers = users;
                    this.$scope.displayedUsers = [].concat(this.$scope.users);

                    this.countUserTypes();
                    this.filterAccounts(accountTypeFilter);
                });
            }
        }
        
        resetPassword(user: Models.IUser) {
            var selectedUser = user;

            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/common/resetPassword/resetPasswordView.html',
                controller: 'resetPasswordController',
                controllerAs: 'ctrl',
                resolve: {
                    user: () => {
                        return selectedUser;
                    }
                }
            });
        }

        delete(id: string) {
            this.orgUserResource.delete({ id: id },
                () => {
                    this.toastr.success('User deleted successfully');
                    this.load();
                },
                (err) => {
                    this.toastr.error(err.data.message);
                });
        }

        deleteAccount(id: string) {
            this.orgUserResource.deleteAccount({ id: id },
                () => {
                    this.toastr.success('Account deleted successfully');
                    this.load();
                }, (err) => {
                    this.toastr.error(err.data.message);
                });
        }

        revokeUser(user: Models.IOrgUser) {
            var params = { id: user.organisation.id, userId: user.id };
            this.organisationResource.revoke(params, (res) => {
                this.toastr.success("User removed from organisation");
                this.toastr.info("Cases and threads moved to OnRecord");

                this.load();
            });
        }

        composeEmail(user: Models.IOrgUser) {
            var modalInstance = this.$uibModal.open({
                animation: true,
                size: 'lg',
                templateUrl: 'comp/home/users/composeEmail/composeEmailView.html',
                controller: 'composeEmailController',
                controllerAs: 'ctrl',
                resolve: {
                    emailAddress: () => {
                        return user.email;
                    }
                }
            });
        }

        countUserTypes() {
            this.$scope.clientsCount = _.filter(this.$scope.safeUsers, (u) => { return u.accountType === 0; }).length;
            this.$scope.staffCount = _.filter(this.$scope.safeUsers, (u) => { return u.accountType === 1; }).length;
            this.$scope.totalCount = this.$scope.clientsCount + this.$scope.staffCount;
        }
        
        filterAccounts(type: string) {
            this.$scope.accountTypeFilter = type;
            this.localStorageService.set('inactive_accounts_type_filter', type);

            if (type === 'clients') {
                this.$scope.users = _.filter(this.$scope.safeUsers, (u) => { return u.accountType === 0; });
            } else if (type === 'staff') {
                this.$scope.users = _.filter(this.$scope.safeUsers, (u) => { return u.accountType === 1; });
            } else if (type === 'all') {
                this.$scope.users = this.$scope.safeUsers;
            }

            this.$scope.displayedUsers = [].concat(this.$scope.users);
        }
    }

    angular.module("app").controller("inactiveUsersController", InactiveUsersController);
}