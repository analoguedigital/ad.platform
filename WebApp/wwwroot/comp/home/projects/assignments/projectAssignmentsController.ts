
module App {
    "use strict";

    interface IProjectAssignmentsController {
        title: string;
        project: Models.IProject;
        errors: string;
        userAssignments: IAssignmentUser[];
        clearErrors: () => void;
    }
    interface IAssignmentUser {
        userId: string;
        name: string;
        canAdd: boolean;
        canEdit: boolean;
        canView: boolean;
        canDelete: boolean;
    }

    class ProjectAssignmentsController implements IProjectAssignmentsController {
        title: string;
        project: Models.IProject;
        users: Models.IOrgUser[];
        assignments: Models.IProjectAssignment[];
        userAssignments: IAssignmentUser[];
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
            var projectPromise = this.projectResource.get({ id: projectId }).$promise;
            projectPromise.then((project) => { this.project = project; });

            var usersPromise = this.orgUserResource.query().$promise;
            usersPromise.then((users) => { this.users = users; });

            this.$q.all([projectPromise, usersPromise]).then(() => {
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
                            canAdd: userAssignment ? userAssignment.canAdd : false,
                            canEdit: userAssignment ? userAssignment.canEdit : false,
                            canView: userAssignment ? userAssignment.canView : false,
                            canDelete: userAssignment ? userAssignment.canDelete : false
                        };

                        this.userAssignments.push(record);
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
        }

        clearErrors() {
            this.errors = undefined;
        }

    }

    angular.module("app").controller("projectAssignmentsController", ProjectAssignmentsController);
}