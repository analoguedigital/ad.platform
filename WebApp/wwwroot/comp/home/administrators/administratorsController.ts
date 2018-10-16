module App {
    "use strict";

    interface IAdministratorsControllerScope extends ng.IScope {
        title: string;
    }

    interface IAdministratorsController {
        activate: () => void;

        superUsers: Models.IUser[]
        displayedSuperUsers: Models.IUser[];

        platformUsers: Models.IUser[];
        displayedPlatformUsers: Models.IUser[];
    }

    class AdministratorsController implements IAdministratorsController {
        superUsers: Models.IUser[] = [];;
        displayedSuperUsers: Models.IUser[] = [];
        platformUsers: Models.IUser[] = [];
        displayedPlatformUsers: Models.IUser[] = [];

        static $inject: string[] = ["$scope", "$stateParams", "$uibModal", "toastr", "userResource", "platformUserResource"];
        constructor(
            private $scope: IAdministratorsControllerScope,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private toastr: any,
            private userResource: Resources.IUserResource,
            private platformUserResource: Resources.IPlatformUserResource) {

            this.activate();
        }

        activate() {
            this.$scope.title = "Administrators";
            this.load();
        }

        loadSuperUsers() {
            this.userResource.query().$promise.then((users) => {
                this.superUsers = users;
                this.displayedSuperUsers = [].concat(users);
            });
        }

        loadPlatformUsers() {
            this.platformUserResource.query().$promise.then((platformAdmins) => {
                this.platformUsers = platformAdmins;
                this.displayedPlatformUsers = [].concat(platformAdmins);
            });
        }

        load() {
            this.loadSuperUsers();
            this.loadPlatformUsers();
        }

        addSuperUser() {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/administrators/addSuperUser/addSuperUserView.html',
                controller: 'addSuperUserController',
                controllerAs: 'ctrl'
            });

            modalInstance.result.then((res) => {
                this.loadSuperUsers();
            }, (err) => {
                console.warn(err);
            });
        }

        addPlatformUser() {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/administrators/addPlatformUser/addPlatformUserView.html',
                controller: 'addPlatformUserController',
                controllerAs: 'ctrl'
            });

            modalInstance.result.then((res) => {
                console.log(res);

                this.loadPlatformUsers();
            }, (err) => {
                console.warn(err);
            });
        }

        deleteSuperUser(id: string) {
            this.userResource.delete({ id: id },
                () => {
                    this.loadSuperUsers();
                },
                (err) => {
                    this.toastr.error(err.data.message);
                });
        }

        deletePlatformUser(id: string) {
            this.platformUserResource.delete({ id: id },
                () => {
                    this.loadPlatformUsers();
                },
                (err) => {
                    this.toastr.error(err.data.message);
                });
        }

    }

    angular.module("app").controller("administratorsController", AdministratorsController);
}