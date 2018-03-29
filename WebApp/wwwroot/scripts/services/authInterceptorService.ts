module App.Services {
    "use strict";

    export interface IAuthInterceptorService {
        request: (config: ng.IRequestConfig) => ng.IRequestConfig;
        responseError: (rejection: ng.IHttpPromiseCallbackArg<any>) => ng.IHttpPromiseCallbackArg<any>;
    }

    class AuthInterceptorService implements IAuthInterceptorService {
        static $inject: string[] = ['$q', '$location', 'authService'];

        constructor(
            private $q: ng.IQService,
            private $location: ng.ILocationService,
            private authService: IAuthService) { }

        request: (config: ng.IRequestConfig) => ng.IRequestConfig = (config) => {
            config.headers = config.headers || {};

            var authData = this.authService.getExistingAuthData();
            if (authData) {
                config.headers['Authorization'] = 'Bearer ' + authData.token;
            }

            return config;
        }

        responseError: (rejection: ng.IHttpPromiseCallbackArg<any>) => ng.IHttpPromiseCallbackArg<any> = (rejection) => {
            if (rejection.status === 401) {
                this.$location.path('/login');
            }

            return this.$q.reject(rejection);
        }
    }

    angular.module('app').service('authInterceptorService', AuthInterceptorService);
}