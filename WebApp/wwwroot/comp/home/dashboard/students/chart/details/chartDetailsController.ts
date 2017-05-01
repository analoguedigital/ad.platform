
module App {
    "use strict";

    interface IChartDetailsControllerScope extends ng.IScope {
        achievement: Models.IDataListItem;
        targets: Models.MBS.ITarget[];
        evidences: Models.MBS.IEvidence[];
        close: () => void;
    }

    interface IChartDetailsController {
        activate: () => void;
    }

    class ChartDetailsController implements IChartDetailsController {

        

        static $inject: string[] = ["$scope", "$uibModalInstance","project", "achievement", "targetResource", "evidenceResource"];
        constructor(private $scope: IChartDetailsControllerScope,
            private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private project: Models.IProject,
            private achievement: Models.IDataListItem,
            private targetResource: Resources.ITargetResource,
            private evidenceResource: Resources.IEvidenceResource) {

            $scope.achievement = achievement;
            $scope.close = () => { this.close(); };
            this.activate();
        }

        activate() {
            this.targetResource.query({ projectId: this.project.id, achievement: this.achievement.id }).$promise.then((targets) => {
                this.$scope.targets = targets;
            });

            this.evidenceResource.query({ projectId: this.project.id, achievement: this.achievement.id }).$promise.then((evidences) => {
                this.$scope.evidences = evidences;
            });
        }

        close() {
            this.$uibModalInstance.dismiss('cancel');
        };
    }

    angular.module("app").controller("chartDetailsController", ChartDetailsController);
}