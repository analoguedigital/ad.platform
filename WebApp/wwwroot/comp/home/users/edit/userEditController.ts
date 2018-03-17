
module App {
    "use strict";

    interface GenderType {
        id: number;
        label: string;
    }

    interface IUserEditControllerScope extends ng.IScope {
        emailConfirmed: boolean;
        phoneNumberConfirmed: boolean;
    }

    interface IUserEditController {
        title: string;
        user: any;
        userType: string;
        types: Models.IOrgUserType[];
        errors: string;
        birthDateCalendar: any;
        teams: Models.IOrgTeam[];
        currentUserIsSuperUser: boolean;
        projects: Models.IProject[];

        submit: (form: ng.IFormController) => void;
        clearErrors: () => void;
        activate: () => void;
        openBirthDateCalendar: () => void;
    }

    class UserEditController implements IUserEditController {
        title: string;
        user: any;
        userType: string;
        types: Models.IOrgUserType[];
        isInsertMode: boolean;
        errors: string;
        genderTypes: GenderType[];
        birthDateCalendar = { isOpen: false };
        teams: Models.IOrgTeam[];
        currentUserIsSuperUser: boolean;
        projects: Models.IProject[];

        static $inject: string[] = ["$scope", "orgUserResource", "orgUserTypeResource", "$state", "$stateParams",
            "orgTeamResource", "userContextService", "projectResource", "toastr", "userResource"];
        constructor(
            private $scope: IUserEditControllerScope,
            private orgUserResource: Resources.IOrgUserResource,
            private orgUserTypeResource: Resources.IOrgUserTypeResource,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private orgTeamResource: Resources.IOrgTeamResource,
            private userContextService: Services.IUserContextService,
            private projectResource: Resources.IProjectResource,
            private toastr: any,
            private userResource: Resources.IUserResource
        ) {
            this.title = "Users";

            this.genderTypes = [
                { id: 0, label: 'Male' },
                { id: 1, label: 'Female' },
                { id: 2, label: 'Other' }
            ];

            var userType = this.$state.params["userType"];
            if (!userType || userType.length < 1) userType = 'orguser';
            this.userType = userType;

            this.activate();
        }

        activate() {
            if (this.userType == 'orguser') {
                this.orgUserTypeResource.query().$promise.then((types) => {
                    this.types = types;
                });
            }

            var userId = this.$stateParams['id'];
            if (userId === '') {
                this.isInsertMode = true;
                userId = '00000000-0000-0000-0000-000000000000';
            } else {
                if (this.userType == 'orguser') {
                    this.orgUserResource.get({ id: userId }).$promise.then((user) => {
                        this.user = user;

                        // put these on $scope for lm-form-group bindings
                        this.$scope.emailConfirmed = user.emailConfirmed;
                        this.$scope.phoneNumberConfirmed = user.phoneNumberConfirmed;

                        this.orgTeamResource.getUserTeams({ userId: user.id }, (teams) => {
                            this.teams = teams;
                        });
                    });
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

            var roles = ["System administrator", "Platform administrator"];
            this.currentUserIsSuperUser = this.userContextService.userIsInAnyRoles(roles);

            if (this.currentUserIsSuperUser && this.userType == 'orguser') {
                this.projectResource.query().$promise.then((projects) => {
                    this.projects = projects;
                });
            }
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            var userId = this.$stateParams['id'];
            if (userId === '') {
                this.orgUserResource.save(
                    this.user,
                    () => { this.$state.go('home.users.list'); },
                    (err) => {
                        if (err.status === 400)
                            this.toastr.error(err.data.message);
                        this.errors = err.data.exceptionMessage;
                    });
            }
            else {
                if (this.userType === 'orguser') {
                    this.orgUserResource.update(
                        this.user,
                        () => { this.$state.go('home.users.list'); },
                        (err) => {
                            if (err.status === 400)
                                this.toastr.error(err.data.message);
                            this.errors = err.data.exceptionMessage;
                        });
                } else {
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
        }

        clearErrors() {
            this.errors = undefined;
        }

        openBirthDateCalendar() {
            this.birthDateCalendar.isOpen = true;
        }
    }

    angular.module("app").controller("userEditController", UserEditController);
}