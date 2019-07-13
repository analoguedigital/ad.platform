module App {
    "use strict";

    interface IPlatformStats {
        totalOrganizations: number;
        totalCases: number;

        totalThreads: number;
        totalAdviceThreads: number;

        totalThreadRecords: number;
        totalAdviceRecords: number;
        totalRecords: number;

        totalThreadAttachments: number;
        totalAdviceThreadAttachments: number;
        totalAttachments: number;

        totalMobileAccounts: number;
        totalWebAccounts: number;
        totalAccounts: number;

        totalSystemAdmins: number;
        totalPlatformAdmins: number;
        totalAdmins: number;

        attachmentStats: IAttachmentStats;
        teamStats: ITeamStats[];
        unconfirmedAccounts: IOrgUserFlatDTO[];
        pastMonthAccounts: IOrgUserFlatDTO[];
    }

    interface ITeamStats {
        name: string;
        color: string;
        organization: string;
        members: number;
    }

    interface IAttachmentStats {
        totalSize: number;
        totalSizeInKB: number;
        totalSizeInMB: number;
        types: IAttachmentTypeStats[];
    }

    interface IAttachmentTypeStats {
        name: string;
        count: number;
        totalSize: number;
        totalSizeInKB: number;
        totalSizeInMB: number;
    }

    interface IOrgUserFlatDTO {
        id: string;
        email: string;
        firstName: string;
        surname: string;
        gender?: number;
        birthdate?: Date;
        address: string;
        isActive: boolean;
        lastLogin?: Date;
        isWebUser: boolean;
        isMobileUser: boolean;
        isRootUser: boolean;
        accountType: number;
    }

    interface IStatisticsControllerScope extends ng.IScope {
        title: string;
        stats: IPlatformStats;

        accountTypeFilter: string;
        unconfirmedAccountsSearchTerm: string;

        pastMonthAccountsFilter: string;
        pastMonthAccountsSearchTerm: string;
    }

    interface IStatisticsController {
        sourceUnconfirmedAccounts: IOrgUserFlatDTO[];
        unconfirmedAccounts: IOrgUserFlatDTO[];
        displayedUnconfirmedAccounts: IOrgUserFlatDTO[];

        sourcePastMonthAccounts: IOrgUserFlatDTO[];
        pastMonthAccounts: IOrgUserFlatDTO[];
        displayedPastMonthAccounts: IOrgUserFlatDTO[];

        activate: () => void;
    }

    class StatisticsController implements IStatisticsController {
        sourceUnconfirmedAccounts: IOrgUserFlatDTO[] = [];
        unconfirmedAccounts: IOrgUserFlatDTO[] = [];
        displayedUnconfirmedAccounts: IOrgUserFlatDTO[] = [];

        sourcePastMonthAccounts: IOrgUserFlatDTO[] = [];
        pastMonthAccounts: IOrgUserFlatDTO[] = [];
        displayedPastMonthAccounts: IOrgUserFlatDTO[] = [];

        unconfirmedAccountsClientsCount: number;
        unconfirmedAccountsStaffCount: number;
        unconfirmedAccountsAdminsCount: number;

        pastMonthAccountsClientsCount: number;
        pastMonthAccountsStaffCount: number;
        pastMonthAccountsAdminsCount: number;

        static $inject: string[] = ['$scope', "$resource", "localStorageService"];
        constructor(
            private $scope: IStatisticsControllerScope,
            private $resource: ng.resource.IResourceService,
            private localStorageService: ng.local.storage.ILocalStorageService) {

            $scope.title = "Statistics";

            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            this.$resource("/api/stats/platform", null).get().$promise.then((data: any) => {
                this.$scope.stats = data;
                this.loadCharts();
                this.loadCounters();
            }, (err) => {
                console.error(err);
            });
        }

        loadCharts() {
            var chartOpts = {
                type: 'pie',
                responsive: true,
                maintainAspectRatio: true,
                showTooltips: true,
                hover: {
                    mode: false
                },
                legend: {
                    display: true,
                    position: 'bottom'
                }
            };

            var stats = this.$scope.stats;

            this.$scope.usersChartOpts = chartOpts;
            this.$scope.threadsChartOpts = chartOpts;
            this.$scope.adminsChartOpts = chartOpts;
            this.$scope.recordsChartOpts = chartOpts;
            this.$scope.threadRecordsChartOpts = chartOpts;
            this.$scope.adviceRecordsChartOpts = chartOpts;
            this.$scope.attachmentsChartOpts = chartOpts;
            //this.$scope.teamsChartOpts = chartOpts;

            this.$scope.usersChartData = [stats.totalMobileAccounts, stats.totalWebAccounts];
            this.$scope.usersChartLabels = ['Clients', 'Staff'];
            this.$scope.usersChartColors = [];

            this.$scope.threadsChartData = [stats.totalThreads, stats.totalAdviceThreads];
            this.$scope.threadsChartLabels = ['Threads', 'Advice Threads'];
            this.$scope.threadsChartColors = [];

            this.$scope.adminsChartData = [stats.totalSystemAdmins, stats.totalPlatformAdmins];
            this.$scope.adminsChartLabels = ['Sys Admins', 'Platform Admins'];
            this.$scope.adminsChartColors = [];

            this.$scope.recordsChartData = [stats.totalRecords, stats.totalAttachments];
            this.$scope.recordsChartLabels = ['Records', 'Attachments'];
            this.$scope.recordsChartColors = [];

            this.$scope.threadRecordsChartData = [stats.totalThreadRecords, stats.totalThreadAttachments];
            this.$scope.threadRecordsChartLabels = ['Records', 'Attachments'];
            this.$scope.threadRecordsChartColors = [];

            this.$scope.adviceRecordsChartData = [stats.totalAdviceRecords, stats.totalAdviceThreadAttachments];
            this.$scope.adviceRecordsChartLabels = ['Records', 'Attachments'];
            this.$scope.adviceRecordsChartColors = [];

            this.$scope.attachmentsChartData = [];
            this.$scope.attachmentsChartLabels = [];
            this.$scope.attachmentsChartColors = [];

            var attachmentTypes = stats.attachmentStats.types;
            _.forEach(attachmentTypes, (type) => {
                this.$scope.attachmentsChartLabels.push(type.name);
                this.$scope.attachmentsChartData.push(type.count);
            });

            this.$scope.attachmentsSizeChartOpts = {
                type: 'pie',
                responsive: true,
                maintainAspectRatio: true,
                showTooltips: true,
                hover: {
                    mode: false
                },
                legend: {
                    display: true,
                    position: 'bottom'
                },
                tooltips: {
                    callbacks: {
                        label: function (tooltipItem, data) {
                            var label = data.labels[tooltipItem.index];
                            var totalSize = data.datasets[0].data[tooltipItem.index];

                            return ' ' + label + ': ' + totalSize + ' MB';
                        }
                    }
                }
            };

            this.$scope.attachmentsSizeChartData = [];
            this.$scope.attachmentsSizeChartLabels = [];
            this.$scope.attachmentsSizeChartColors = [];

            _.forEach(attachmentTypes, (type) => {
                this.$scope.attachmentsSizeChartLabels.push(type.name);
                this.$scope.attachmentsSizeChartData.push(type.totalSizeInMB.toFixed(2));
            });

            // teams chart
            //this.$scope.teamsChartData = [];
            //this.$scope.teamsChartLabels = [];
            //this.$scope.teamsChartColors = [];

            //_.forEach(stats.teamStats, (team) => {
            //    this.$scope.teamsChartLabels.push(team.name);
            //    this.$scope.teamsChartColors.push(team.color);
            //    this.$scope.teamsChartData.push(team.members);
            //});

            // unconfirmed accounts
            this.sourceUnconfirmedAccounts = stats.unconfirmedAccounts;
            this.unconfirmedAccounts = stats.unconfirmedAccounts;
            this.displayedUnconfirmedAccounts = stats.unconfirmedAccounts;

            // past month accounts
            this.sourcePastMonthAccounts = stats.pastMonthAccounts;
            this.pastMonthAccounts = stats.pastMonthAccounts;
            this.displayedPastMonthAccounts = stats.pastMonthAccounts;

            var unconfirmedAccountsFilter = <string>this.localStorageService.get('stats_unconfirmed_accounts_filter');
            var pastMonthAccountsFilter = <string>this.localStorageService.get('stats_recent_accounts_filter');
            if (unconfirmedAccountsFilter == null || !unconfirmedAccountsFilter.length)
                unconfirmedAccountsFilter = 'clients';
            if (pastMonthAccountsFilter == null || !pastMonthAccountsFilter.length)
                pastMonthAccountsFilter = 'clients';

            this.filterUnconfirmedAccounts(unconfirmedAccountsFilter);
            this.filterPastMonthAccounts(pastMonthAccountsFilter);
        }

        loadCounters() {
            this.unconfirmedAccountsClientsCount = _.filter(this.sourceUnconfirmedAccounts, (item) => { return item.accountType == 0; }).length;
            this.unconfirmedAccountsStaffCount = _.filter(this.sourceUnconfirmedAccounts, (item) => { return item.accountType == 1 && !item.isRootUser; }).length;
            this.unconfirmedAccountsAdminsCount = _.filter(this.sourceUnconfirmedAccounts, (item) => { return item.isRootUser == true; }).length;

            this.pastMonthAccountsClientsCount = _.filter(this.sourcePastMonthAccounts, (item) => { return item.accountType == 0; }).length;
            this.pastMonthAccountsStaffCount = _.filter(this.sourcePastMonthAccounts, (item) => { return item.accountType == 1 && !item.isRootUser; }).length;
            this.pastMonthAccountsAdminsCount = _.filter(this.sourcePastMonthAccounts, (item) => { return item.isRootUser == true; }).length;
        }

        formatBytes(a, b) {
            // https://stackoverflow.com/questions/15900485/correct-way-to-convert-size-in-bytes-to-kb-mb-gb-in-javascript

            if (0 == a) return "0 Bytes";
            var c = 1024;
            var d = b || 2;
            var e = ["Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
            var f = Math.floor(Math.log(a) / Math.log(c));

            return parseFloat((a / Math.pow(c, f)).toFixed(d)) + " " + e[f];
        }

        filterUnconfirmedAccounts(filter: string) {
            this.$scope.accountTypeFilter = filter;
            this.localStorageService.set('stats_unconfirmed_accounts_filter', filter);

            switch (filter) {
                case 'clients': {
                    this.unconfirmedAccounts = _.filter(this.sourceUnconfirmedAccounts, (a) => { return a.accountType === 0 });
                    break;
                }
                case 'staff': {
                    this.unconfirmedAccounts = _.filter(this.sourceUnconfirmedAccounts, (a) => { return a.accountType === 1 && !a.isRootUser });
                    break;
                }
                case 'admins': {
                    this.unconfirmedAccounts = _.filter(this.sourceUnconfirmedAccounts, (a) => { return a.accountType === 1 && a.isRootUser });
                    break;
                }
                case 'all': {
                    this.unconfirmedAccounts = this.sourceUnconfirmedAccounts;
                    break;
                }
            }

            this.displayedUnconfirmedAccounts = [].concat(this.unconfirmedAccounts);
        }

        filterPastMonthAccounts(filter: string) {
            this.$scope.pastMonthAccountsFilter = filter;
            this.localStorageService.set('stats_recent_accounts_filter', filter);

            switch (filter) {
                case 'clients': {
                    this.pastMonthAccounts = _.filter(this.sourcePastMonthAccounts, (a) => { return a.accountType === 0 });
                    break;
                }
                case 'staff': {
                    this.pastMonthAccounts = _.filter(this.sourcePastMonthAccounts, (a) => { return a.accountType === 1 && !a.isRootUser });
                    break;
                }
                case 'admins': {
                    this.pastMonthAccounts = _.filter(this.sourcePastMonthAccounts, (a) => { return a.accountType === 1 && a.isRootUser });
                    break;
                }
                case 'all': {
                    this.pastMonthAccounts = this.sourcePastMonthAccounts;
                    break;
                }
            }

            this.displayedPastMonthAccounts = [].concat(this.pastMonthAccounts);
        }

    }

    angular.module("app").controller("statisticsController", StatisticsController);
}