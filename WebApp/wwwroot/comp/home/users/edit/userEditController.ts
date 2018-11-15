module App {
    "use strict";

    interface GenderType {
        id: number;
        label: string;
    }

    interface IUserEditControllerScope extends ng.IScope {
        title: string;
        emailConfirmed: boolean;
        phoneNumberConfirmed: boolean;
        accountType: string;
        parentBreadcrumb: string;
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

        static $inject: string[] = ["$scope", "orgUserResource", "orgUserTypeResource", "$state", "$stateParams", "organisationResource",
            "orgTeamResource", "userContextService", "projectResource", "toastr", "userResource", "subscriptionResource"];
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
            private subscriptionResource: Resources.ISubscriptionResource
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
            var accountType = this.$stateParams["accountType"];
            if (accountType === undefined || !accountType.length) {
                console.warn('accountType param is undefined!');
                accountType = 'web-account';
            }

            this.$scope.accountType = accountType;
            if (accountType === 'web-account')
                this.$state.current.ncyBreadcrumb.parent = 'home.users.list';
            else if (accountType === 'mobile-account')
                this.$state.current.ncyBreadcrumb.parent = 'home.users.mobile';

            if (accountType === 'web-account') {
                this.orgUserTypeResource.query().$promise.then((types) => {
                    this.types = types;
                });
            }

            var userId = this.$stateParams['id'];
            if (userId === '') {
                this.isInsertMode = true;
                userId = '00000000-0000-0000-0000-000000000000';
            } else {
                this.orgUserResource.get({ id: userId }).$promise.then((user) => {
                    this.user = user;

                    // put these on $scope for lm-form-group bindings
                    this.$scope.emailConfirmed = user.emailConfirmed;
                    this.$scope.phoneNumberConfirmed = user.phoneNumberConfirmed;

                    this.orgTeamResource.getUserTeams({ userId: user.id }, (teams) => {
                        this.teams = teams;
                    });
                });
            }

            var roles = ["System administrator", "Platform administrator"];
            this.currentUserIsSuperUser = this.userContextService.userIsInAnyRoles(roles);

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
            if (isAdmin && !this.isInsertMode) {
                this.subscriptionResource.getUserSubscriptions({ id: userId }, (data) => {
                    this.subscriptions = data;
                }, (err) => {
                    console.error(err);
                });
            }
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
                location.reload(true);
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
                location.reload(true);
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