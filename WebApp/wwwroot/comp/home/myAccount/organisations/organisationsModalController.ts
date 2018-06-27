module App {
    "use strict";

    interface IOrganisationsModalController {
        activate: () => void;
        close: () => void;
    }

    class OrganisationsModalController implements IOrganisationsModalController {
        token: string;

        static $inject: string[] = ["$uibModalInstance", "$uibModal", "toastr"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private $uibModal: ng.ui.bootstrap.IModalService,
            public toastr: any) {
            this.activate();
        }

        activate() {
            
        }

        joinOrganisation() {
            this.close();

            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/myAccount/joinOrganisation/joinOrganisationView.html',
                controller: 'joinOrganisationController',
                controllerAs: 'ctrl'
            }).result.then(
                (res) => {
                    location.reload(true);
                },
                (err) => {
                    console.error(err);
                });
        }

        connectToOrganisation() {
            this.close();

            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/myAccount/connectToOrganisation/connectToOrganisationModal.html',
                controller: 'connectToOrganisationModalController',
                controllerAs: 'ctrl'
            }).result.then(
                (res) => {
                    location.reload(true);
                },
                (err) => {
                    console.error(err);
                });
        }

        requestOrganisation() {
            this.close();

            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/myAccount/requestOrganisation/requestOrganisationModal.html',
                controller: 'requestOrganisationModalController',
                controllerAs: 'ctrl'
            }).result.then(
                (res) => {
                    location.reload(true);
                },
                (err) => {
                    console.error(err);
                });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("organisationsModalController", OrganisationsModalController);
}