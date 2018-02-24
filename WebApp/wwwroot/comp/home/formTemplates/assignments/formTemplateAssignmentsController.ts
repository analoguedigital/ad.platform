
module App {
    "use strict";

    interface IFormTemplateAssignmentsController {
        title: string;
        activate: () => void;
        formTemplate: Models.IFormTemplate;
        orgUsers: Models.IOrgUser[];
        userAssignments: IThreadAssignmentUser[];
        displayedAssignments: IThreadAssignmentUser[];
        searchTerm: string;
    }

    interface IThreadAssignmentUser {
        userId: string;
        name: string;
        email: string;
        isRootUser: boolean;
        isWebUser: boolean;
        isMobileUser: boolean;
        canAdd: boolean;
        canEdit: boolean;
        canView: boolean;
        canDelete: boolean;
    }

    class FormTemplateAssignmentsController implements IFormTemplateAssignmentsController {
        title: string = "Form template assignments";
        formTemplateId: string;
        formTemplate: Models.IFormTemplate;
        orgUsers: Models.IOrgUser[];
        userAssignments: IThreadAssignmentUser[];
        displayedAssignments: IThreadAssignmentUser[];
        searchTerm: string;

        static $inject: string[] = ["$scope", "$state", "$stateParams", "$uibModal", "formTemplateResource", "orgUserResource"];

        constructor(
            private $scope: ng.IScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private formTemplateResource: Resources.IFormTemplateResource,
            private orgUserResource: Resources.IOrgUserResource
        ) {
            this.formTemplateId = $stateParams['id'];
            this.activate();
        }

        activate() {
            if (this.formTemplateId === '') {
                this.formTemplateId = '00000000-0000-0000-0000-000000000000';

                this.formTemplateResource.get({ id: this.formTemplateId }).$promise.then((form) => {
                    this.formTemplate = form;
                });
            } else {
                this.formTemplateResource.get({ id: this.formTemplateId }).$promise.then((form) => {
                    this.formTemplate = form;

                    this.orgUserResource.query({ organisationId: this.formTemplate.organisation.id }).$promise.then((users) => {
                        this.orgUsers = users;
                        this.userAssignments = [];

                        this.formTemplateResource.getAssignments({ id: this.formTemplateId }, (assignments) => {
                            _.forEach(users, (user) => {
                                var userName = user.email;
                                if (user.firstName || user.surname)
                                    userName = `${user.firstName} ${user.surname}`;

                                var assgn = _.find(assignments, { 'orgUserId': user.id });
                                var userAssignment = <Models.IThreadAssignment>assgn;

                                var record: IThreadAssignmentUser = {
                                    userId: user.id,
                                    name: userName,
                                    email: user.email,
                                    isRootUser: user.isRootUser,
                                    isWebUser: user.isWebUser,
                                    isMobileUser: user.isMobileUser,
                                    canAdd: userAssignment ? userAssignment.canAdd : false,
                                    canEdit: userAssignment ? userAssignment.canEdit : false,
                                    canView: userAssignment ? userAssignment.canView : false,
                                    canDelete: userAssignment ? userAssignment.canDelete : false
                                };

                                this.userAssignments.push(record);
                            });

                            this.displayedAssignments = [].concat(this.userAssignments);
                        });
                    });
                });
            }
        }

        updateAssignment(assignment: IThreadAssignmentUser, accessLevel: string) {
            var params = {
                id: this.formTemplate.id,
                userId: assignment.userId,
                accessLevel: accessLevel
            };

            var toggled = false;
            switch (accessLevel) {
                case 'allowAdd': {
                    toggled = assignment.canAdd;
                    break;
                }
                case 'allowEdit': {
                    toggled = assignment.canEdit;
                    break;
                }
                case 'allowDelete': {
                    toggled = assignment.canDelete;
                    break;
                }
                case 'allowView': {
                    toggled = assignment.canView;
                    break;
                }
            }

            if (toggled) {
                this.formTemplateResource.assign(params, (result: Models.IThreadAssignment) => {
                    this.refreshAssignment(assignment, result);
                }, (error) => { });
            } else {
                this.formTemplateResource.unassign(params, (result: Models.IThreadAssignment) => {
                    this.refreshAssignment(assignment, result);
                }, (error) => { });
            }
        }

        refreshAssignment(assignment: IThreadAssignmentUser, newValue: Models.IThreadAssignment) {
            assignment.canView = newValue.canView;
            assignment.canAdd = newValue.canAdd;
            assignment.canEdit = newValue.canEdit;
            assignment.canDelete = newValue.canDelete;
        }
    }

    angular.module("app").controller("formTemplateAssignmentsController", FormTemplateAssignmentsController);
}