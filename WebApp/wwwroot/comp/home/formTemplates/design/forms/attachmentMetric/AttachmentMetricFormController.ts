
module App {
    "use strict";

    interface IAttachmentMetricFormControllerScope extends ng.IScope {
        metric: Models.IAttachmentMetric;
        attachmentTypes: string[];
        toggleAttachmentType: (attachmentType: string) => void;
        close: () => void;
    }

    interface IAttachmentMetricFormController {
        activate: () => void;
    }

    class AttachmentMetricFormController implements IAttachmentMetricFormController {
        static $inject: string[] = ["$scope", "$uibModalInstance", "metric", "Upload"];

        constructor(private $scope: IAttachmentMetricFormControllerScope,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private metric: Models.IAttachmentMetric,
            private Upload: any) {

            $scope.attachmentTypes = ['Document', 'Image', 'Video'];
            $scope.metric = metric;
            $scope.toggleAttachmentType = (attachmentType) => { this.toggleAttachmentType(attachmentType); };
            $scope.close = () => { this.close(); };
            this.activate();
        }

        activate() {

        }

        toggleAttachmentType(attachmentType: string) {
            var idx = this.$scope.metric.allowedAttachmentTypes.indexOf(attachmentType);

            // is currently selected
            if (idx > -1) {
                this.$scope.metric.allowedAttachmentTypes.splice(idx, 1);
            }

            // is newly selected
            else {
                this.$scope.metric.allowedAttachmentTypes.push(attachmentType);
            }
        };

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("attachmentMetricFormController", AttachmentMetricFormController);
}