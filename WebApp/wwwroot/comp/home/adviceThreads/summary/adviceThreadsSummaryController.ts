module App {
    "use strict";

    interface IAdviceThreadsSummaryController {
        title: string;
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];

        activate: () => void;
        getAttachmentsCount: (survey: Models.ISurvey) => number;
        currentUserId: string;
    }

    class AdviceThreadsSummaryController implements IAdviceThreadsSummaryController {
        title: string = "Surveys";
        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];
        currentUserId: string;

        static $inject: string[] = ['$scope', '$stateParams', 'toastr', 'formTemplateResource', 'surveyResource', 'projectSummaryPrintSessionResource', 'userContextService', 'project'];
        constructor(
            public $scope: ng.IScope,
            public $stateParams: ng.ui.IStateParamsService,
            public toastr: any,
            private formTemplateResource: Resources.IFormTemplateResource,
            private surveyResource: Resources.ISurveyResource,
            private projectSummaryPrintSessionResource: Resources.IProjectSummaryPrintSessionResource,
            private userContextService: Services.IUserContextService,
            public project: Models.IProject) {

            this.activate();
        }

        activate() {
            this.currentUserId = this.userContextService.current.user.id;
            this.load();
        }

        load() {
            var projectId = null;
            if (this.project != null) {
                projectId = this.project.id;
            }

            this.formTemplateResource.query({ discriminator: 1, projectId: projectId }).$promise
                .then((templates) => {
                    this.formTemplates = templates;
                }, (err) => {
                    this.toastr.error(err.data);
                    console.log(err)
                });

            this.surveyResource.query({ discriminator: 1, projectId: projectId }).$promise
                .then((surveys) => {
                    this.surveys = surveys;
                }, (err) => {
                    this.toastr.error(err.data);
                    console.error(err);
                });
        }

        delete(id: string) {
            this.surveyResource.delete({ id: id },
                () => { this.load(); },
                (err) => {
                    console.error(err);
                    this.toastr.error(err.data);
                });
        }

        getDescriptionHeading(id: string) {
            var template = _.filter(this.formTemplates, (t) => { return t.id === id; });
            if (template.length) {
                var metricTitles = [];
                let descFormat = template[0].descriptionFormat;
                var pattern = /{{\s*([^}]+)\s*}}/g;
                var segment;

                while (segment = pattern.exec(descFormat))
                    metricTitles.push(segment[1]);

                return metricTitles.join(' - ');
            }

            return "Your record";
        }

        getAttachmentsCount(survey: Models.ISurvey) {
            var attachmentCount = 0;
            _.forEach(survey.formValues, (fv) => {
                if (fv.attachments.length > 0)
                    attachmentCount += fv.attachments.length;
            });

            return attachmentCount;
        }

        hasAccess(template: Models.IFormTemplate, flag: string): boolean {
            var authorized = false;

            switch (flag) {
                case 'view': {
                    authorized = template.canView === null ? this.project.allowView : template.canView;
                    break;
                }
                case 'add': {
                    authorized = template.canAdd === null ? this.project.allowAdd : template.canAdd;
                    break;
                }
                case 'edit': {
                    authorized = template.canEdit === null ? this.project.allowEdit : template.canEdit;
                    break;
                }
                case 'delete': {
                    authorized = template.canDelete === null ? this.project.allowDelete : template.canDelete;
                    break;
                }
            }

            return authorized;
        }

        isProjectOwner(survey: Models.ISurvey) {
            var templates = _.filter(this.formTemplates, (t) => { return t.id == survey.formTemplateId; });
            var createdBy = templates[0].project.createdBy;

            return survey.filledById === createdBy.id;
        }
    }

    angular.module("app").controller("adviceThreadsSummaryController", AdviceThreadsSummaryController);
}