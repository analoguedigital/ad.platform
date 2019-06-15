module App.Services {
    "use strict";

    export interface IUserContext {
        user: Models.IUser;
        orgUser: Models.IOrgUser;
    }

    export interface IUserContextService {
        current: IUserContext;
        login(loginData: ILoginData): ng.IPromise<void>;
        twoFactorLogin(loginData: ILoginData, token: string): ng.IPromise<void>;
        logout(): void;
        userIsInAnyRoles(role: string[]): boolean;
        userIsRestricted(): boolean;
        initialize(): ng.IPromise<IUserContext>;

        setCurrentUser(authenticationData: AuthData): ng.IPromise<void>;
    }

    export class UserContextService implements IUserContextService {
        LOCAL_STORAGE_KEY: string = 'userContext';

        public current: IUserContext = null;

        public role_system_admin = 'System administrator';
        public role_org_admin = 'Organisation administrator';

        static $inject: string[] = ['$q', '$http', 'httpService', 'authService', 'orgUserResource', 'toastr'];
        constructor(
            private $q: ng.IQService,
            private $http: ng.IHttpService,
            private httpService: IHttpService,
            private authService: IAuthService,
            private orgUserResource: Resources.IOrgUserResource,
            private toastr: any) {

            this.current = <IUserContext>{ user: null, orgUser: null };

        }

        public initialize(): ng.IPromise<IUserContext> {

            var defer = this.$q.defer();

            if (this.authService.authContext.isAuth) {
                this.setCurrentUser(this.authService.getExistingAuthData()).then(() => {
                    defer.resolve(this.current);
                });
            } else {
                defer.resolve(this.current);
            }

            return defer.promise;
        }

        public userIsInAnyRoles(roles: string[]): boolean {
            return _.intersection(this.current.user.roles, roles).length > 0;
        }

        login(loginData: ILoginData): ng.IPromise<void> {
            var defer = this.$q.defer<void>();

            this.httpService.getAuthenticationToken(loginData)
                .then((response) => {
                    // user is authenicated 
                    var authenticationData = new App.Services.AuthData(response.access_token, loginData.email);
                    this.authService.loginUser(authenticationData);
                    this.setCurrentUser(authenticationData)
                        .then(() => {
                            defer.resolve();
                        }, (err) => {
                            defer.reject(err);
                        });
                }, (err) => {
                    // user is not authenicated 
                    this.authService.logOutUser();
                    this.logout();
                    defer.reject(err);
                });

            return defer.promise;
        }

        twoFactorLogin(loginData: ILoginData, token: string): ng.IPromise<void> {
            var defer = this.$q.defer<void>();

            this.httpService.getTwoFactorAuthToken(loginData, token)
                .then((response) => {
                    // user is authenicated 
                    var authenticationData = new App.Services.AuthData(response.access_token, loginData.email);
                    this.authService.loginUser(authenticationData);
                    this.setCurrentUser(authenticationData)
                        .then(() => {
                            defer.resolve();
                        }, (err) => {
                            defer.reject(err);
                        });
                }, (err) => {
                    // user is not authenicated 
                    this.authService.logOutUser();
                    this.logout();
                    defer.reject(err);
                });

            return defer.promise;
        }

        logout() {
            this.authService.logOutUser();
            this.current.user = null;
            this.current.orgUser = null;

            // this isn't enough as we need to actually call our api 
            // to log out the current session. refactor this after enabling 2FA.
        }

        setCurrentUser(authenticationData: AuthData): ng.IPromise<void> {
            var deferred = this.$q.defer<void>();

            this.httpService.getUserInfo()
                .then((userinfo) => {
                    this.current.user = <Models.IUser>{
                        id: userinfo.userId,
                        email: authenticationData.email,
                        emailConfirmed: userinfo.emailConfirmed,
                        phoneNumber: userinfo.phoneNumber,
                        phoneNumberConfirmed: userinfo.phoneNumberConfirmed,
                        roles: userinfo.roles,
                        securityQuestionEnabled: userinfo.securityQuestionEnabled,
                        notifications: userinfo.notifications
                    };

                    if (userinfo.organisationId !== null) {
                        this.orgUserResource.get({ id: this.current.user.id }).$promise
                            .then((orguser: Models.IOrgUser) => {
                                this.current.orgUser = orguser;
                                this.current.orgUser.quota = userinfo.profile.monthlyQuota;

                                deferred.resolve();
                            }, (err) => {
                                deferred.reject(err);
                            });
                    } else {
                        deferred.resolve();
                    }
                }, (err) => {
                    deferred.reject(err);
                });

            return deferred.promise;
        }

        userIsRestricted() {
            var restrictedUserRole = "Restricted user";
            return this.userIsInAnyRoles(new Array(restrictedUserRole));
        }
    }

    angular.module("app").service("userContextService", UserContextService);
}