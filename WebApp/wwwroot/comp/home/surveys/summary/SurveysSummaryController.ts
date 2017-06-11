
module App {
    "use strict";

    interface ISurveysSummaryController {
        title: string;
        project: Models.IProject;
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        printSelected: () => void;
        activate: () => void;
    }

    class SurveysSummaryController implements ISurveysSummaryController {
        title: string = "Surveys";
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];

        static $inject: string[] = ['$state', 'project', 'formTemplateResource', 'surveyResource'];

        constructor(
            public $state: ng.ui.IStateService,
            public project: Models.IProject,
            private formTemplateResource: Resources.IFormTemplateResource,
            private surveyResource: Resources.ISurveyResource) {
          
            this.activate();
        }

        activate() {
            this.load();
        }

        load() {

            if (this.project == null)
                return;

            this.formTemplates = this.formTemplateResource.query({ projectId: this.project.id });
            this.surveys = this.surveyResource.query({ projectId: this.project.id });
        }


        delete(id: string) {
            this.surveyResource.delete({ id: id },
                () => { this.load(); },
                (err) => { console.log(err); });
        }

        printSelected() {
            let selectedSurveys = this.surveys.filter((survey) => survey.isChecked == true);
            let result = selectedSurveys.map((survey) => { return survey.id; });

            this.$state.go('home.surveys.print-multiple', { selectedSurveys: result });
        }
        
    }

    angular.module("app").controller("surveysSummaryController", SurveysSummaryController);
}