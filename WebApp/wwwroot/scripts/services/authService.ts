
module App.Services {
    "use strict";

    export interface IAuthContext {
        isAuth: boolean;
        email: string;
    }

    class AuthContext implements IAuthContext {
        public isAuth: boolean = false;
        public email: string = '';
    }

    export class AuthData {
        constructor(
            public token: string,
            public email: string) { }
    }

    export interface IAuthService {
        authContext: IAuthContext;

        loginUser(authenticationData: AuthData): void;
        logOutUser(): void;
        getExistingAuthData(): AuthData;
    }

    class AuthService implements IAuthService {

        LOCAL_STORAGE_KEY: string = 'authenticationData';
        authContext: IAuthContext;

        static $inject: string[] = ['$q', 'localStorageService'];
        constructor(
            private $q: ng.IQService,
            private localStorageService: ng.local.storage.ILocalStorageService) {

            this.authContext = new AuthContext;
            var existing = this.getExistingAuthData();

            if (existing !== null) {
                this.authContext.email = existing.email;
                this.authContext.isAuth = true;
            }

        }

        loginUser(authenticationData: AuthData): void {
            this.localStorageService.set(this.LOCAL_STORAGE_KEY, authenticationData);
            this.authContext.isAuth = true;
            this.authContext.email = authenticationData.email;
        }


        getExistingAuthData(): AuthData {
            return this.localStorageService.get<AuthData>(this.LOCAL_STORAGE_KEY);
        }


        logOutUser(): void {
            this.localStorageService.remove(this.LOCAL_STORAGE_KEY);
            this.authContext.isAuth = false;
            this.authContext.email = "";
        }
    }

    angular.module('app').service("authService", AuthService);
}
