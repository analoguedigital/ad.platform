
module App {
    "use strict";

    interface IProjectAssignmentsController {
        title: string;
        project: Models.IProject;
        errors: string;
        userAssignments: IAssignmentUser[];
        displayedAssignments: IAssignmentUser[];
        searchTerm: string;
        clearErrors: () => void;
    }
    interface IAssignmentUser {
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
        canExportPdf: boolean;
        canExportZip: boolean;
    }

    class ProjectAssignmentsController implements IProjectAssignmentsController {
        title: string;
        project: Models.IProject;
        users: Models.IOrgUser[];
        assignments: Models.IProjectAssignment[];
        userAssignments: IAssignmentUser[];
        displayedAssignments: IAssignmentUser[];
        searchTerm: string;
        errors: string;

        static $inject: string[] = ["projectResource", "orgUserResource", "$q", "toastr", "$state", "$stateParams"];
        constructor(
            private projectResource: Resources.IProjectResource,
            private orgUserResource: Resources.IOrgUserResource,
            private $q: angular.IQService,
            private toastr: any,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService) {

            this.title = "Projects";
            this.activate();
        }

        activate() {
            var projectId = this.$stateParams['id'];
            var projectPromise = this.projectResource.get({ id: projectId }, (project) => {
                this.project = project;

                this.orgUserResource.query({ organisationId: this.project.organisation.id }, (users) => {
                    this.users = users;

                    this.projectResource.assignments({ id: projectId }, (assignments) => {
                        this.userAssignments = [];

                        _.forEach(this.users, (user) => {
                            var userName = user.email;
                            if (user.firstName || user.surname)
                                userName = `${user.firstName} ${user.surname}`;

                            var assgn = _.find(assignments, { 'orgUserId': user.id });
                            var userAssignment = <Models.IProjectAssignment>assgn;

                            var record: IAssignmentUser = {
                                userId: user.id,
                                name: userName,
                                email: user.email,
                                isRootUser: user.isRootUser,
                                isWebUser: user.isWebUser,
                                isMobileUser: user.isMobileUser,
                                canAdd: userAssignment ? userAssignment.canAdd : false,
                                canEdit: userAssignment ? userAssignment.canEdit : false,
                                canView: userAssignment ? userAssignment.canView : false,
                                canDelete: userAssignment ? userAssignment.canDelete : false,
                                canExportPdf: userAssignment ? userAssignment.canExportPdf : false,
                                canExportZip: userAssignment ? userAssignment.canExportZip : false
                            };

                            this.userAssignments.push(record);
                        });

                        this.displayedAssignments = [].concat(this.userAssignments);
                    });
                });
            });
        }

        updateAssignment(assignment: IAssignmentUser, accessLevel: string) {
            var params = {
                id: this.project.id,
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
                case 'allowExportPdf': {
                    toggled = assignment.canExportPdf;
                    break;
                }
                case 'allowExportZip': {
                    toggled = assignment.canExportZip;
                    break;
                }
            }

            if (toggled) {
                this.projectResource.assign(params, (result: Models.IProjectAssignment) => {
                    this.refreshAssignment(assignment, result);
                }, (error) => { });
            } else {
                this.projectResource.unassign(params, (result: Models.IProjectAssignment) => {
                    this.refreshAssignment(assignment, result);
                }, (error) => { });
            }
        }

        refreshAssignment(assignment: IAssignmentUser, newValue: Models.IProjectAssignment) {
            assignment.canView = newValue.canView;
            assignment.canAdd = newValue.canAdd;
            assignment.canEdit = newValue.canEdit;
            assignment.canDelete = newValue.canDelete;
            assignment.canExportPdf = newValue.canExportPdf;
            assignment.canExportZip = newValue.canExportZip;
        }

        clearErrors() {
            this.errors = undefined;
        }

    }

    angular.module("app").controller("projectAssignmentsController", ProjectAssignmentsController);
}