module App {
    "use strict";

    interface IJoinOrganisationController {
        token: string;

        activate: () => void;
        close: () => void;
        processCode: (form: ng.IFormController) => void;
    }

    class JoinOrganisationController implements IJoinOrganisationController {
        token: string;

        static $inject: string[] = ["$uibModalInstance", "orgInvitationResource", "toastr"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private orgInvitationResource: Resources.IOrgInvitationResource,
            public toastr: any) {
            this.activate();
        }

        activate() {
            this.token = '';
        }

        processCode(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            this.orgInvitationResource.join({ token: this.token },
                (res) => {
                    this.toastr.success('You have joined this organisation.');
                    this.$uibModalInstance.close(res);

                    location.reload(true);
                },
                (err) => {
                    if (err.data.message)
                        this.toastr.error(err.data.message);

                    if (err.status == 404) {
                        this.toastr.error('Invitation token is not valid!');
                    }
                });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("joinOrganisationController", JoinOrganisationController);
}