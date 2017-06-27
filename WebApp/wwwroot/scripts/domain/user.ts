
module App.Models {
    "use strict";

    export interface IUserBase {

        id: string;
        email: string;
        password: string;
        confirmPassword: string;
        roles: string[];
    }

    export interface IUser extends IUserBase, ng.resource.IResource<IUser> { }

    export interface IOrgUser extends IUserBase, ng.resource.IResource<IOrgUser> {

        organisationId: string;
        firstName: string;
        surname: string;
        type: IOrgUserType;
        isRootUser: boolean;
        isWebUser: boolean;
        isMobileUser: boolean;
    }
       

}