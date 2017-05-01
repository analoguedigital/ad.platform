
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

                    angular.forEach(this.users, (user) => {
                        this.userAssignments.push({ userId: user.id, name: user.firstName + ' ' + user.surname, isAssigned: _.some(assignments, { orgUserId: user.id }) });
                    });
                });
            });
        }

        updateAssignment(assg: IAssignmentUser) {
            if (assg.isAssigned) {
                this.projectResource.assign({ id: this.project.id, userId: assg.userId },
                    () => {
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
            else {
                this.projectResource.unassign({ id: this.project.id, userId: assg.userId },
                    () => {
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