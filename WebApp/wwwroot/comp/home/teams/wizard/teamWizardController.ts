module App {
    "use strict";

    interface ITeamWizardControllerScope extends ng.IScope {
        title: string;
        team: Models.IOrgTeam;
        projects: Models.IProject[];

        clients: Models.IProject[];
        displayedClients: Models.IProject[];
        clientSearchTerm: string;

        staffMembers: Models.IOrgTeamUser[];
        displayedStaffMembers: Models.IOrgTeamUser[];
        staffSearchTerm: string;

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

    interface ITeamWizardController {
        activate: () => void;
    }

    class TeamWizardController implements ITeamWizardController {
        static $inject: string[] = ["$scope", "$state", "$stateParams", "$q",
            "orgTeamResource", "projectResource", "toastr", "WizardHandler"];
        constructor(
            private $scope: ITeamWizardControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $q: ng.IQService,
            private orgTeamResource: Resources.IOrgTeamResource,
            private projectResource: Resources.IProjectResource,
            private toastr: any,
            private WizardHandler: any
        ) {

            $scope.title = "Organisation Teams";
            $scope.clients = [];

            this.activate();
        }

        activate() {
            this.$scope.$on('wizard:stepChanged', (event, args) => this.wizardStepChanged(args));

            var teamId = this.$stateParams['id'];
            var teamPromise = this.orgTeamResource.get({ id: teamId }).$promise;
            teamPromise.then((team) => { this.$scope.team = team; });

            var projectsPromise = this.orgTeamResource.getProjects({ id: teamId }, (projects: Models.IProject[]) => {
                this.$scope.projects = projects;
                this.$scope.teamProjects = [];
            }).$promise;

            //var projectsPromise = this.projectResource.query((projects) => {
            //    this.$scope.projects = projects;
            //    this.$scope.teamProjects = [];
            //}).$promise;

            this.$q.all([teamPromise, projectsPromise]).then(() => {
                this.$scope.clients = _.filter(this.$scope.projects, (p) => { return p.createdBy !== null && p.createdBy.accountType === 0; });
                this.$scope.displayedClients = [].concat(this.$scope.clients);

                this.$scope.staffMembers = _.filter(this.$scope.team.users, (tu) => { return tu.orgUser.accountType === 1; });
                this.$scope.displayedStaffMembers = [].concat(this.$scope.staffMembers);
            });
        }

        updateAssignment(assignment: ITeamProjectAssignment, accessLevel: string) {
            var toggled = false;
            switch (accessLevel) {
                case 'allowAdd': {
                    toggled = assignment.canAdd;
                    if (toggled) assignment.canView = true;
                    break;
                }
                case 'allowEdit': {
                    toggled = assignment.canEdit;
                    if (toggled) assignment.canView = true;
                    break;
                }
                case 'allowDelete': {
                    toggled = assignment.canDelete;
                    if (toggled) assignment.canView = true;
                    break;
                }
                case 'allowView': {
                    toggled = assignment.canView;
                    break;
                }
                case 'allowExportPdf': {
                    toggled = assignment.canExportPdf;
                    if (toggled) assignment.canView = true;
                    break;
                }
                case 'allowExportZip': {
                    toggled = assignment.canExportZip;
                    if (toggled) assignment.canView = true;
                    break;
                }
            }
        }

        wizardStepChanged(args: any) {
            if (args.index === 2) {
                // step changes to Permissions (step 3)
                this.$scope.teamProjects = [];
                var selectedProjects = _.filter(this.$scope.clients, (c) => { return c.isSelected === true; });

                _.forEach(selectedProjects, (p: Models.IProject) => {
                    this.$scope.teamProjects.push(<ITeamProject>{
                        projectId: p.id,
                        name: p.name,
                        number: p.number,
                        notes: p.notes,
                        assignments: []
                    });
                });

                var selectedStaff = _.filter(this.$scope.staffMembers, (s) => { return s.isSelected === true; });

                var teamId = this.$stateParams['id'];
                var projects = _.map(selectedProjects, (p) => { return p.id; });
                var users = _.map(selectedStaff, (u) => { return u.orgUser.id; });

                var params = {
                    id: teamId,
                    projects: projects,
                    orgUsers: users
                };

                this.orgTeamResource.assignments(params, (assignments: Models.IProjectAssignment[]) => {
                    this.$scope.userAssignments = assignments;

                    _.forEach(this.$scope.teamProjects, (tp) => {
                        var projectAssignments = _.filter(assignments, (a) => { return a.projectId == tp.projectId; });

                        _.forEach(selectedStaff, (user) => {
                            var userAssignment = _.filter(projectAssignments, (pa) => { return pa.orgUserId == user.orgUser.id; });
                            if (userAssignment.length) {
                                var record = this.generateAssignment(user, tp, userAssignment);
                                tp.assignments.push(record);
                            } else {
                                var record = this.generateEmptyAssignment(user, tp);
                                tp.assignments.push(record);
                            }
                        });
                    });
                });
            }
        }

        generateAssignment(user: Models.IOrgTeamUser, project: ITeamProject, assignment: Models.IProjectAssignment[]) {
            var record: ITeamProjectAssignment = {
                userId: user.orgUser.id,
                name: `${user.orgUser.firstName} ${user.orgUser.surname}`,
                email: user.orgUser.email,
                accountType: user.orgUser.accountType,
                isRootUser: user.orgUser.isRootUser,
                isWebUser: user.orgUser.isWebUser,
                isMobileUser: user.orgUser.isMobileUser,
                isManager: user.isManager,
                projectId: project.projectId,
                currentProjectId: user.orgUser.currentProjectId,
                canAdd: assignment ? assignment[0].canAdd : false,
                canEdit: assignment ? assignment[0].canEdit : false,
                canView: assignment ? assignment[0].canView : false,
                canDelete: assignment ? assignment[0].canDelete : false,
                canExportPdf: assignment ? assignment[0].canExportPdf : false,
                canExportZip: assignment ? assignment[0].canExportZip : false
            };

            return record;
        }

        generateEmptyAssignment(user: Models.IOrgTeamUser, project: ITeamProject) {
            var record = <ITeamProjectAssignment>{
                userId: user.orgUser.id,
                name: `${user.orgUser.firstName} ${user.orgUser.surname}`,
                email: user.orgUser.email,
                accountType: user.orgUser.accountType,
                isRootUser: user.orgUser.isRootUser,
                isWebUser: user.orgUser.isWebUser,
                isMobileUser: user.orgUser.isMobileUser,
                isManager: user.isManager,
                projectId: project.projectId,
                currentProjectId: user.orgUser.currentProjectId,
                canAdd: false,
                canEdit: false,
                canView: false,
                canDelete: false,
                canExportPdf: false,
                canExportZip: false
            };

            return record;
        }

        validateClients() {
            var selectedClients = _.filter(this.$scope.clients, (c) => { return c.isSelected === true; });
            return selectedClients.length > 0;
        }
        
        validateStaff() {
            var selectedStaff = _.filter(this.$scope.staffMembers, (s) => { return s.isSelected === true; });
            return selectedStaff.length > 0;
        }

        finishWizard() {
            var payload = [];

            _.forEach(this.$scope.teamProjects, (tp) => {
                var record:any = {};
                record.projectId = tp.projectId;
                record.projectName = tp.name;
                record.assignments = [];

                _.forEach(tp.assignments, (a) => {
                    record.assignments.push({
                        userId: a.userId,
                        userName: a.name,
                        canView: a.canView,
                        canAdd: a.canAdd,
                        canEdit: a.canEdit,
                        canDelete: a.canDelete,
                        canExportPdf: a.canExportPdf,
                        canExportZip: a.canExportZip
                    });
                });

                payload.push(record);
            });

            var params = {
                id: this.$state.params['id'],
                permissions: payload
            };

            this.orgTeamResource.updatePermissions(params, (res) => {
                this.toastr.success('Permissions configured successfully');
                this.WizardHandler.wizard('teamWizard').reset();
            }, (err) => {
                console.error(err);
            });
        }

        cancelWizard() {
            this.toastr.info('Wizard cancelled!');
        }

    }

    angular.module("app").controller("teamWizardController", TeamWizardController);
}