
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

        delete: (id: string) => void;
        resetPassword: (user: Models.IUser) => void;
    }

    interface IUsersController {
        activate: () => void;
        delete: (id: string) => void;
        resetPassword: (user: Models.IUser) => void;
    }

    class UsersController implements IUsersController {
        static $inject: string[] = ["$scope", "$uibModal", "toastr", "orgUserResource"];

        constructor(
            private $scope: IUsersControllerScope,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private toastr: any,
            private userResource: Resources.IOrgUserResource) {

            $scope.title = "Users";
            $scope.delete = (id) => { this.delete(id); };
            $scope.resetPassword = (user) => { this.resetPassword(user); }

            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            this.userResource.query().$promise.then((users) => {
                this.$scope.users = users;
                this.$scope.displayedUsers = [].concat(this.$scope.users);
            });
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
            this.userResource.delete({ id: id },
                () => {
                    this.load();
                },
                (err) => {
                    this.toastr.error(err.data.message);
                });
        }
    }

    angular.module("app").controller("usersController", UsersController);
}