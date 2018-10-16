﻿
module App {
    "use strict";

    interface IDataListEditController {
        title: string;
        activate: () => void;
        dataList: Models.IDataList;
        errors: string;
        currentUserIsSuperUser: boolean;
        organisations: Models.IOrganisation[];

        submit: (form: ng.IFormController) => void;
        clearErrors: () => void;
    }

    class DataListEditController implements IDataListEditController {
        title: string = "Data list details";
        dataListId: string;
        dataList: Models.IDataList;
        references: Models.IDataListBasic[] = [];
        organisations: Models.IOrganisation[] = [];
        isAddMode: boolean;
        errors: string;
        currentUserIsSuperUser: boolean;

        metricsDraggableOptions = {
        };

        static $inject: string[] = ["dataListResource", "dataListRelationshipResource", "$state",
            "$stateParams", "$uibModal", "toastr", "userContextService", "organisationResource"];

        constructor(
            private dataListResource: Resources.IDataListResource,
            private dataListRelationshipResource: Resources.IDataListRelationshipResource,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private toastr: any,
            private userContextService: Services.IUserContextService,
            private organisationResource: Resources.IOrganisationResource
        ) {
            this.dataListId = $stateParams['id'];
            this.activate();
        }

        activate() {
            var roles = ["System administrator"];
            this.currentUserIsSuperUser = this.userContextService.userIsInAnyRoles(roles);

            if (this.dataListId === '') {
                this.dataListId = '00000000-0000-0000-0000-000000000000';
                this.isAddMode = true;
            }

            if (this.currentUserIsSuperUser) {
                this.organisationResource.query().$promise.then((organisations) => {
                    this.organisations = organisations;
                });
            }

            this.dataListResource.get({ id: this.dataListId })
                .$promise.then((dataList) => {
                    this.dataList = dataList;
                });

            this.dataListResource.getReferences({ id: this.dataListId }).$promise.then((references) => {
                this.references = references;
            });

        }

        getItem(dataList, dataListItemId) {
            if (!dataList) return {};
            return _.find(dataList.items, { 'id': dataListItemId });
        }

        getRelationValue(item: Models.IDataListItem, relationship: Models.IDataListRelationship) {
            var attr = _.filter(item.attributes, { relationshipId: relationship.id })[0];
            if (!attr) {
                attr = <Models.IDataListItemAttr>{ relationshipId: relationship.id };
                item.attributes.push(attr);
            }

            return this.getItem(relationship.dataList, attr.valueId)
        }

        openEditItem(item: Models.IDataListItem) {
            var i = item;
            var d = this.dataList;
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/dataLists/edit/dataListItemEditView.html',
                controller: 'dataListItemEditController',
                size: 'lg',
                resolve: {
                    dataListItem: () => {
                        return i;
                    },
                    dataList: () => {
                        return d;
                    }
                }
            });
        }

        openAddItem() {
            var i = null;
            var d = this.dataList;
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/dataLists/edit/dataListItemEditView.html',
                controller: 'dataListItemEditController',
                size: 'lg',
                resolve: {
                    dataListItem: () => {
                        return i;
                    },
                    dataList: () => {
                        return d;
                    }
                }
            });


            modalInstance.result.then((newItem) => {
                //TODO: View is not updaed when new item is added

                this.dataList.items.push(newItem);
            });
        }

        openEditRelationship(item: Models.IDataListRelationship) {
            var i = _.clone(item);
            var d = this.dataList;
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/dataLists/edit/dataListRelationshipEditView.html',
                controller: 'dataListRelationshipEditController',
                resolve: {
                    dataListRelationship: () => {
                        return i;
                    },
                    dataList: () => {
                        return d;
                    }
                }
            });


            modalInstance.result.then((updatedItem: Models.IDataListRelationship) => {
                this.dataListRelationshipResource.update(updatedItem).$promise.then((relationship) => {
                    item.name = updatedItem.name;
                });
            });
        }

        openAddRelationship() {
            var i = null;
            var d = this.dataList;
            var modalInstance = this.$uibModal.open({
                animation: true,
                templateUrl: 'comp/home/dataLists/edit/dataListRelationshipEditView.html',
                controller: 'dataListRelationshipEditController',
                resolve: {
                    dataListRelationship: () => {
                        return i;
                    },
                    dataList: () => {
                        return d;
                    }
                }
            });

            modalInstance.result.then((newItem) => {
                this.dataListRelationshipResource.save({ ownerId: this.dataListId }, newItem).$promise.then((relationship) => {
                    this.dataList.relationships.push(relationship);
                });
            });
        }

        deleteRelationship(item: Models.IDataListRelationship) {
            this.dataListRelationshipResource.delete({ ownerId: this.dataListId, id: item.id }).$promise.then(() => {
                _.remove(this.dataList.relationships, (value) => { return value.id === item.id; });
            });
        }

        deleteItem(item: Models.IDataListItem) {
            item.isDeleted = true;
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                return;
            }

            if (this.dataList.items.length < 1) {
                this.toastr.error('Data list is empty! add data items first');
                return;
            }

            var items = _.map(this.dataList.items, (item) => { return item.value; });
            if (items.length !== _.uniq(items).length) {
                this.toastr.error('Data list cannot contain duplicate values!');
                return;
            }

            var dataListId = this.$stateParams['id'];
            if (dataListId === '') {
                this.dataListResource.save(
                    this.dataList,
                    () => { this.$state.go('home.datalists.list'); },
                    (err) => { this.onSubmitError(err) });
            }
            else {
                this.dataListResource.update(
                    this.dataList,
                    () => { this.$state.go('home.datalists.list'); },
                    (err) => { this.onSubmitError(err) });
            }
        }

        onSubmitError(err) {
            console.log(err);
            this.errors = err.data.message;
            if (err.data.exceptionMessage)
                this.errors += err.data.exceptionMessage;

            var innerException = err.data.innerException;
            while (innerException) {
                this.errors += innerException.exceptionMessage;
                innerException = innerException.innerException;
            }

            if (err.data.modelState) {
                _.forEach(err.data.modelState, (error) => {
                    this.toastr.error(error[0]);
                });
            }
        }

        clearErrors() {
            this.errors = undefined;
        }

    }

    angular.module("app").controller("dataListEditController", DataListEditController);
}