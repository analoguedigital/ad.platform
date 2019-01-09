module App.Services {
    "use strict";

    export interface ILoginData {
        email: string;
        password: string;
    }

    export interface IHttpService {
        getAuthenticationToken(loginData: ILoginData): ng.IPromise<any>;
        getTwoFactorAuthToken(loginData: ILoginData, token: string): ng.IPromise<any>;

        getUserInfo(): ng.IPromise<any>;
        getServiceBase(): string;
    }

    class HttpService implements IHttpService {
        static $inject: string[] = ['$http', '$q'];
        serviceBase: string = '/';

        constructor(
            private $http: ng.IHttpService,
            private $q: ng.IQService) { }

        getAuthenticationToken(loginData: ILoginData): ng.IPromise<any> {
            var deferred = this.$q.defer();
            var data = "grant_type=password&username=" + loginData.email + "&password=" + loginData.password;

            this.$http.post(this.serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
                .then((value) => {
                    deferred.resolve(value.data);
                }, (err) => {
                    deferred.reject(this.onError(err));
                });

            return deferred.promise;
        }

        getTwoFactorAuthToken(loginData: ILoginData, token: string): ng.IPromise<any> {
            var deferred = this.$q.defer();
            var data = "grant_type=password&username=" + loginData.email + "&password=" + loginData.password;

            this.$http.post(this.serviceBase + 'token', data, {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'X-2FA-Token': token
                }
            })
                .then((value) => {
                    deferred.resolve(value.data);
                }, (err) => {
                    deferred.reject(this.onError(err));
                });

            return deferred.promise;
        }

        getUserInfo(): ng.IPromise<any> {
            var deferred = this.$q.defer();

            this.$http.get(this.serviceBase + 'api/account/userinfo/')
                .then((value) => {
                    deferred.resolve(value.data);
                }, (err) => {
                    deferred.reject(this.onError(err));
                });

            return deferred.promise;
        }

        onError(err: any) {
            if (err.data) {
                if (err.data.error_description) {
                    return err.data.error_description;
                } else {
                    return err.data;
                }
            } else {
                return err.status + ': Server connection failed!';
            }
        }

        getServiceBase() {
            return this.serviceBase;
        }

    }

    angular.module('app').service("httpService", HttpService);
}