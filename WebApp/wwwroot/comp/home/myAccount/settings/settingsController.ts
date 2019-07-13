
module App {
    "use strict";

    interface ISecurityQuestion {
        text: string;
        value: number;
    }

    interface ISettingsControllerScope extends ng.IScope {
        currentUser: Models.IUser;
    }

    interface ISettingsController {
        questions: ISecurityQuestion[];
        selectedSecurityQuestion: ISecurityQuestion;
        selectedSecurityAnswer: string;
        securityQuestionEnabled: boolean;

        activate: () => void;
    }

    class SettingsController implements ISettingsController {
        questions: ISecurityQuestion[] = [];
        selectedSecurityQuestion: ISecurityQuestion;
        selectedSecurityAnswer: string;
        securityQuestionEnabled: boolean;

        static $inject: string[] = ["$scope", "$uibModal", "userContextService", "$resource", "toastr", "$timeout", "$http"];
        constructor(
            private $scope: ISettingsControllerScope,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private userContext: Services.IUserContextService,
            private $resource: ng.resource.IResourceService,
            private toastr: any,
            private $timeout: ng.ITimeoutService,
            private $http: ng.IHttpService) {

            $scope.title = "Account Settings";
            this.activate();
        }

        activate() {
            this.$scope.currentUser = this.userContext.current.user;
            this.populateSecurityQuestions();
            this.loadSecurityInfo();
        }

        loadSecurityInfo() {
            this.$resource("/api/account/get-security-qa").get().$promise.then(
                (res) => {
                    if (res.question !== null)
                        this.selectedSecurityQuestion = _.filter(this.questions, (q) => { return q.value === res.question; })[0];
                    else
                        this.selectedSecurityQuestion = null;

                    this.selectedSecurityAnswer = res.answer;

                    this.securityQuestionEnabled = res.question !== null && res.answer !== null;
                }, (err) => {
                    console.error(err);
                });
        }

        populateSecurityQuestions() {
            this.questions.push({ text: 'What was your childhood nickname?', value: 1 });
            this.questions.push({ text: 'In what city did you meet your spouse/significant other?', value: 2 });
            this.questions.push({ text: 'What is the name of your favorite childhood friend?', value: 3 });
            this.questions.push({ text: 'What street did you live on in third grade?', value: 4 });
            this.questions.push({ text: 'What is your oldest sibling’s birthday month and year? (e.g., January 1900)', value: 5 });
            this.questions.push({ text: 'What is the middle name of your youngest child?', value: 6 });
            this.questions.push({ text: "What is your oldest sibling's middle name?", value: 7 });
            this.questions.push({ text: 'What school did you attend for sixth grade?', value: 8 });
            this.questions.push({ text: 'What was your childhood phone number including area code? (e.g., 000-000-0000)', value: 9 });
            this.questions.push({ text: "What is your oldest cousin's first and last name?", value: 10 });
            this.questions.push({ text: 'What was the name of your first stuffed animal?', value: 11 });
            this.questions.push({ text: 'In what city or town did your mother and father meet?', value: 12 });
            this.questions.push({ text: 'Where were you when you had your first kiss?', value: 13 });
            this.questions.push({ text: 'What is the first name of the boy or girl that you first kissed?', value: 14 });
            this.questions.push({ text: 'What was the last name of your third grade teacher?', value: 15 });
            this.questions.push({ text: 'In what city does your nearest sibling live?', value: 16 });
            this.questions.push({ text: 'What is your youngest brother’s birthday month and year? (e.g., January 1900)', value: 17 });
            this.questions.push({ text: "What is your maternal grandmother's maiden name?", value: 18 });
            this.questions.push({ text: 'In what city or town was your first job?', value: 19 });
            this.questions.push({ text: 'What is the name of the place your wedding reception was held?', value: 20 });
            this.questions.push({ text: "What is the name of a college you applied to but didn't attend?", value: 21 });
            this.questions.push({ text: 'Where were you when you first heard about 9/11?', value: 22 });
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
            if (this.selectedSecurityQuestion == null) {
                this.toastr.warning('Please choose a security question first');
                return false;
            }

            if (this.selectedSecurityAnswer == null || !this.selectedSecurityAnswer.length) {
                this.toastr.warning('Please enter your security answer first');
                return false;
            }

            var payload = {
                question: this.selectedSecurityQuestion.value,
                answer: this.selectedSecurityAnswer
            };

            this.$resource("/api/account/set-security-qa").save(payload).$promise.then(
                (res) => {
                    this.toastr.clear();
                    this.toastr.success('Security information saved');
                    this.securityQuestionEnabled = true;
                }, (err) => {
                    console.error(err);
                });
        }

        removeSecurityInfo() {
            this.$resource("/api/account/remove-security-qa").save().$promise.then((res) => {
                this.toastr.success('Security information removed');
                this.loadSecurityInfo();
            }, (err) => {
                console.error(err);
            });
        }
    }

    angular.module("app").controller("settingsController", SettingsController);
}