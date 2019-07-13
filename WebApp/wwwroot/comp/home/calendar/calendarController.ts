module App {
    "use strict";

    interface ICalendarControllerScope extends ng.IScope {
        title: string;
        today: Date;
        calendarView: string;
        viewDate: Date;
        events: any[];
        calendarTitle: string;
    }

    interface ICalendarController {
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        projects: Models.IProject[];
        selectedProject: Models.IProject;
        isCurrentUserAnAdmin: boolean;

        activate: () => void;
    }

    class CalendarController implements ICalendarController {
        formTemplates: Models.IFormTemplate[] = [];
        surveys: Models.ISurvey[] = [];
        projects: Models.IProject[] = [];
        selectedProject: Models.IProject;
        isCurrentUserAnAdmin: boolean;
        noRecordsFound: boolean;

        static $inject: string[] = ["$scope", "$state", "$stateParams", "$uibModal", "$q",
            "projectResource", "formTemplateResource", "surveyResource", "userContextService", "toastr"];
        constructor(
            private $scope: ICalendarControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private $q: ng.IQService,
            private projectResource: Resources.IProjectResource,
            private formTemplateResource: Resources.IFormTemplateResource,
            private surveyResource: Resources.ISurveyResource,
            private userContextService: Services.IUserContextService,
            private toastr: any) {

            this.activate();
        }

        activate() {
            this.$scope.title = "Calendar";
            this.$scope.today = new Date();
            this.$scope.calendarView = 'month';
            this.$scope.viewDate = this.$scope.today;
            this.$scope.calendarTitle = "Records Calendar";

            var roles = ["System administrator", "Platform administrator", "Organisation administrator"];
            this.isCurrentUserAnAdmin = this.userContextService.userIsInAnyRoles(roles);

            if (this.isCurrentUserAnAdmin) {
                this.loadProjects();
            } else {
                this.load();
            }
        }

        load() {
            var self = this;

            var promises = [];
            if (this.selectedProject && this.selectedProject !== null) {
                promises.push(this.formTemplateResource.query({ discriminator: 0, projectId: this.selectedProject.id }).$promise);
                promises.push(this.surveyResource.query({ discriminator: 0, projectId: this.selectedProject.id }).$promise);
            } else {
                promises.push(this.formTemplateResource.query({ discriminator: 0 }).$promise);
                promises.push(this.surveyResource.query({ discriminator: 0 }).$promise);
            }

            this.$q.all(promises).then((data: any) => {
                this.formTemplates = data[0];
                this.surveys = data[1];
                this.$scope.events = [];
                this.noRecordsFound = this.surveys.length < 1;

                _.forEach(this.surveys, (survey) => {
                    var template = _.filter(this.formTemplates, (t) => { return t.id === survey.formTemplateId; });
                    if (template.length) {
                        var event = {
                            surveyId: survey.id,
                            projectId: survey.projectId,
                            title: survey.description,
                            startsAt: survey.date,
                            color: {
                                primary: template[0].colour,
                                secondary: template[0].colour
                            },
                            actions: [{
                                label: '<i class=\'fa fa-eye\'></i>',
                                cssClass: 'view-action',
                                onClick: function (args) {
                                    var surveyId = args.calendarEvent.surveyId;
                                    var projectId = args.calendarEvent.projectId;
                                    self.$state.go('home.surveys.view', { projectId: projectId, surveyId: surveyId });
                                },
                            }, {
                                label: '<i class=\'fa fa-pencil\'></i>',
                                cssClass: 'edit-action',
                                onClick: function (args) {
                                    var surveyId = args.calendarEvent.surveyId;
                                    var projectId = args.calendarEvent.projectId;
                                    self.$state.go('home.surveys.edit', { projectId: projectId, surveyId: surveyId });
                                }
                            }],
                            draggable: false,
                            resizable: false,
                            incrementsBadgeTotal: true,
                            cssClass: 'calendar-event-item'
                        };

                        this.$scope.events.push(event);
                    }
                });
            });
        }

        loadProjects() {
            this.projectResource.query().$promise.then((projects) => {
                this.projects = projects;
            }, (error) => {
                console.error(error);
            });
        }

        selectedProjectChanged() {
            if (this.selectedProject) {
                this.load();
            }
        }

        goToday() {
            this.$scope.today = new Date();
            this.$scope.viewDate = this.$scope.today;
        }

        goPrevious() {
            switch (this.$scope.calendarView) {
                case 'day': {
                    this.$scope.today = moment(this.$scope.today).subtract(1, 'days').toDate();
                    break;
                }
                case 'week': {
                    this.$scope.today = moment(this.$scope.today).subtract(1, 'weeks').toDate();
                    break;
                }
                case 'month': {
                    this.$scope.today = moment(this.$scope.today).subtract(1, 'months').toDate();
                    break;
                }
                case 'year': {
                    this.$scope.today = moment(this.$scope.today).subtract(1, 'years').toDate();
                    break;
                }
            }

            this.$scope.viewDate = this.$scope.today;
        }

        goNext() {
            switch (this.$scope.calendarView) {
                case 'day': {
                    this.$scope.today = moment(this.$scope.today).add(1, 'days').toDate();
                    break;
                }
                case 'week': {
                    this.$scope.today = moment(this.$scope.today).add(1, 'weeks').toDate();
                    break;
                }
                case 'month': {
                    this.$scope.today = moment(this.$scope.today).add(1, 'months').toDate();
                    break;
                }
                case 'year': {
                    this.$scope.today = moment(this.$scope.today).add(1, 'years').toDate();
                    break;
                }
            }

            this.$scope.viewDate = this.$scope.today;
        }

        onViewChangeClick(calendarDate: any, calendarNextView: any) {
            this.$scope.today = moment(calendarDate).toDate();
            this.$scope.viewDate = this.$scope.today;
            this.$scope.calendarView = calendarNextView;
        }

        timespanClicked(calendarDate: any) {
            this.$scope.today = calendarDate;
            this.$scope.viewDate = calendarDate;
        }

        eventClicked(calendarEvent: any) {
            this.$scope.today = calendarEvent.startsAt;
            this.$scope.viewDate = calendarEvent.startsAt;
        }

        changeCalendarView(mode: string) {
            this.$scope.calendarView = mode;
        }

    }

    angular.module("app").controller("calendarController", CalendarController);
}