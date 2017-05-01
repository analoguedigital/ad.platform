
module App {
    "use strict";

    interface ISurveysController {
        title: string;
        projects: Models.IProject[];
        selectedProject: Models.IProject;
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        activate: () => void;
    }

    export interface IProjectStateParamsService extends ng.ui.IStateParamsService {
        project: Models.IProject;
    }

    class SurveysController implements ISurveysController {
        public selectedProject: Models.IProject;
        title: string = "Forms";
        formTemplates: Models.IFormTemplate[];
        projects: Models.IProject[];
        surveys: Models.ISurvey[];

        static $inject: string[] = ['$stateParams', '$state', 'projectResource', 'formTemplateResource', 'surveyResource'];

        constructor(

            private $stateParams: IProjectStateParamsService,
            private $state: ng.ui.IStateService,
            private projectResource: Resources.IProjectResource,
            private formTemplateResource: Resources.IFormTemplateResource,
            private surveyResource: Resources.ISurveyResource) {

            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            this.projectResource.query().$promise.then((projects) => {
                this.projects = projects;
                if (this.projects.length > 0) {
                    if (this.$state.params['projectId'] !== undefined) {
                        this.selectedProject = _.find(this.projects, { id: this.$state.params['projectId'] });
                    }
                    else {
                        this.selectedProject = this.projects[0];
                        this.selectedProjectChanged();
                    }
                }
            });
        }

        selectedProjectChanged() {
            if (!this.selectedProject)
                return;

            if (this.$state.current.name === 'home.surveys.list.all') {
                this.$state.go("home.surveys.list.all", { projectId: this.selectedProject.id });
            }
            else {
                this.$state.go("home.surveys.list.summary", { projectId: this.selectedProject.id });
            }

        }

    }

    angular.module("app").controller("surveysController", SurveysController);
}