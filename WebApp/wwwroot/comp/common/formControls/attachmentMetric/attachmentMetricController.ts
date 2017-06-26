// https://jsfiddle.net/58oL5g4c/10/
module App {
    "use strict";

    interface IAttachmentMetricController {
        activate: () => void;
    }

    interface IAttachmentMetricControllerScope extends IMetricControllerScope {
        metric: Models.INumericMetric;
        metricGroup: Models.IMetricGroup;
        files: any[];
        errFiles: any;
        errorMsg: string;

        uploadFiles: any;
        abort: any;
        abortForOneFile: any;
        validateFile: any;
        deleteAttachment: (attachment: Models.IAttachment) => void;
        deleteFile: (index: number) => void;
    }

    class AttachmentMetricController implements IAttachmentMetricController {

        uploadInstance: any;
        uploadIndex: number;

        static $inject: string[] = ['$scope', 'Upload'];

        constructor(private $scope: IAttachmentMetricControllerScope,
            private Upload: any) {

            this.uploadIndex = 0;
            $scope.uploadFiles = () => {
                this.uploadFiles();
            }

            $scope.abort = () => { this.abort(); };
            $scope.abortForOneFile = (index) => { this.abortForOneFile(index); };
            $scope.validateFile = ($file) => { this.validateFile($file); };
            $scope.deleteAttachment = (attachment) => { this.deleteAttachment(attachment); };
            $scope.deleteFile = (index) => { this.deleteFile(index); };

            this.activate();
        }

        activate() {
            if (_.isEmpty(this.$scope.formValues)) {
                this.$scope.formValue = this.$scope.ctrl.addFormValue(this.$scope.metric, this.$scope.dataListItem, this.$scope.rowNumber);
            }
            else {
                this.$scope.formValue = this.$scope.formValues[0];
            }

            setTimeout(() => {
                this.$scope.$broadcast('angular-xGallerify.refresh');
            }, 1000);
        }

        uploadFiles() {
            if (this.uploadIndex < this.$scope.files.length && !this.uploadInstance) {
                var file = this.$scope.files[this.uploadIndex];
                this.uploadInstance = this.Upload.upload({
                    url: '/api/files',
                    method: 'POST',
                    //  file:file,
                    data: { file: file }
                }).progress((evt) => {
                    file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total + ''));
                }).success((response) => {
                    this.uploadIndex++;
                    this.uploadInstance = null;
                    this.uploadFiles();
                    if (this.$scope.formValue.textValue.length > 0)
                        this.$scope.formValue.textValue += ',';
                    this.$scope.formValue.textValue += response;
                    file.guid = response;

                }).error((response) => {
                    if (response.status > 0)
                        this.$scope.errorMsg = response.status + ': ' + response.data;
                });
            }

        }

        abort() {
            if (this.uploadInstance) {
                this.uploadInstance.abort();
                this.uploadInstance = null;
                this.$scope.files = this.$scope.files.slice(0, this.uploadIndex);
            };
        }

        abortForOneFile(index) {
            if (this.uploadIndex === index) {
                this.uploadInstance.abort();
                this.uploadInstance = null;
            }
            this.$scope.files.splice(index, 1);
            this.$scope.files = this.$scope.files.slice(0);
            this.$scope.uploadFiles();
        }

        validateFile($file) {
            $file.$error = "eelo";
        }

        deleteAttachment(attachment: Models.IAttachment) {
            _.find(this.$scope.formValue.attachments, { 'id': attachment.id }).isDeleted = true;
        }

        deleteFile(index: number) {
            var file = this.$scope.files.splice(index);
            this.$scope.files.splice(index);
            var updatedGuids = this.$scope.formValue.textValue.split(',');
            _.remove(updatedGuids, (g) => { return g === file[0].guid; });
            this.$scope.formValue.textValue = updatedGuids.join(',');
        }
    }

    angular.module("app").controller("attachmentMetricController", AttachmentMetricController);
}