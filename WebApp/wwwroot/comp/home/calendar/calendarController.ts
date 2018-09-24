module App {
    "use strict";

    interface ICalendarControllerScope extends ng.IScope {
        title: string;
        calendarView: string;
        viewDate: Date;
        events: any[];
        calendarTitle: string;
    }

    interface ICalendarController {
        activate: () => void;
    }

    class CalendarController implements ICalendarController {
        static $inject: string[] = ["$scope", "$stateParams", "$uibModal", "projectResource"];

        constructor(
            private $scope: ICalendarControllerScope,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: ng.ui.bootstrap.IModalService) {

            this.activate();
        }

        activate() {
            this.$scope.title = "Calendar";
            this.$scope.calendarView = 'month';
            this.$scope.viewDate = new Date();
            this.$scope.calendarTitle = "My Calendar Title";
            this.$scope.events = [
                {
                    title: 'My event title', // The title of the event
                    startsAt: new Date(2018, 8, 17), // A javascript date object for when the event starts
                    endsAt: new Date(2018, 8, 17), // Optional - a javascript date object for when the event ends
                    color: { // can also be calendarConfig.colorTypes.warning for shortcuts to the deprecated event types
                        primary: '#e3bc08', // the primary event color (should be darker than secondary)
                        secondary: '#fdf1ba' // the secondary event color (should be lighter than primary)
                    },
                    actions: [{ // an array of actions that will be displayed next to the event title
                        label: '<i class=\'glyphicon glyphicon-pencil\'></i>', // the label of the action
                        cssClass: 'edit-action', // a CSS class that will be added to the action element so you can implement custom styling
                        onClick: function (args) { // the action that occurs when it is clicked. The first argument will be an object containing the parent event
                            console.log('Edit event', args.calendarEvent);
                        }
                    }],
                    draggable: false, //Allow an event to be dragged and dropped
                    resizable: false, //Allow an event to be resizable
                    incrementsBadgeTotal: true, //If set to false then will not count towards the badge total amount on the month and year view
                    recursOn: 'year', // If set the event will recur on the given period. Valid values are year or month
                    cssClass: 'a-css-class-name', //A CSS class (or more, just separate with spaces) that will be added to the event when it is displayed on each view. Useful for marking an event as selected / active etc
                    allDay: false // set to true to display the event as an all day event on the day view
                }
            ];

            this.load();
        }

        load() { }

    }

    angular.module("app").controller("calendarController", CalendarController);
}