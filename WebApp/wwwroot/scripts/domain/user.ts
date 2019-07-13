﻿module App.Models {
    "use strict";

    export interface IUserBase {
        id: string;
        email: string;
        userName: string;
        emailConfirmed: boolean;
        password: string;
        phoneNumber: string;
        phoneNumberConfirmed: boolean;
        confirmPassword: string;
        roles: string[];
        securityQuestionEnabled: boolean;
        notifications: IAdminNotifications;
    }

    interface IAdminNotifications {
        connectionRequests?: number;
        adviceRecords?: number;
    }

    export interface IUser extends IUserBase, ng.resource.IResource<IUser> { }

    export interface IOrgUser extends IUserBase, ng.resource.IResource<IOrgUser> {
        organisationId: string;
        organisation: IOrganisation;
        firstName: string;
        surname: string;
        type: IOrgUserType;
        accountType: number;
        isRootUser: boolean;
        currentProjectId?: string;
        currentProject: IProject;
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
        isAuthorizedStaff: boolean;
        quota: IMonthlyQuota;
    }
}