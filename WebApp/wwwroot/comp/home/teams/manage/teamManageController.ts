module App {
    "use strict";

    interface IOrganisationTeamManageControllerScope extends ng.IScope {
        title: string;
        team: Models.IOrgTeam;
        projects: Models.IProject[];
        teamProjects: ITeamProject[];
        userAssignments: Models.IProjectAssignment[];
    }

    interface ITeamProjectAssignment {
        userId: string;
        name: string;
        email: string;
        accountType: number;
        isRootUser: boolean;
        isWebUser: boolean;
        isMobileUser: boolean;
        isManager: boolean;
        projectId: string;
        currentProjectId: string;
        canAdd: boolean;
        canEdit: boolean;
        canView: boolean;
        canDelete: boolean;
        canExportPdf: boolean;
        canExportZip: boolean;
    }

    interface ITeamProject {
        projectId: string;
        name: string;
        number: string;
        notes: string;
        assignments: ITeamProjectAssignment[];
        canView: boolean;
        canAdd: boolean;
        canEdit: boolean;
        canDelete: boolean;
        canExportPdf: boolean;
        canExportZip: boolean;
    }

    interface IOrganisationTeamManageController {
        listType: string;
        activate: () => void;
    }

    class OrganisationTeamManageController implements IOrganisationTeamManageController {
        listType: string = '2';

        static $inject: string[] = ["$scope", "$state", "$stateParams", "$q", "orgTeamResource", "projectResource"];
        constructor(
            private $scope: IOrganisationTeamManageControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $q: ng.IQService,
            private orgTeamResource: Resources.IOrgTeamResource,
            private projectResource: Resources.IProjectResource
        ) {

            $scope.title = "Organisation Teams";

            this.activate();
        }

        activate() {
            var teamId = this.$stateParams['id'];

            var teamPromise = this.orgTeamResource.get({ id: teamId }).$promise;
            teamPromise.then((team) => { this.$scope.team = team; });

            var projectsPromise = this.orgTeamResource.getProjects({ id: teamId }, (projects) => {
                this.$scope.projects = projects;
                this.$scope.teamProjects = [];

                _.forEach(projects, (p: Models.IProject) => {
                    this.$scope.teamProjects.push(<ITeamProject>{
                        projectId: p.id,
                        name: p.name,
                        number: p.number,
                        notes: p.notes,
                        assignments: []
                    });
                });
            }).$promise;

            this.$q.all([teamPromise, projectsPromise]).then(() => {
                var projects = _.map(this.$scope.projects, (project) => { return project.id; });
                var users = _.map(this.$scope.team.users, (user) => { return user.orgUser.id; });

                var params = {
                    id: teamId,
                    projects: projects,
                    orgUsers: users
                };

                this.orgTeamResource.assignments(params, (assignments: Models.IProjectAssignment[]) => {
                    //this.$scope.userAssignments = [];
                    this.$scope.userAssignments = assignments;

                    _.forEach(this.$scope.teamProjects, (tp) => {
                        var projectAssignments = _.filter(assignments, (a) => { return a.projectId == tp.projectId; });

                        _.forEach(this.$scope.team.users, (user) => {
                            var userAssignment = _.filter(projectAssignments, (pa) => { return pa.orgUserId == user.orgUser.id; });
                            if (userAssignment.length) {
                                var record: ITeamProjectAssignment = {
                                    userId: user.orgUser.id,
                                    name: `${user.orgUser.firstName} ${user.orgUser.surname}`,
                                    email: user.orgUser.email,
                                    accountType: user.orgUser.accountType,
                                    isRootUser: user.orgUser.isRootUser,
                                    isWebUser: user.orgUser.isWebUser,
                                    isMobileUser: user.orgUser.isMobileUser,
                                    isManager: user.isManager,
                                    projectId: tp.projectId,
                                    currentProjectId: user.orgUser.currentProjectId,
                                    canAdd: userAssignment ? userAssignment[0].canAdd : false,
                                    canEdit: userAssignment ? userAssignment[0].canEdit : false,
                                    canView: userAssignment ? userAssignment[0].canView : false,
                                    canDelete: userAssignment ? userAssignment[0].canDelete : false,
                                    canExportPdf: userAssignment ? userAssignment[0].canExportPdf : false,
                                    canExportZip: userAssignment ? userAssignment[0].canExportZip : false
                                };
                                tp.assignments.push(record);
                            } else {
                                tp.assignments.push(<ITeamProjectAssignment>{
                                    userId: user.orgUser.id,
                                    name: `${user.orgUser.firstName} ${user.orgUser.surname}`,
                                    email: user.orgUser.email,
                                    accountType: user.orgUser.accountType,
                                    isRootUser: user.orgUser.isRootUser,
                                    isWebUser: user.orgUser.isWebUser,
                                    isMobileUser: user.orgUser.isMobileUser,
                                    isManager: user.isManager,
                                    projectId: tp.projectId,
                                    currentProjectId: user.orgUser.currentProjectId,
                                    canAdd: false,
                                    canEdit: false,
                                    canView: false,
                                    canDelete: false,
                                    canExportPdf: false,
                                    canExportZip: false
                                });
                            }
                        });
                    });
                });
            });
        }

        updateAssignment(assignment: ITeamProjectAssignment, accessLevel: string) {
            var params = {
                id: assignment.projectId,
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

        refreshAssignment(assignment: ITeamProjectAssignment, newValue: Models.IProjectAssignment) {
            assignment.canView = newValue.canView;
            assignment.canAdd = newValue.canAdd;
            assignment.canEdit = newValue.canEdit;
            assignment.canDelete = newValue.canDelete;
            assignment.canExportPdf = newValue.canExportPdf;
            assignment.canExportZip = newValue.canExportZip;
        }

        updateListType(value: string) {
            var accountType = +value;

            _.forEach(this.$scope.teamProjects, (tp) => {
                tp.assignments = [];
                var projectAssignments = _.filter(this.$scope.userAssignments, (a) => { return a.projectId == tp.projectId; });

                var teamUsers = [];
                if (accountType < 2) {
                    teamUsers = _.filter(this.$scope.team.users, (u) => {
                        return u.orgUser.accountType === accountType;
                    });
                } else {
                    teamUsers = this.$scope.team.users;
                }
                
                _.forEach(teamUsers, (user) => {
                    var userAssignment = _.filter(projectAssignments, (pa) => { return pa.orgUserId == user.orgUser.id; });
                    if (userAssignment.length) {
                        var record: ITeamProjectAssignment = {
                            userId: user.orgUser.id,
                            name: `${user.orgUser.firstName} ${user.orgUser.surname}`,
                            email: user.orgUser.email,
                            accountType: user.orgUser.accountType,
                            isRootUser: user.orgUser.isRootUser,
                            isWebUser: user.orgUser.isWebUser,
                            isMobileUser: user.orgUser.isMobileUser,
                            isManager: user.isManager,
                            projectId: tp.projectId,
                            currentProjectId: user.orgUser.currentProjectId,
                            canAdd: userAssignment ? userAssignment[0].canAdd : false,
                            canEdit: userAssignment ? userAssignment[0].canEdit : false,
                            canView: userAssignment ? userAssignment[0].canView : false,
                            canDelete: userAssignment ? userAssignment[0].canDelete : false,
                            canExportPdf: userAssignment ? userAssignment[0].canExportPdf : false,
                            canExportZip: userAssignment ? userAssignment[0].canExportZip : false
                        };
                        tp.assignments.push(record);
                    } else {
                        tp.assignments.push(<ITeamProjectAssignment>{
                            userId: user.orgUser.id,
                            name: `${user.orgUser.firstName} ${user.orgUser.surname}`,
                            email: user.orgUser.email,
                            accountType: user.orgUser.accountType,
                            isRootUser: user.orgUser.isRootUser,
                            isWebUser: user.orgUser.isWebUser,
                            isMobileUser: user.orgUser.isMobileUser,
                            projectId: tp.projectId,
                            currentProjectId: user.orgUser.currentProjectId,
                            canAdd: false,
                            canEdit: false,
                            canView: false,
                            canDelete: false,
                            canExportPdf: false,
                            canExportZip: false
                        });
                    }
                });
            });
        }

    }

    angular.module("app").controller("organisationTeamManageController", OrganisationTeamManageController);
}