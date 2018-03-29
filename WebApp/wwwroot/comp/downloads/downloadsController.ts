module App {
    "use strict";

    interface IDownloadsControllerScope extends angular.IScope {
        title: string;
        downloadId: string;
    }

    interface IDownloadsController {
        activate: () => void;
    }

    class DownloadsController implements IDownloadsController {
        errors: Error[] = [];
        static $inject: string[] = ["$scope", "$stateParams", "$ngBootbox", "toastr", "downloadResource"];

        constructor(
            private $scope: IDownloadsControllerScope,
            private $stateParams: ng.ui.IStateParamsService,
            private $ngBootbox: BootboxStatic,
            private toastr: any,
            private downloadResource: Resources.IDownloadResource) {

            $scope.title = "Download Center";
            this.activate();
        }

        activate() {
            var downloadId = this.$stateParams["id"];
            this.$scope.downloadId = downloadId;

            if (downloadId && downloadId.length) {
                this.downloadResource.requestFile({ id: downloadId }, (result) => {
                    var accessId = result.accessId;
                    var url = "/api/downloads/" + accessId;
                    location.href = url;
                }, (err) => {
                    if (err.status === 404) {
                        this.toastr.error('Download not found!');
                    }
                });
            }
        }
    }

    angular.module("app").controller("downloadsController", DownloadsController);
}