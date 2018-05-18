
module App {
    "use strict";

    interface PeriodOptionModel {
        value: number;
        text: string;
    }

    interface IOrganisationVouchersEditControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        currentPage: number;
        numberOfPages: number;
        pageSize: number;
        periodOptions: PeriodOptionModel[];

        voucher: Models.IVoucher;
        organisations: Models.IOrganisation[];

        currentUserIsSuperUser: boolean;
    }

    interface IOrganisationVouchersEditController {
        activate: () => void;
    }

    class OrganisationVouchersEditController implements IOrganisationVouchersEditController {
        static $inject: string[] = ["$scope", "$state", "$stateParams", "organisationResource", "voucherResource", "userContextService", "toastr"];

        constructor(
            private $scope: IOrganisationVouchersEditControllerScope,
            private $state: ng.ui.IStateService,
            private $stateParams: ng.ui.IStateParamsService,
            private organisationResource: Resources.IOrganisationResource,
            private voucherResource: Resources.IVoucherResource,
            private userContextService: Services.IUserContextService,
            private toastr: any) {

            $scope.title = "Edit Voucher";

            $scope.periodOptions = [];
            for (var i = 1; i <= 12; i++) {
                var itemText = i === 1 ? 'month' : 'months';
                this.$scope.periodOptions.push({
                    value: i,
                    text: `${i} ${itemText}`
                });
            }

            this.activate();
        }
        
        activate() {
            this.load();
        }

        load() {
            var voucherId = this.$stateParams["id"];
            if (voucherId != '') {
                this.voucherResource.get({ id: voucherId }).$promise.then((voucher) => {
                    this.$scope.voucher = voucher;
                });
            } else {
                this.$scope.voucher = <Models.IVoucher>{
                    id: '',
                    title: '',
                    code: '',
                    period: 1,
                    isRedeemed: false,
                    organisationId: ''
                };
            }

            var roles = ["System administrator", "Platform administrator"];
            this.$scope.currentUserIsSuperUser = this.userContextService.userIsInAnyRoles(roles);

            if (this.$scope.currentUserIsSuperUser) {
                this.organisationResource.query().$promise.then((organisations) => {
                    this.$scope.organisations = organisations;
                });
            }
        }

        submit(form: ng.IFormController) {
            if (form.$invalid) {
                this.toastr.warning('Form entry is not valid');
                return;
            }

            var voucherId = this.$stateParams['id'];
            if (voucherId === '') {
                this.voucherResource.save(
                    this.$scope.voucher,
                    () => { this.$state.go('home.organisations.vouchers'); },
                    (err) => {
                        console.log(err);
                        this.$scope.errors = err.data.exceptionMessage;
                    });
            }
            else {
                this.voucherResource.update(
                    this.$scope.voucher,
                    () => { this.$state.go('home.organisations.vouchers'); },
                    (err) => {
                        console.log(err);
                        this.$scope.errors = err.data.exceptionMessage;
                    });
            }
        }
    }

    angular.module("app").controller("organisationVouchersEditController", OrganisationVouchersEditController);
}