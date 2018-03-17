module App {
    "use strict";

    interface IChangePhoneNumberModel {
        phoneNumber: string;
        code: string;
    }

    interface IChangePhoneNumberController {
        model: IChangePhoneNumberModel;
        codeSent: boolean;

        sendCode: (form: ng.IFormController) => void;
        verifyCode: (form: ng.IFormController) => void;
        activate: () => void;
        close: () => void;
    }

    class ChangePhoneNumberController implements IChangePhoneNumberController {
        model: IChangePhoneNumberModel;
        codeSent: boolean = false;

        static $inject: string[] = ["$uibModalInstance", "$resource", "toastr", "phoneNumber"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private $resource: ng.resource.IResourceService,
            public toastr: any,
            public phoneNumber: string) {

            this.activate();
        }

        activate() {
            this.model = <IChangePhoneNumberModel>{};
            if (this.phoneNumber) this.model.phoneNumber = this.phoneNumber;
        }

        sendCode(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            var params = { phoneNumber: this.model.phoneNumber };
            this.$resource("/api/account/changePhoneNumber").save(params).$promise.then(
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
            this.$resource("/api/account/verifyChangedNumber").save(params).$promise.then(
                (result) => {
                    this.toastr.info("Thank you for verifying your number");
                    this.$uibModalInstance.close(true);
                },
                (err) => {
                    this.toastr.error('Action failed. Check for errors.');

                    console.error(err);

                    if (err.status === 400) {
                        this.toastr.error(err.data.message);
                    }
                    if (err.status === 500) {
                        this.toastr.error(err.data.exceptionMessage);
                    }
                });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("changePhoneNumberController", ChangePhoneNumberController);
}