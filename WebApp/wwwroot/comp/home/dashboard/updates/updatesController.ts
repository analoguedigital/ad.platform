
module App {
    "use strict";

    interface IUpdatesController {
        activate: () => void;
        updates: Models.MBS.IUpdate[];
    }

    class UpdatesController implements IUpdatesController {
        updates: Models.MBS.IUpdate[];


        static $inject: string[] = ["updateResource"];

        constructor(private updateResource: Resources.IUpdateResource) {
            this.activate();
        }

        activate() {
            this.updateResource.query()
                .$promise.then((updates) => {
                    this.updates = updates;
                });
        }

      
    }

    angular.module("app").controller("updatesController", UpdatesController);
}