module App {
    "use strict";

    interface IRequestOrganisationModel {
        name: string;
        address: string;
        contactNumber: string;
        email: string;
        telNumber: string;
        postcode: string;
    }

    interface IRequestOrganisationModalController {
        model: IRequestOrganisationModel;

        activate: () => void;
        close: () => void;
        sendRequest: (form: ng.IFormController) => void;
    }

    class RequestOrganisationModalController implements IRequestOrganisationModalController {
        model: IRequestOrganisationModel;

        static $inject: string[] = ["$uibModalInstance", "orgRequestResource", "toastr"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private orgRequestResource: Resources.IOrgRequestResource,
            public toastr: any) {
            this.activate();
        }

        activate() {
            this.model = {
                name: '',
                address: '',
                contactNumber: '',
                email: '',
                telNumber: '',
                postcode: ''
            };
        }

        sendRequest(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            this.orgRequestResource.sendRequest(this.model,
                (res) => {
                    this.toastr.success('Request sent. Many thanks.');
                    this.close();
                },
                (err) => {
                    if (err.data.message)
                        this.toastr.error(err.data.message);
                });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("requestOrganisationModalController", RequestOrganisationModalController);
}