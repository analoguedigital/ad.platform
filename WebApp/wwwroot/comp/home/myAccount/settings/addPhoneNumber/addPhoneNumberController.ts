module App {
    "use strict";

    interface IAddPhoneNumberModel {
        phoneNumber: string;
        code: string;
    }

    interface IAddPhoneNumberController {
        model: IAddPhoneNumberModel;
        codeSent: boolean;

        sendCode: (form: ng.IFormController) => void;
        verifyCode: (form: ng.IFormController) => void;
        activate: () => void;
        close: () => void;
    }

    class AddPhoneNumberController implements IAddPhoneNumberController {
        model: IAddPhoneNumberModel;
        codeSent: boolean = false;

        static $inject: string[] = ["$uibModalInstance", "$resource", "toastr", "number"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private $resource: ng.resource.IResourceService,
            public toastr: any,
            public number: string) {
            this.activate();
        }

        activate() {
            this.model = <IAddPhoneNumberModel>{};

            if (this.number && this.number.length)
                this.model.phoneNumber = this.number;
        }

        sendCode(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            var params = { phoneNumber: this.model.phoneNumber };
            this.$resource("/api/account/addPhoneNumber").save(params).$promise.then(
                (result) => {
                    this.codeSent = true;
                    this.toastr.info("We have sent you a security code via SMS");
                },
                (err) => {
                    console.error(err);

                    if (err.status === 400) {
                        this.toastr.error(err.data.message);
                    }
                    if (err.status === 500) {
                        this.toastr.error(err.data.exceptionMessage);
                    }
                });
        }

        verifyCode(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            var params = { phoneNumber: this.model.phoneNumber, code: this.model.code };
            this.$resource("/api/account/verifyPhoneNumber").save(params).$promise.then(
                (result) => {
                    this.toastr.info("Thank you for verifying your number");
                    this.$uibModalInstance.close(true);
                },
                (err) => {
                    if (err.status === 400) {
                        this.toastr.error(err.data.message);
                    }
                    if (err.status === 500)
                        this.toastr.error(err.data.exceptionMessage);
                });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("addPhoneNumberController", AddPhoneNumberController);
}