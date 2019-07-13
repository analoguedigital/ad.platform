module App {
    "use strict";

    interface IAdviceReferencesController {
        activate: () => void;
        close: () => void;
    }

    class AdviceReferencesController implements IAdviceReferencesController {
        static $inject: string[] = ["$uibModalInstance", "toastr", "survey", "adviceRecords"];
        constructor(
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            public toastr: any,
            public survey: Models.ISurvey,
            public adviceRecords: Models.ISurvey[]) {
            this.activate();
        }

        activate() {
            
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("adviceReferencesController", AdviceReferencesController);
}