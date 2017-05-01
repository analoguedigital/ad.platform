
module App {
    "use strict";

    interface ISurveysSummaryController {
        title: string;
        project: Models.IProject;
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        activate: () => void;
    }

    class SurveysSummaryController implements ISurveysSummaryController {
        title: string = "Surveys";
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];

        static $inject: string[] = ['project', 'formTemplateResource', 'surveyResource'];

        constructor(
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
    }

    angular.module("app").controller("surveysSummaryController", SurveysSummaryController);
}