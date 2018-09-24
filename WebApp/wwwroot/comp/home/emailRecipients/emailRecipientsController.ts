module App {
    "use strict";

    interface IEmailRecipientsControllerScope extends angular.IScope {
        title: string;
        searchTerm: string;
    }

    interface IEmailRecipientsController {
        activate: () => void;
    }

    interface IEmailRecipientRecord {
        userId: string;
        name: string;
        email: string;
        isRootUser: boolean;
        feedbacks: boolean;
        newMobileUsers: boolean;
        orgRequests: boolean;
        orgConnectionRequests: boolean;
    }

    class EmailRecipientsController implements IEmailRecipientsController {
        users: Models.IOrgUser[];
        recipients: Models.IEmailRecipient[];
        recipientRecords: IEmailRecipientRecord[];
        displayedRecipients: IEmailRecipientRecord[];

        static $inject: string[] = ["$scope", "$ngBootbox", "toastr", "orgUserResource", "emailRecipientResource"];
        constructor(
            private $scope: IEmailRecipientsControllerScope,
            private $ngBootbox: BootboxStatic,
            private toastr: any,
            private orgUserResource: Resources.IOrgUserResource,
            private emailRecipientResource: Resources.IEmailRecipientResource) {

            $scope.title = "Email Recipients";
            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            this.orgUserResource.getOnRecordStaff((users) => {
                this.users = users;

                this.emailRecipientResource.query().$promise.then((recipients) => {
                    this.recipients = recipients;

                    this.recipientRecords = [];
                    this.displayedRecipients = [];

                    _.forEach(this.users, (user) => {
                        var userName = user.email;
                        if (user.firstName || user.surname)
                            userName = `${user.firstName} ${user.surname}`;

                        var recipient = _.find(this.recipients, { 'orgUserId': user.id });
                        if (recipient) {
                            var record: IEmailRecipientRecord = {
                                userId: user.id,
                                name: recipient.orgUserName,
                                email: user.email,
                                isRootUser: user.isRootUser,
                                feedbacks: recipient ? recipient.feedbacks : false,
                                newMobileUsers: recipient ? recipient.newMobileUsers : false,
                                orgRequests: recipient ? recipient.orgRequests : false,
                                orgConnectionRequests: recipient ? recipient.orgConnectionRequests : false
                            };

                            this.recipientRecords.push(record);
                        }
                        else {
                            this.recipientRecords.push(<IEmailRecipientRecord>{
                                userId: user.id,
                                name: user.firstName + ' ' + user.surname,
                                email: user.email,
                                isRootUser: user.isRootUser,
                                feedbacks: false,
                                newMobileUsers: false,
                                orgRequests: false,
                                orgConnectionRequests: false
                            });
                        }
                    });

                    this.displayedRecipients = [].concat(this.recipientRecords);
                }, (err) => {
                    console.log(err);
                });
            }, (err) => {
                console.log(err);
            });
        }

        updateRecipient(record: IEmailRecipientRecord, flag: string) {
            var params = {
                userId: record.userId,
                flag: flag
            };

            var toggled = false;
            switch (flag) {
                case 'feedbacks': {
                    toggled = record.feedbacks;
                    break;
                }
                case 'newMobileUsers': {
                    toggled = record.newMobileUsers;
                    break;
                }
                case 'orgRequests': {
                    toggled = record.orgRequests;
                    break;
                }
                case 'orgConnectionRequests': {
                    toggled = record.orgConnectionRequests;
                    break;
                }
            }

            if (toggled) {
                this.emailRecipientResource.assign(params, (result: Models.IEmailRecipient) => {
                    this.refreshAssignment(record, result);
                }, (error) => { });
            } else {
                this.emailRecipientResource.unassign(params, (result: Models.IEmailRecipient) => {
                    this.refreshAssignment(record, result);
                }, (error) => { });
            }
        }

        refreshAssignment(record: IEmailRecipientRecord, newValue: Models.IEmailRecipient) {
            record.feedbacks = newValue.feedbacks;
            record.newMobileUsers = newValue.newMobileUsers;
            record.orgRequests = newValue.orgRequests;
            record.orgConnectionRequests = newValue.orgConnectionRequests;
        }

    }

    angular.module("app").controller("emailRecipientsController", EmailRecipientsController);
}