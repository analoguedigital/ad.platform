
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
        isAssigned: boolean;
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
                            isAssigned: _.some(assignments, { 'orgUserId': user.id }),
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

        updateAssignment(assg: IAssignmentUser) {
            var params = {
                id: this.project.id,
                userId: assg.userId,
                canAdd: assg.canAdd,
                canEdit: assg.canEdit,
                canView: assg.canView,
                canDelete: assg.canDelete
            };

            this.projectResource.assign(params,
                () => {
                    assg.isAssigned = true;
                    this.toastr.success('User assigned successfully!', 'Success', {
                        closeButton: true
                    });
                },
                (err) => {
                    this.toastr.error('Unable to assign the user!', 'Error', {
                        closeButton: true
                    });
                });
        }

        deleteAssignment(assg: IAssignmentUser) {
            if (confirm('Are you sure you want to remove this assignment?')) {
                this.projectResource.unassign({ id: this.project.id, userId: assg.userId },
                    () => {
                        assg.isAssigned = false;
                        assg.canAdd = false;
                        assg.canEdit = false;
                        assg.canDelete = false;
                        assg.canView = false;

                        this.toastr.success('User removed from the project successfully!', 'Success', {
                            closeButton: true
                        });
                    },
                    (err) => {
                        this.toastr.error('Unable to remove the user from the project!', 'Error', {
                            closeButton: true
                        });
                    });
            }
        }

        clearErrors() {
            this.errors = undefined;
        }

    }

    angular.module("app").controller("projectAssignmentsController", ProjectAssignmentsController);
}