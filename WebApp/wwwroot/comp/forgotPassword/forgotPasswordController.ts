module App {
    "use strict";

    interface IForgotPasswordModel {
        emailAddress: string;
        securityAnswer: string;
    }

    interface IForgotPasswordControllerScope extends ng.IScope {
        model: IForgotPasswordModel;
        resetPassword: (form: ng.IFormController) => void;
        requestSent: boolean;

        securityQAEnabled: boolean;
        securityQuestions: string[];
        securityQuestion: string;
    }

    interface IForgotPasswordController {
        activate: () => void;

        goNext: (form: ng.IFormController) => void;
        verifyAnswer: (form: ng.IFormController) => void;
        resetPassword: (form: ng.IFormController) => void;
    }

    class ForgotPasswordController implements IForgotPasswordController {
        static $inject: string[] = ["$scope", "$resource", "$state", "toastr", "userContextService"];
        constructor(
            private $scope: IForgotPasswordControllerScope,
            private $resource: ng.resource.IResourceService,
            private $state: ng.ui.IStateService,
            private toastr: any,
            private userContextService: App.Services.IUserContextService) {
            this.activate();
        }

        activate() {
            this.$scope.model = { emailAddress: '', securityAnswer: '' };
            this.$scope.securityQuestions = [];

            this.$scope.goNext = (form: ng.IFormController) => { this.goNext(form); };
            this.$scope.verifyAnswer = (form: ng.IFormController) => { this.verifyAnswer(form); };
            this.$scope.resetPassword = (form: ng.IFormController) => { this.resetPassword(form); };

            this.populateQuestions();
        }

        populateQuestions() {
            this.$scope.securityQuestions.push('What was your childhood nickname?');
            this.$scope.securityQuestions.push('In what city did you meet your spouse/significant other?');
            this.$scope.securityQuestions.push('What is the name of your favorite childhood friend?');
            this.$scope.securityQuestions.push('What street did you live on in third grade?');
            this.$scope.securityQuestions.push('What is your oldest sibling’s birthday month and year? (e.g., January 1900)');
            this.$scope.securityQuestions.push('What is the middle name of your youngest child?');
            this.$scope.securityQuestions.push("What is your oldest sibling's middle name?");
            this.$scope.securityQuestions.push('What school did you attend for sixth grade?');
            this.$scope.securityQuestions.push('What was your childhood phone number including area code? (e.g., 000-000-0000)');
            this.$scope.securityQuestions.push("What is your oldest cousin's first and last name?");
            this.$scope.securityQuestions.push('What was the name of your first stuffed animal?');
            this.$scope.securityQuestions.push('In what city or town did your mother and father meet?');
            this.$scope.securityQuestions.push('Where were you when you had your first kiss?');
            this.$scope.securityQuestions.push('What is the first name of the boy or girl that you first kissed?');
            this.$scope.securityQuestions.push('What was the last name of your third grade teacher?');
            this.$scope.securityQuestions.push('In what city does your nearest sibling live?');
            this.$scope.securityQuestions.push("What is your youngest brother’s birthday month and year? (e.g., January 1900)");
            this.$scope.securityQuestions.push("What is your maternal grandmother's maiden name?");
            this.$scope.securityQuestions.push('In what city or town was your first job?');
            this.$scope.securityQuestions.push('What is the name of the place your wedding reception was held?');
            this.$scope.securityQuestions.push("What is the name of a college you applied to but didn't attend?");
            this.$scope.securityQuestions.push('Where were you when you first heard about 9/11?');
        }

        goNext(form: ng.IFormController) {
            var payload = { emailAddress: this.$scope.model.emailAddress };
            this.$resource("/api/account/check-security-qa").save(payload).$promise.then(
                (res) => {
                    if (res.enabled) {
                        this.$scope.securityQuestion = this.$scope.securityQuestions[res.questionIndex - 1];
                        this.$scope.securityQAEnabled = true;
                    }
                    else {
                        this.$resource("/api/account/forgotPassword").save({ email: this.$scope.model.emailAddress }).$promise.then(
                            (result) => {
                                this.$scope.requestSent = true;
                            },
                            (err) => {
                                console.error(err);
                                this.toastr.error(err);
                            });
                    }
                }, (err) => {
                    console.error(err);
                    if (err.status == 404)
                        this.toastr.error('Email address not found');
                });
        }

        verifyAnswer(form: ng.IFormController) {
            this.$resource("/api/account/verify-security-answer").save(this.$scope.model).$promise.then(
                (res) => {
                    this.$scope.requestSent = true;
                }, (err) => {
                    if (err.data.message)
                        this.toastr.error(err.data.message);
                });
        }
    }

    angular.module("app").controller("forgotPasswordController", ForgotPasswordController);
}