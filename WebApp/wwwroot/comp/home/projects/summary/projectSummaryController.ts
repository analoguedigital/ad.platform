declare var moment: any;
declare var Chart: any;

module App {
    "use strict";

    interface IProjectSummaryControllerScope extends ng.IScope {
        safeSurveys: Models.ISurvey[];
        displayedSurveys: Models.ISurvey[];
        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        today: Date;
        months: string[];

        filterValues: Models.IFilterValue[];

        summaryStateLabel: string;
    }

    interface IProjectSummaryController {
        discriminator: number;

        searchTerm: string;
        startDate: Date;
        endDate: Date;
        startDateCalendar: any;
        endDateCalendar: any;
        locationCount: number;

        formTemplates: Models.IFormTemplate[];
        surveys: Models.ISurvey[];

        displayedSurveys: Models.ISurvey[];
        selectedTemplates: Models.IFormTemplate[];

        timelineSnapshotView: boolean;
        metricFilters: Models.IMetricFilter[];

        activate: () => void;
        clearAll: () => void;
        print: () => void;
        selectAll: () => void;
        selectNone: () => void;
        openStartDateCalendar: () => void;
        openEndDateCalendar: () => void;
        search: () => void;
        delete: (id: string) => void;
        getAttachmentsCount: (survey: Models.ISurvey) => number;
    }

    class ProjectSummaryController implements IProjectSummaryController {
        discriminator: number;

        searchTerm: string;
        startDate: Date;
        endDate: Date;
        startDateCalendar = { isOpen: false };
        endDateCalendar = { isOpen: false };
        locationCount: number;

        formTemplates: Models.IFormTemplate[] = [];
        surveys: Models.ISurvey[] = [];
        displayedSurveys: Models.ISurvey[] = [];
        selectedTemplates: Models.IFormTemplate[] = [];
        timelineSnapshotView: boolean = true;
        metricFilters: Models.IMetricFilter[];

        mapCenter: any;
        mapZoomLevel: number;
        mapType: string = 'roadmap';

        static $inject: string[] = ["$scope", "$rootScope", "$state", "$q", "$stateParams",
            "projectSummaryPrintSessionResource", "projectResource", "$timeout", "$uibModal",
            "formTemplateResource", "surveyResource", "project", "toastr", "uiTourService", "localStorageService"];

        constructor(
            private $scope: IProjectSummaryControllerScope,
            private $rootScope: ng.IRootScopeService,
            private $state: ng.ui.IStateService,
            private $q: ng.IQService,
            private $stateParams: ng.ui.IStateParamsService,
            private projectSummaryPrintSessionResource: Resources.IProjectSummaryPrintSessionResource,
            private projectResource: Resources.IProjectResource,
            private $timeout: ng.ITimeoutService,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private formTemplateResource: Resources.IFormTemplateResource,
            private surveyResource: Resources.ISurveyResource,
            public project: Models.IProject,
            private toastr: any,
            private uiTourService: any,
            private localStorageService: ng.local.storage.ILocalStorageService) {

            this.activate();
        }

        activate() {
            this.$scope.title = this.project.name;
            this.$scope.today = new Date();
            this.$scope.months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
            //this.startDate = moment().add('-1', 'month').toDate();

            this.metricFilters = [];
            this.$scope.filterValues = [];

            this.$rootScope.$on('timeline-in-snapshot-view', () => {
                this.timelineSnapshotView = true;
            });

            this.$rootScope.$on('timeline-in-month-view', () => {
                this.timelineSnapshotView = false;
            });

            this.$scope.onTourStart = () => { this.onTourStart(); };

            var snapshotToggle = this.localStorageService.get('timeline_snapshot_view');
            if (snapshotToggle !== null) {
                this.timelineSnapshotView = <boolean>snapshotToggle;
            }

            if (this.$stateParams.discriminator) {
                this.discriminator = this.$stateParams.discriminator;
                this.$scope.summaryStateLabel = this.$stateParams.discriminator === '0' ? 'Records Summary' : 'Advice Summary';
            }
            else {
                this.discriminator = 0;
                this.$scope.summaryStateLabel = 'Records Summary';
            }

            this.load();
        }

        load() {
            this.$q.all([
                this.formTemplateResource.query({ discriminator: this.discriminator, projectId: this.project.id }).$promise,
                this.surveyResource.query({ discriminator: this.discriminator, projectId: this.project.id }).$promise
            ]).then((data) => {
                this.formTemplates = data[0];
                this.surveys = data[1];
                this.$scope.safeSurveys = this.surveys;

                _.map(this.formTemplates, (t) => { t.isChecked = true });

                this.formTemplates = _.filter(this.formTemplates, (t) => {
                    return t.projectId == this.project.id;
                });

                var templateIds = _.map(this.formTemplates, (template) => { return template.id; });
                this.getMetricFilters(templateIds);

                // populate data sets
                this.displayedSurveys = this.surveys;
                this.selectedTemplates = this.formTemplates;
                this.$scope.displayedSurveys = this.displayedSurveys;

                this.$timeout(() => {
                    this.$rootScope.$broadcast('update_locations_map_bounds');
                }, 500);

                var currentDate = this.localStorageService.get('timeline-current-date');
                if (currentDate !== null) {
                    this.$scope.today = moment(<Date>currentDate).toDate();
                    this.$rootScope.$broadcast('timeline-change-date', this.$scope.today);
                }

                //this.uiTourService.getTour().start();
            });
        }

        findMetricFilter(metricFilter: Models.IMetricFilter, filters: Models.IMetricFilter[]) {
            var result = undefined;
            var count = 0;

            _.forEach(filters, (item) => {
                if (item.shortTitle === metricFilter.shortTitle && item.type === metricFilter.type) {
                    result = item;
                    count++;
                }
            });

            if (result && count > 1)
                return result;

            return undefined;
        }

        clearFilterValues() {
            _.forEach(this.$scope.filterValues, (fv) => {
                switch (fv.type) {
                    case "single": {
                        var singleValue = <Models.ISingleFilterValue>fv;
                        singleValue.value = undefined;
                        break;
                    }
                    case "range": {
                        var rangeValue = <Models.IRangeFilterValue>fv;
                        rangeValue.fromValue = undefined;
                        rangeValue.toValue = undefined;
                        break;
                    }
                    case "multiple": {
                        var multipleValue = <Models.IMultipleFilterValue>fv;
                        multipleValue.values = [];
                        _.forEach(multipleValue.options, (opt) => { opt.selected = false });
                        break;
                    }
                }
            });
        }

        clearAll() {
            this.searchTerm = undefined;
            this.startDate = undefined;
            this.endDate = undefined;

            this.clearFilterValues();
            this.load();
        }

        print() {
            let surveys = _.filter(this.surveys, (r) => { return r.isChecked });
            if (surveys.length < 1) {
                this.toastr.info('Select desired records first', 'Print Surveys');
                return false;
            }

            let printSession = <Models.IProjectSummaryPrintSession>{};
            printSession.projectId = this.project.id;
            printSession.surveyIds = _.map(surveys, (survey) => { return survey.id; });

            this.projectSummaryPrintSessionResource.save(printSession).$promise.then((session) => {
                this.$state.go("home.projects.summaryPrint", {
                    sessionId: session.id,
                    mapCenter: this.mapCenter,
                    mapZoomLevel: this.mapZoomLevel,
                    mapType: this.mapType
                });
            });
        }

        selectAll() {
            _.forEach(this.displayedSurveys, (survey) => {
                survey.isChecked = true;
            });
        }

        selectAllRecords() {
            _.forEach(this.surveys, (survey) => {
                survey.isChecked = true;
            });
        }

        selectAllWithAttachments() {
            var surveys = _.filter(this.surveys, (survey) => {
                var formValues = _.filter(survey.formValues, (fv) => {
                    if (fv.attachments.length > 0) return fv;
                });

                if (formValues.length) return survey;
            });

            if (surveys.length) {
                _.forEach(this.surveys, (survey) => {
                    survey.isChecked = false;
                });

                _.forEach(surveys, (s) => {
                    s.isChecked = true;
                });

                this.surveys = surveys;
                this.displayedSurveys = surveys;
            }
        }

        selectNone() {
            _.forEach(this.displayedSurveys, (survey) => {
                survey.isChecked = false;
            });
        }

        clearAllRecords() {
            _.forEach(this.surveys, (survey) => {
                survey.isChecked = false;
            });
        }

        getFormTemplate(id: string) {
            let template = _.find(this.formTemplates, (t) => { return t.id == id; });
            if (template && template.title)
                return template.title;

            return '';
        }

        getTemplateColour(id: string) {
            let template = _.find(this.formTemplates, (t) => { return t.id == id; });
            if (template && template.colour && template.colour.length)
                return template.colour;

            return '';
        }

        getThreadsCount() {
            //let templates = _.uniq(_.map(this.displayedSurveys, (r) => { return r.formTemplateId }));
            //return templates.length;

            let templates = _.uniq(_.map(this.surveys, (r) => { return r.formTemplateId }));

            var counter = 0;
            _.forEach(templates, (tid) => {
                var res = _.filter(this.formTemplates, (t) => { return t.id == tid; });
                if (res.length) {
                    counter++;
                }
            });

            return counter;
        }

        openStartDateCalendar() {
            this.startDateCalendar.isOpen = true;
        }

        openEndDateCalendar() {
            this.endDateCalendar.isOpen = true;
        }

        timelineNextMonth() {
            this.$scope.today = moment(this.$scope.today).add(1, 'months').toDate();
            this.$rootScope.$broadcast('timeline-next-month');
            this.localStorageService.set('timeline-current-date', this.$scope.today);
        }

        timelinePreviousMonth() {
            this.$scope.today = moment(this.$scope.today).subtract(1, 'months').toDate();
            this.$rootScope.$broadcast('timeline-previous-month');
            this.localStorageService.set('timeline-current-date', this.$scope.today);
        }

        delete(id: string) {
            this.surveyResource.delete({ id: id },
                () => { this.load(); },
                (err) => { console.error(err); });
        }

        getFilterValues() {
            var filterValues = [];

            _.forEach(this.$scope.filterValues, (filterValue) => {
                switch (filterValue.type) {
                    case "single": {
                        var singleValue = <Models.ISingleFilterValue>filterValue;
                        if (singleValue.value && singleValue.value.length)
                            filterValues.push(filterValue);

                        break;
                    }
                    case "range": {
                        var rangeValue = <Models.IRangeFilterValue>filterValue;
                        var fromValue = rangeValue.fromValue;
                        var toValue = rangeValue.toValue;

                        if (fromValue || toValue)
                            filterValues.push(filterValue);

                        break;
                    }
                    case "multiple": {
                        var multipleValue = <Models.IMultipleFilterValue>filterValue;
                        if (multipleValue.values && multipleValue.values.length)
                            filterValues.push(multipleValue);

                        break;
                    }
                }
            });

            return filterValues;
        }

        getMetricFilters(templateIds: string[]) {
            let filterPromises: ng.IPromise<any>[] = [];
            _.forEach(templateIds, (id) => {
                filterPromises.push(this.formTemplateResource.getFilters({ id: id }, (res) => { }).$promise);
            });

            this.$q.all(filterPromises)
                .then((metricFilters) => {
                    var filters: Models.IMetricFilter[] = [];
                    _.forEach(metricFilters, (mf) => {
                        filters = filters.concat(mf);
                    });

                    if (templateIds.length == 1) {
                        this.metricFilters = filters;
                    } else {
                        var matchedFilters = [];
                        _.forEach(filters, (f) => {
                            var found = this.findMetricFilter(f, filters);
                            if (found) matchedFilters.push(found);
                        });

                        this.metricFilters = _.uniqBy(matchedFilters, 'shortTitle');
                    }
                });
        }

        search() {
            var filterValues = this.getFilterValues();
            var templateIds = _.map(_.filter(this.formTemplates, (template) => { return template.isChecked == true }), (template) => { return template.id });

            var searchModel: Models.SearchDTO = {
                projectId: this.project.id,
                formTemplateIds: templateIds,
                term: this.searchTerm,
                startDate: this.startDate,
                endDate: this.endDate,
                filterValues: filterValues
            };

            this.surveyResource.search(searchModel, (surveys: Models.ISurvey[]) => {
                this.surveys = surveys;
                this.displayedSurveys = [].concat(this.surveys);
                this.$scope.displayedSurveys = this.displayedSurveys;

                if (templateIds.length < 1)
                    this.selectedTemplates = [];
                else
                    this.getMetricFilters(templateIds); // reload advanced search UI

                this.$timeout(() => {
                    this.$rootScope.$broadcast('update_locations_map_bounds');
                }, 500);
            }, (error) => {
                console.error(error);
            });
        }

        getDescriptionHeading() {
            var templateIds = _.uniq(_.map(this.displayedSurveys, (s) => { return s.formTemplateId; }));
            var templates = _.filter(this.formTemplates, (t) => {
                return _.includes(templateIds, t.id);
            });

            if (templates.length == 1) {
                var metricTitles = [];
                let descFormat = templates[0].descriptionFormat;
                var pattern = /{{\s*([^}]+)\s*}}/g;
                var segment;

                while (segment = pattern.exec(descFormat))
                    metricTitles.push(segment[1]);

                return metricTitles.join(' - ');
            }

            return "Record";
        }

        getAttachmentsCount(survey: Models.ISurvey) {
            var attachmentCount = 0;
            _.forEach(survey.formValues, (fv) => {
                if (fv.attachments.length > 0)
                    attachmentCount += fv.attachments.length;
            });

            return attachmentCount;
        }

        hasAccess(templateId: string, flag: string): boolean {
            var template = _.filter(this.formTemplates, (t) => { return t.id === templateId; })[0];

            var authorized = false;
            if (template) {
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
            } else {
                authorized = true;
            }

            return authorized;
        }

        onTourStart() {
            var tour = this.uiTourService.getTour();

            tour.createStep({
                selector: '.timeline-box',
                order: 1,
                fixed: true,
                position: 'bottom',
                title: 'Timeline',
                content: 'A smart overview of your activities'
            });

            tour.createStep({
                selector: '.locations-box',
                order: 2,
                fixed: true,
                position: 'right',
                title: 'Locations',
                content: 'Find your records on the map'
            });

            tour.createStep({
                selector: '.pie-chart-box',
                order: 3,
                fixed: true,
                position: 'left',
                title: 'Threads',
                content: 'Record distribution across threads'
            });

            tour.createStep({
                selector: '.recordings-box',
                order: 4,
                position: 'top',
                fixed: false,
                scrollIntoView: true,
                title: 'Recordings',
                content: 'All of your records are listed here'
            });

            tour.createStep({
                selector: '.search-box',
                order: 5,
                fixed: false,
                position: 'left',
                scrollIntoView: true,
                title: 'Search & filter',
                content: 'You can search and filter your data here'
            });

            tour.createStep({
                selector: '#keyword-search',
                order: 6,
                position: 'bottom-left',
                title: 'Keyword Search',
                content: 'Find key words or numbers, including records by serial numbers'
            });

            tour.createStep({
                selector: '#date-range-search',
                order: 7,
                position: 'left',
                title: 'Date Range',
                content: 'Filter records by the date they were uploaded'
            });

            tour.createStep({
                selector: '#threads-container',
                order: 8,
                position: 'left',
                title: 'Select Threads',
                content: 'Select the threads you want and click the search button to filter records'
            });
        }

        toggleTimelineView(toggle: boolean) {
            this.timelineSnapshotView = toggle;
            this.localStorageService.set('timeline_snapshot_view', toggle);
        }

        changeTimelineDate() {
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/projects/summary/timeline-datepicker/timeline-datepicker-view.html',
                controller: 'timelineDatepickerController',
                controllerAs: 'ctrl',
                resolve: {
                    currentTimelineDate: () => {
                        return this.$scope.today;
                    }
                }
            }).result.then(
                (res) => {
                    var month = res.month;
                    var year = res.year;

                    this.timelineSnapshotView = false;
                    this.$scope.today = moment(new Date(year, month, 1)).toDate();;
                    this.$rootScope.$broadcast('timeline-change-date', this.$scope.today);
                    this.localStorageService.set('timeline-current-date', this.$scope.today);
                },
                (err) => {
                    //console.error(err);
                });
        }

        resetTimelineDate() {
            this.$scope.today = moment(new Date()).toDate();
            this.$rootScope.$broadcast('timeline-change-date', this.$scope.today);
            this.localStorageService.set('timeline-current-date', this.$scope.today);
        }

        openSlideshow() {
            let selectedSurveys = _.filter(this.surveys, (r) => { return r.isChecked });
            if (selectedSurveys.length < 1) {
                this.toastr.info('Select desired records first', 'Slideshow');
                return false;
            }

            var modalInstance = this.$uibModal.open({
                size: 'lg',
                animation: true,
                templateUrl: 'comp/home/projects/summary/slideshow/slideshow-view.html',
                controller: 'slideshowController',
                controllerAs: 'ctrl',
                resolve: {
                    surveys: () => {
                        return selectedSurveys;
                    },
                    formTemplates: () => {
                        var threads = [];

                        _.forEach(selectedSurveys, (survey) => {
                            var thread = _.find(this.formTemplates, (t) => { return t.id == survey.formTemplateId; });
                            threads.push(thread);
                        });

                        return threads;
                    }
                }
            }).result.then(
                (res) => {
                    console.log(res);
                },
                (err) => {
                    //console.error(err);
                });
        }

    }

    angular.module("app").controller("projectSummaryController", ProjectSummaryController);

}   