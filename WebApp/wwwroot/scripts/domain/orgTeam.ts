module App.Models {
    "use strict";

    export interface IOrgTeam extends ng.resource.IResource<IOrgTeam> {
        id: string;
        organisation: Models.IOrganisation;
        name: string;
        description: string;
        colour: string;
        isActive: boolean;
        managers: IOrgTeamManager[];
        members: IOrgTeamMember[];
        users: IOrgTeamUser[];
    }

    export interface IOrgTeamUser {
        id: string;
        organisationTeam: IOrgTeam;
        orgUser: IOrgUser;
        isManager: boolean;
    }

    export interface IOrgTeamMember {
        organisationTeam: IOrgTeam;
        orgUser: IOrgUser;
    }

    export interface IOrgTeamManager {
        organisationTeam: IOrgTeam;
        orgUser: IOrgUser;
    }
}