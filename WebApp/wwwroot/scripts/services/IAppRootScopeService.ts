
module App {
    "use strict";

    export interface IAppRootScopeService extends ng.IRootScopeService {
        $state: ng.ui.IStateService;
        $stateParams: ng.ui.IStateParamsService;
    }
}


