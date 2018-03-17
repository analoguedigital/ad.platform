
module App.Models {
    "use strict";

    export interface IUserBase {
        id: string;
        email: string;
        emailConfirmed: boolean;
        password: string;
        confirmPassword: string;
        roles: string[];
    }

    export interface IUser extends IUserBase, ng.resource.IResource<IUser> { }

    export interface IOrgUser extends IUserBase, ng.resource.IResource<IOrgUser> {

        organisationId: string;
        organisation: IOrganisation;
        firstName: string;
        surname: string;
        type: IOrgUserType;
        isRootUser: boolean;
        isWebUser: boolean;
        isMobileUser: boolean;
        gender: number;
        birthdate?: Date;
        address: string;
        phoneNumber: string;
        assignments: IProjectAssignment[];
        isMember: boolean;
        isManager: boolean;
        isSelected: boolean;
    }
       

}