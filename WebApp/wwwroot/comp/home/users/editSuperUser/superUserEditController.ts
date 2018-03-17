
module App {
    "use strict";

    interface GenderType {
        id: number;
        label: string;
    }

    interface ISuperUserEditControllerScope extends ng.IScope {
        emailConfirmed: boolean;
        phoneNumberConfirmed: boolean;
    }

    interface ISuperUserEditController {
        title: string;
        user: Models.IUser;
        errors: string;
        birthDateCalendar: any;
        teams: Models.IOrgTeam[];

        submit: (form: ng.IFormController) => void;
        clearErrors: () => void;
        activate: () => void;
        openBirthDateCalendar: () => void;
    }

    class SuperUserEditController implements ISuperUserEditController {
        title: string;
        user: Models.IUser;
        isInsertMode: boolean;
        errors: string;
        genderTypes: GenderType[];
        birthDateCalendar = { isOpen: false };
        teams: Models.IOrgTeam[];

        static $inject: string[] = ["$scope", "$state", "$stateParams", "orgTeamResource", "userContextService", "toastr", "userResource"];
        constructor(
            private $scope: ISuperUserEditControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private orgTeamResource: Resources.IOrgTeamResource,
            private userContextService: Services.IUserContextService,
            private toastr: any,
            private userResource: Resources.IUserResource
        ) {
            this.title = "Super Users";

            this.genderTypes = [
                { id: 0, label: 'Male' },
                { id: 1, label: 'Female' },
                { id: 2, label: 'Other' }
            ];

            this.activate();
        }

        activate() {
            var userId = this.$stateParams['id'];
            if (userId === '') {
                this.isInsertMode = true;
                userId = '00000000-0000-0000-0000-000000000000';
            } else {
                this.userResource.get({ id: userId }).$promise.then((user) => {
                    this.user = user;

                    // put these on $scope for lm-form-group bindings
                    this.$scope.emailConfirmed = user.emailConfirmed;
                    this.$scope.phoneNumberConfirmed = user.phoneNumberConfirmed;

                    this.orgTeamResource.getUserTeams({ userId: user.id }, (teams) => {
                        this.teams = teams;
                    });
                });
            }
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            var userId = this.$stateParams['id'];
            if (userId === '') {
                this.userResource.save(
                    this.user,
                    () => { this.$state.go('home.users.list'); },
                    (err) => {
                        if (err.status === 400)
                            this.toastr.error(err.data.message);
                        this.errors = err.data.exceptionMessage;
                    });
            }
            else {
                this.userResource.update(
                    this.user,
                    () => { this.$state.go('home.users.list'); },
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
    }

    angular.module("app").controller("superUserEditController", SuperUserEditController);
}