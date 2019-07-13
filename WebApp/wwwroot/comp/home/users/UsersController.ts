
module App {
    "use strict";

    interface IUsersControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        users: Models.IOrgUser[];
        displayedUsers: Models.IOrgUser[];

        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        selectedUser: Models.IOrgUser;

        delete: (id: string) => void;
        resetPassword: (user: Models.IUser) => void;
        revoke: (user) => void;
    }

    interface IUsersController {
        activate: () => void;
        delete: (id: string) => void;
        resetPassword: (user: Models.IUser) => void;
        revokeUser: (user) => void;
    }

    class UsersController implements IUsersController {
        static $inject: string[] = ["$scope", "$uibModal", "toastr", "$stateParams", 
            "userResource", "orgUserResource", "organisationResource", "userContextService"];

        constructor(
            private $scope: IUsersControllerScope,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private toastr: any,
            private $stateParams: ng.ui.IStateParamsService,
            private userResource: Resources.IOrgUserResource,
            private orgUserResource: Resources.IOrgUserResource,
            private organisationResource: Resources.IOrganisationResource,
            private userContextService: Services.IUserContextService) {

            $scope.title = "Users";
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

                    }
                }
            };

            this.load();
        }

        selectedUserChanged(user: Models.IOrgUser) {
            this.$scope.selectedUser = user;
        }

        load() {
            var orgId = this.$stateParams["organisationId"];
            if (orgId !== undefined && orgId.length) {
                this.orgUserResource.query({ listType: 1, organisationId: orgId }).$promise.then((users) => {
                    this.$scope.users = users;
                    this.$scope.displayedUsers = [].concat(this.$scope.users);
                });
            } else {
                //var currentOrgId = this.userContextService.current.orgUser.organisationId;
                this.orgUserResource.query({ listType: 1, organisationId: null }).$promise.then((users) => {
                    this.$scope.users = users;
                    this.$scope.displayedUsers = [].concat(this.$scope.users);
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
                var item = _.filter(this.$scope.users, (u) => { return u.id == user.id })[0];
                var onRecordId = 'cfa81eb0-9fc7-4932-a3e8-1c822370d034';

                this.organisationResource.get({ id: onRecordId }).$promise.then((org) => {
                    item.organisation = org;
                });

                this.toastr.success("User removed from organisation");
                this.toastr.info("Cases and threads moved to OnRecord");
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

        updateStatus(userId: string, isAuthorized: boolean) {
            var payload = {
                userId: userId,
                isAuthorizedStaff: isAuthorized
            };

            this.orgUserResource.updateStatus(payload, (result) => {
                // user status updated.
            }, (err) => {
                console.error(err);
            });
        }
    }

    angular.module("app").controller("usersController", UsersController);
}