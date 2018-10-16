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

        static $inject: string[] = ["$scope", "$state", "$stateParams", "$uibModal", "$q",
            "projectResource", "formTemplateResource", "surveyResource", "userContextService"];
        constructor(
            private $scope: ICalendarControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private $q: ng.IQService,
            private projectResource: Resources.IProjectResource,
            private formTemplateResource: Resources.IFormTemplateResource,
            private surveyResource: Resources.ISurveyResource,
            private userContextService: Services.IUserContextService) {

            this.activate();
        }

        activate() {
            this.$scope.title = "Calendar";
            this.$scope.today = new Date();
            this.$scope.calendarView = 'month';
            this.$scope.viewDate = this.$scope.today;
            this.$scope.calendarTitle = "My Calendar Title";

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

                _.forEach(this.surveys, (survey) => {
                    var template = _.filter(this.formTemplates, (t) => { return t.id === survey.formTemplateId; });
                    if (template.length) {
                        var event = {
                            surveyId: survey.id,
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
                                    console.log(args.calendarEvent);
                                    var surveyId = args.calendarEvent.surveyId;
                                    self.$state.go('home.surveys.view', { surveyId: surveyId });
                                },
                            }, {
                                label: '<i class=\'fa fa-pencil\'></i>',
                                cssClass: 'edit-action',
                                onClick: function (args) {
                                    console.log(args.calendarEvent);
                                    var surveyId = args.calendarEvent.surveyId;
                                    self.$state.go('home.surveys.edit', { surveyId: surveyId });
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

        timespanClicked(calendarDate: any) {
            this.$scope.today = calendarDate;
            this.$scope.viewDate = calendarDate;
        }

        eventClicked(calendarEvent: any) {
            this.$scope.today = calendarEvent.startsAt;
            this.$scope.viewDate = calendarEvent.startsAt;
        }
    }

    angular.module("app").controller("calendarController", CalendarController);
}