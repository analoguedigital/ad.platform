
module App {
    "use strict";

    interface IStudentChartController {
        title: string;
        project: Models.IProject;
        achievements: Models.IDataList;
        stages: Models.IDataList;
        dimensions: Models.IDataList;
        achievementSummaries: Models.MBS.IAchievementSummary[];

        activate: () => void;
    }

    class StudentChartController implements IStudentChartController {

        title: string;
        achievements: Models.IDataList;
        stages: Models.IDataList;
        dimensions: Models.IDataList;
        achievementSummaries: Models.MBS.IAchievementSummary[];

        private achievementsDataListId = 'd8a8a1f3-4764-457b-ad6c-5e4753d2eb20';

        static $inject: string[] = ["$uibModal", "project", "dataListResource", "achievementSummaryResource"];
        constructor(
            private $uibModal: ng.ui.bootstrap.IModalService,
            public project: Models.IProject,
            private dataListResource: Resources.IDataListResource,
            private achievementSummaryResource: Resources.IAchievementSummaryResource) {

            this.title = project.name;
            this.activate();
        }

        activate() {
            this.load();
        }

        load() {

            this.achievementSummaryResource.query({ projectId: this.project.id }).$promise.then((summaries) => {
                this.achievementSummaries = summaries;

                this.dataListResource.get({ id: this.achievementsDataListId }).$promise.then((datalist) => {
                    this.achievements = datalist;
                    //this.stages = datalist.relationship1;
                    //this.dimensions = datalist.relationship2;
                });

            });
        }

        getAchievement(stage: Models.IDataListItem, dimension: Models.IDataListItem, index: number) {
          //  return _.filter(this.achievements.items, { attr1Id: stage.id, attr2Id: dimension.id })[index];
        }

        getAchievementSummary(achievement: Models.IDataListItem) {
            if (!achievement)
                return;
            return _.find(this.achievementSummaries, { achievementId: achievement.id });
        }

        openDetails(achievement: Models.IDataListItem) {
            var a = achievement;
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/dashboard/students/chart/details/chartDetailsView.html',
                controller: 'chartDetailsController',
                size: 'lg',
                resolve: {
                    achievement: () => {
                        return a;
                    },
                    project: () => {
                        return this.project;
                    }
                }
            });
        }

    }

    angular.module("app").controller("studentChartController", StudentChartController);
}