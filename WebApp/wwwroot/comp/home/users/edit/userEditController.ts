module App {
    "use strict";

    interface GenderType {
        id: number;
        label: string;
    }

    interface IUserStatistics {
        hasProject: boolean;

        totalThreads: number;
        totalAdviceThreads: number;
        totalThreadRecords: number;
        totalAdviceRecords: number;
        totalRecords: number;
        totalLocations: number;
        totalAttachments: number;
        totalAttachmentsSize: number;
        totalAttachmentsSizeInKB: number;
        totalAttachmentsSizeInMB: number;

        currentMonthThreadRecords: number;
        currentMonthAdviceRecords: number;
        currentMonthRecords: number;
        currentMonthLocations: number;
        currentMonthAttachments: number;
        currentMonthAttachmentsSize: number;
        currentMonthAttachmentsSizeInKB: number;
        currentMonthAttachmentsSizeInMB: number;

        threadStats: IThreadStat[];
        attachmentStats: IAttachmentStats;
    }

    interface IThreadStat {
        title: string;
        color: string;
        discriminator: number;
        records: number;
    }

    interface IAttachmentStats {
        totalSize: number;
        totalSizeInKB: number;
        totalSizeInMB: number;
        types: IAttachmentStatItem[];
    }

    interface IAttachmentStatItem {
        name: string;
        count: number;
        totalSize: number;
        totalSizeInKB: number;
        totalSizeInMB: number;
    }

    interface IUserEditControllerScope extends ng.IScope {
        title: string;
        emailConfirmed: boolean;
        phoneNumberConfirmed: boolean;
        accountType: string;
        parentBreadcrumb: string;

        userStats: IUserStatistics;
    }

    interface IUserEditController {
        user: Models.IOrgUser;
        types: Models.IOrgUserType[];
        errors: string;
        birthDateCalendar: any;
        teams: Models.IOrgTeam[];
        currentUserIsSuperUser: boolean;
        projects: Models.IProject[];
        organisations: Models.IOrganisation[];
        subscriptions: Models.ISubscriptionEntry[];
        displayedSubscriptions: Models.ISubscriptionEntry[];

        submit: (form: ng.IFormController) => void;
        clearErrors: () => void;
        activate: () => void;
        openBirthDateCalendar: () => void;
    }

    class UserEditController implements IUserEditController {
        user: Models.IOrgUser;
        types: Models.IOrgUserType[];
        isInsertMode: boolean;
        errors: string;
        genderTypes: GenderType[];
        birthDateCalendar = { isOpen: false };
        teams: Models.IOrgTeam[];
        currentUserIsSuperUser: boolean;
        projects: Models.IProject[];
        organisations: Models.IOrganisation[];
        subscriptions: Models.ISubscriptionEntry[];
        displayedSubscriptions: Models.ISubscriptionEntry[];

        static $inject: string[] = ["$scope", "orgUserResource", "orgUserTypeResource", "$state", "$stateParams", "organisationResource",
            "orgTeamResource", "userContextService", "projectResource", "toastr", "userResource", "subscriptionResource", "$resource"];
        constructor(
            private $scope: IUserEditControllerScope,
            private orgUserResource: Resources.IOrgUserResource,
            private orgUserTypeResource: Resources.IOrgUserTypeResource,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private organisationResource: Resources.IOrganisationResource,
            private orgTeamResource: Resources.IOrgTeamResource,
            private userContextService: Services.IUserContextService,
            private projectResource: Resources.IProjectResource,
            private toastr: any,
            private userResource: Resources.IUserResource,
            private subscriptionResource: Resources.ISubscriptionResource,
            private $resource: ng.resource.IResourceService
        ) {
            this.$scope.title = "Users";

            this.genderTypes = [
                { id: 0, label: 'Male' },
                { id: 1, label: 'Female' },
                { id: 2, label: 'Other' }
            ];

            this.activate();
        }

        activate() {
            var userId = this.$stateParams['id'];
            if (userId == undefined || userId == '') {
                this.isInsertMode = true;
                userId = '00000000-0000-0000-0000-000000000000';
            } else {
                this.isInsertMode = false;
            }

            var accountType = this.$stateParams["accountType"];
            if (accountType == undefined || !accountType.length) {
                accountType = 'web-account';
            }

            var allowedAccountTypes = ['web-account', 'mobile-account'];
            if (allowedAccountTypes.indexOf(accountType) == -1)
                this.$state.go('home.dashboard.layout');

            this.$scope.accountType = accountType;

            if (this.isInsertMode && this.userContextService.userIsInAnyRoles(['Organisation administrator']) && accountType == 'mobile-account') {
                this.$state.go('home.dashboard.layout');
            }

            var roles = ["System administrator", "Platform administrator"];
            this.currentUserIsSuperUser = this.userContextService.userIsInAnyRoles(roles);
            
            if (accountType === 'web-account')
                this.$state.current.ncyBreadcrumb.parent = 'home.users.list';
            else if (accountType === 'mobile-account')
                this.$state.current.ncyBreadcrumb.parent = 'home.users.mobile';

            if (accountType === 'web-account') {
                this.orgUserTypeResource.query().$promise.then((types) => {
                    this.types = types;
                });
            }

            if (!this.isInsertMode) {
                this.orgUserResource.get({ id: userId }).$promise.then((user) => {
                    this.user = user;

                    // put these on $scope for lm-form-group bindings
                    this.$scope.emailConfirmed = user.emailConfirmed;
                    this.$scope.phoneNumberConfirmed = user.phoneNumberConfirmed;

                    this.orgTeamResource.getUserTeams({ userId: user.id }, (teams) => {
                        this.teams = teams;
                    });

                    if (user.currentProject !== null) {
                        // get user statistics
                        this.$resource("/api/stats/user/" + userId, null).get().$promise.then((data: any) => {
                            this.$scope.userStats = data;
                            this.loadCharts();
                        }, (err) => {
                            console.error(err);
                        });
                    }
                });
            }

            if (this.currentUserIsSuperUser) {
                this.organisationResource.query().$promise.then((organisations) => {
                    this.organisations = organisations;
                });
            }

            if (this.currentUserIsSuperUser && (accountType === 'web-account' || accountType === 'mobile-account' && !this.isInsertMode)) {
                this.projectResource.query().$promise.then((projects) => {
                    this.projects = projects;
                });
            }

            var adminRoles = ["System administrator", "Platform administrator", "Organisation administrator"];
            var isAdmin = this.userContextService.userIsInAnyRoles(adminRoles);
            if (isAdmin && !this.isInsertMode && accountType === 'mobile-account') {
                this.subscriptionResource.getUserSubscriptions({ id: userId }, (data) => {
                    this.subscriptions = data;
                    this.displayedSubscriptions = data;
                }, (err) => {
                    console.error(err);
                });
            }
        }

        loadCharts() {
            var stats = this.$scope.userStats;

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

            this.$scope.threadsChartOpts = chartOpts;
            this.$scope.threadsChartData = [];
            this.$scope.threadsChartLabels = [];
            this.$scope.threadsChartColors = [];

            this.$scope.adviceThreadsChartOpts = chartOpts;
            this.$scope.adviceThreadsChartData = [];
            this.$scope.adviceThreadsChartLabels = [];
            this.$scope.adviceThreadsChartColors = [];

            var threads = _.filter(stats.threadStats, (item) => { return item.discriminator === 0 });
            var adviceThreads = _.filter(stats.threadStats, (item) => { return item.discriminator === 1 });

            _.forEach(threads, (item) => {
                this.$scope.threadsChartData.push(item.records);
                this.$scope.threadsChartLabels.push(item.title);
                this.$scope.threadsChartColors.push(item.color);
            });

            _.forEach(adviceThreads, (item) => {
                this.$scope.adviceThreadsChartData.push(item.records);
                this.$scope.adviceThreadsChartLabels.push(item.title);
                this.$scope.adviceThreadsChartColors.push(item.color);
            });

            // attachments charts
            this.$scope.attachmentsChartOpts = chartOpts;
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
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            var userId = this.$stateParams['id'];
            if (userId === '') {
                if (this.$scope.accountType === 'mobile-account')
                    this.user.accountType = 0;
                else if (this.$scope.accountType === 'web-account')
                    this.user.accountType = 1;

                this.orgUserResource.save(
                    this.user,
                    () => {
                        if (this.$scope.accountType === 'web-account')
                            this.$state.go('home.users.list');
                        else if (this.$scope.accountType === 'mobile-account')
                            this.$state.go('home.users.mobile');
                    },
                    (err) => {
                        if (err.status === 400)
                            this.toastr.error(err.data.message);
                        this.errors = err.data.exceptionMessage;
                    });
            }
            else {
                this.orgUserResource.update(
                    this.user,
                    () => {
                        if (this.$scope.accountType === 'web-account')
                            this.$state.go('home.users.list');
                        else if (this.$scope.accountType === 'mobile-account')
                            this.$state.go('home.users.mobile');
                    },
                    (err) => {
                        if (err.status === 400)
                            this.toastr.error(err.data.message);
                        this.errors = err.data.exceptionMessage;
                    });
            }
        }

        clearErrors() {
            this.errors = undefined;
        }

        openBirthDateCalendar() {
            this.birthDateCalendar.isOpen = true;
        }

        closeSubscription(entry: Models.ISubscriptionEntry) {
            var params = {
                type: entry.type,
                recordId: ''
            };

            if (entry.type === 1) params.recordId = entry.id;
            else params.recordId = entry.paymentRecordId;

            this.subscriptionResource.closeSubscription(params, (res) => {
                //location.reload(true);
                var userId = this.$stateParams['id'];
                this.subscriptionResource.getUserSubscriptions({ id: userId }, (data) => {
                    this.subscriptions = data;
                }, (err) => {
                    console.error(err);
                });
            }, (err) => {
                console.error(err);
                if (err.data.message)
                    this.toastr.error(err.data.message);
            });
        }

        removeSubscription(entry: Models.ISubscriptionEntry) {
            var params = {
                type: entry.type,
                recordId: ''
            };

            if (entry.type === 1) params.recordId = entry.id;
            else params.recordId = entry.paymentRecordId;

            this.subscriptionResource.removeSubscription(params, (res) => {
                //location.reload(true);
                var userId = this.$stateParams['id'];
                this.subscriptionResource.getUserSubscriptions({ id: userId }, (data) => {
                    this.subscriptions = data;
                }, (err) => {
                    console.error(err);
                });
            }, (err) => {
                if (err.data.message)
                    this.toastr.error(err.data.message);
            });
        }

        joinOnRecord() {
            this.subscriptionResource.joinOnRecord({ userId: this.user.id }, (res) => {
                location.reload(true);
            }, (err) => {
                console.error(err);
                if (err.data.message)
                    this.toastr.error(err.data.message);
            });
        }
    }

    angular.module("app").controller("userEditController", UserEditController);
}