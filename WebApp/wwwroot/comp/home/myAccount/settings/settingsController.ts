﻿
module App {
    "use strict";

    interface ISecurityInfoModel {
        question: string;
        answer: string;
    }

    interface ISettingsControllerScope extends ng.IScope {
        currentUser: Models.IUser;
        securityInfo: ISecurityInfoModel;
    }

    interface ISettingsController {
        activate: () => void;
    }

    class SettingsController implements ISettingsController {
        static $inject: string[] = ["$scope", "$uibModal", "userContextService", "$resource", "toastr", "$timeout"];
        constructor(
            private $scope: ISettingsControllerScope,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private userContext: Services.IUserContextService,
            private $resource: ng.resource.IResourceService,
            private toastr: any,
            private $timeout: ng.ITimeoutService) {

            $scope.title = "Account Settings";
            this.activate();
        }

        activate() {
            this.$scope.currentUser = this.userContext.current.user;
            this.$scope.securityInfo = { question: '', answer: '' };

            this.$resource("/api/account/get-security-qa").get().$promise.then(
                (res) => {
                    var question = res.question;
                    var answer = res.answer;

                    if (question !== null)
                        this.$scope.securityInfo.question = question.toString();
                    if (answer !== null)
                        this.$scope.securityInfo.answer = answer;
                }, (err) => {
                    console.error(err);
                });
        }

        confirmEmailAddress() {
            // TODO
        }

        addPhoneNumber() {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/myAccount/settings/addPhoneNumber/addPhoneNumber.html',
                controller: 'addPhoneNumberController',
                controllerAs: 'ctrl',
                resolve: {
                    number: () => {
                        return '';
                    }
                }
            }).result.then(
                (res) => {
                    if (res) {
                        this.$timeout(() => { location.reload(); }, 2000);
                    }
                },
                (err) => {
                    console.error(err);
                });
        }

        verifyPhoneNumber() {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/myAccount/settings/addPhoneNumber/addPhoneNumber.html',
                controller: 'addPhoneNumberController',
                controllerAs: 'ctrl',
                resolve: {
                    number: () => {
                        return this.userContext.current.user.phoneNumber;
                    }
                }
            }).result.then(
                (res) => {
                    if (res) {
                        this.$timeout(() => { location.reload(); }, 2000);
                    }
                },
                (err) => {
                    console.error(err);
                });
        }

        changePhoneNumber() {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/myAccount/settings/changePhoneNumber/changePhoneNumber.html',
                controller: 'changePhoneNumberController',
                controllerAs: 'ctrl',
                resolve: {
                    phoneNumber: () => {
                        return this.userContext.current.user.phoneNumber;
                    }
                }
            }).result.then(
                (res) => {
                    if (res) {
                        this.$timeout(() => { location.reload(); }, 2000);
                    }
                },
                (err) => {
                    console.error(err);
                });
        }

        removePhoneNumber() {
            this.$resource("/api/account/removePhoneNumber").save().$promise.then(
                (result) => {
                    this.toastr.success("You have removed your phone number");
                    this.$timeout(() => {
                        location.reload();
                    }, 1000);
                },
                (err) => {
                    this.toastr.error('Action failed. Check for errors.');
                    if (err.status === 400) {
                        this.toastr.error(err.data.message);
                    }
                });
        }

        saveSecurityInfo() {
            this.$resource("/api/account/set-security-qa").save(this.$scope.securityInfo).$promise.then(
                (res) => {
                    console.info(res);

                    this.toastr.success('Security information saved');
                }, (err) => {
                    console.error(err);
                });
        }
    }

    angular.module("app").controller("settingsController", SettingsController);
}