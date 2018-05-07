
module App {
    "use strict";

    interface IUsersControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        users: Models.IOrgUser[];
        superUsers: Models.IUser[];
        displayedUsers: Models.IOrgUser[];
        displayedSuperUsers: Models.IUser[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
        currentUserIsSuperUser: boolean;

        delete: (id: string) => void;
        resetPassword: (user: Models.IUser) => void;
        addSuperUser: () => void;
        revoke: (user) => void;
    }

    interface IUsersController {
        activate: () => void;
        delete: (id: string) => void;
        resetPassword: (user: Models.IUser) => void;
        addSuperUser: () => void;
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
            $scope.addSuperUser = () => { this.addSuperUser(); }
            $scope.revoke = (user) => { this.revokeUser(user); }

            this.activate();
        }

        activate() {
            this.load();
        }
        
        load() {
            var roles = ["System administrator", "Platform administrator"];
            this.$scope.currentUserIsSuperUser = this.userContextService.userIsInAnyRoles(roles);

            if (this.$scope.currentUserIsSuperUser) {
                this.userResource.query().$promise.then((users) => {
                    this.$scope.superUsers = users;
                    this.$scope.displayedSuperUsers = [].concat(this.$scope.superUsers);
                });
            }

            var orgId = this.$stateParams["organisationId"];
            if (orgId !== undefined && orgId.length) {
                this.orgUserResource.query({ organisationId: orgId }).$promise.then((users) => {
                    var webAccounts = _.filter(users, (u) => {
                        return u.accountType === 1;
                    });

                    this.$scope.users = webAccounts;
                    this.$scope.displayedUsers = [].concat(this.$scope.users);
                });
            } else {
                this.orgUserResource.query().$promise.then((users) => {
                    var webAccounts = _.filter(users, (u) => {
                        return u.accountType === 1;
                    });

                    this.$scope.users = webAccounts;
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

        addSuperUser() {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/users/addSuperUser/addSuperUserView.html',
                controller: 'addSuperUserController',
                controllerAs: 'ctrl'
            });

            modalInstance.result.then((res) => {
                console.log(res);
            }, (err) => {
                console.warn(err);
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
    }

    angular.module("app").controller("usersController", UsersController);
}