module App {
    "use strict";

    interface IComposeEmailController {
        activate: () => void;
        close: () => void;
    }

    interface IComposeEmailModel {
        email: string;
        body: string;
    }

    class ComposeEmailController implements IComposeEmailController {
        model: IComposeEmailModel;

        static $inject: string[] = ["$uibModalInstance", "$resource", "userContextService", "userResource", "toastr", "emailAddress"];
        constructor(private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private $resource: ng.resource.IResourceService,
            private userContext: Services.IUserContextService,
            private userResource: Resources.IUserResource,
            private toastr: any,
            private emailAddress: string) {

            this.activate();
        }

        activate() {
            this.model = <IComposeEmailModel>{};

            if (this.emailAddress)
                this.model.email = this.emailAddress;
        }

        sendEmail() {
            var params = {
                emailAddress: this.model.email,
                body: this.model.body
            };

            this.$resource('/api/feedbacks/sendEmail').save(params).$promise.then((res) => {
                this.toastr.success('Email message queued');
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

    angular.module("app").controller("composeEmailController", ComposeEmailController);
}