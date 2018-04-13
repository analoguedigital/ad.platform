
module App {
    "use strict";

    interface IOrganisationVouchersControllerScope extends ng.IScope {
        title: string;
        searchTerm: string;
        currentPage: number;
        numberOfPages: number;
        pageSize: number;

        vouchers: Models.IVoucher[];
        displayedVouchers: Models.IVoucher[];
    }

    interface IOrganisationVouchersController {
        activate: () => void;
    }

    class OrganisationVouchersController implements IOrganisationVouchersController {
        static $inject: string[] = ["$scope", "organisationResource", "voucherResource", "toastr"];

        constructor(
            private $scope: IOrganisationVouchersControllerScope,
            private organisationResource: Resources.IOrganisationResource,
            private voucherResource: Resources.IVoucherResource,
            private toastr: any) {

            $scope.title = "Vouchers";
            this.activate();
        }

        activate() {
            this.load();
        }

        load() {
            this.voucherResource.query().$promise.then((vouchers) => {
                this.$scope.vouchers = vouchers;
                this.$scope.displayedVouchers = [].concat(this.$scope.vouchers);
            });
        }

        delete(id: string) {
            this.voucherResource.delete({ id: id }).$promise.then((res) => {
                this.load();
            }, (err) => {
                console.error(err);
            });
        }
        
    }

    angular.module("app").controller("organisationVouchersController", OrganisationVouchersController);
}