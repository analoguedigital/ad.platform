module App {
    "use strict";

    interface IConnectToOrganisationModalController {
        organisations: Models.IOrganisation[];

        activate: () => void;
        close: () => void;
    }

    class ConnectToOrganisationModalController implements IConnectToOrganisationModalController {
        organisations: Models.IOrganisation[];

        static $inject: string[] = ["$uibModalInstance", "$uibModal", "organisationResource", "orgConnectionRequestResource", "toastr"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private organisationResource: Resources.IOrganisationResource,
            private orgConnectionRequestResource: Resources.IOrgConnectionRequestResource,
            public toastr: any) {
            this.activate();
        }

        activate() {
            this.organisationResource.getList().$promise.then((data) => {
                this.organisations = data;
            }, (err) => {
                console.error(err);
            });
        }
        
        requestToJoin(id: string) {
            this.orgConnectionRequestResource.requestToJoin({ organisationId: id }, (res) => {
                this.toastr.success('Connection request sent. Thank you.');
                this.close();
            }, (err) => {
                console.error(err);
                if (err.data.message)
                    this.toastr.error(err.data.message);
            });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("connectToOrganisationModalController", ConnectToOrganisationModalController);
}