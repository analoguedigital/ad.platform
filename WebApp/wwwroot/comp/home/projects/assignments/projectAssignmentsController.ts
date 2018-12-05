
module App {
    "use strict";

    interface IProjectAssignmentsControllerScope extends ng.IScope {
        title: string;
        accountTypeFilter: string;
    }

    interface IProjectAssignmentsController {
        project: Models.IProject;
        errors: string;
        safeUserAssignments: IAssignmentUser[];
        userAssignments: IAssignmentUser[];
        displayedAssignments: IAssignmentUser[];
        searchTerm: string;
        clearErrors: () => void;
    }

    interface IAssignmentUser {
        userId: string;
        name: string;
        email: string;
        accountType: number;
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
        projectId: string;
        project: Models.IProject;
        users: Models.IOrgUser[];
        assignments: Models.IProjectAssignment[];
        safeUserAssignments: IAssignmentUser[];
        userAssignments: IAssignmentUser[];
        displayedAssignments: IAssignmentUser[];
        searchTerm: string;
        errors: string;

        static $inject: string[] = ["$scope", "projectResource", "orgUserResource", "$q", "toastr", "$state", "$stateParams"];
        constructor(
            private $scope: IProjectAssignmentsControllerScope,
            private projectResource: Resources.IProjectResource,
            private orgUserResource: Resources.IOrgUserResource,
            private $q: angular.IQService,
            private toastr: any,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService) {

            this.activate();
        }

        activate() {
            this.$scope.title = "Case Assignments"
            this.$scope.accountTypeFilter = 'all';

            this.projectId = this.$stateParams['id'];
            var projectPromise = this.projectResource.get({ id: this.projectId }, (project) => {
                this.project = project;

                this.projectResource.assignments({ id: this.projectId }, (assignments) => {
                    this.assignments = assignments;
                    this.load();
                });
            });
        }

        load() {
            this.orgUserResource.query({ listType: 2, organisationId: this.project.organisation.id }, (users) => {
                this.users = users;
                this.safeUserAssignments = [];
                this.userAssignments = [];

                _.forEach(this.users, (user) => {
                    var userName = user.email;
                    if (user.firstName || user.surname)
                        userName = `${user.firstName} ${user.surname}`;

                    var assgn = _.find(this.assignments, { 'orgUserId': user.id });
                    var userAssignment = <Models.IProjectAssignment>assgn;

                    var record: IAssignmentUser = {
                        userId: user.id,
                        name: userName,
                        email: user.email,
                        accountType: user.accountType,
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

                    this.safeUserAssignments.push(record);
                });

                this.userAssignments = this.safeUserAssignments;
                this.displayedAssignments = [].concat(this.userAssignments);
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

        filterAccounts(type: string) {
            this.$scope.accountTypeFilter = type;

            if (type === 'clients') {
                this.userAssignments = _.filter(this.safeUserAssignments, (a) => { return a.accountType === 0; });
            } else if (type === 'staff') {
                this.userAssignments = _.filter(this.safeUserAssignments, (a) => { return a.accountType === 1; });
            } else if (type === 'all') {
                this.userAssignments = this.safeUserAssignments;
            }

            this.displayedAssignments = [].concat(this.userAssignments);
        }
    }

    angular.module("app").controller("projectAssignmentsController", ProjectAssignmentsController);
}