using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.Services
{
    public class UserService
    {

        private UnitOfWork UnitOfWork { get; set; }

        public UserService(UnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public List<OrgUserDTO> GetOrgUsers(OrgUserListType listType, Guid? organisationId = null)
        {
            if (listType == OrgUserListType.InactiveAccounts)
                return GetInactiveAccounts(organisationId);

            IQueryable<OrgUser> dataSource = UnitOfWork.OrgUsersRepository
                .AllIncluding(u => u.Type)
                .OrderBy(u => u.Surname)
                .ThenBy(u => u.FirstName)
                .AsQueryable();

            if (organisationId.HasValue)
                dataSource = dataSource.Where(u => u.OrganisationId == organisationId);

            if (listType != OrgUserListType.AllAccounts)
            {
                var accountType = (AccountType)(int)listType;
                dataSource = dataSource.Where(x => x.AccountType == accountType);
            }

            var result = dataSource
                .ToList()
                .Select(u => Mapper.Map<OrgUserDTO>(u))
                .ToList();

            return result;
        }

        private List<OrgUserDTO> GetInactiveAccounts(Guid? organisationId = null)
        {
            IQueryable<OrgUser> users = UnitOfWork.OrgUsersRepository
                .AllAsNoTracking
                .Where(x => !x.IsRootUser)
                .AsQueryable();

            if (organisationId.HasValue)
                users = users.Where(x => x.OrganisationId == organisationId);

            List<OrgUser> inactiveAccounts = new List<OrgUser>();

            foreach (var user in users)
            {
                var surveysCount = UnitOfWork.FilledFormsRepository
                    .AllAsNoTracking
                    .Where(x => x.FilledById == user.Id)
                    .Count();

                if (surveysCount == 0)
                    inactiveAccounts.Add(user);
            }

            var result = inactiveAccounts
                .OrderBy(x => x.Surname)
                .ThenBy(x => x.FirstName)
                .ToList()
                .Select(x => Mapper.Map<OrgUserDTO>(x))
                .ToList();

            return result;
        }

        public List<OrgUserDTO> GetOnRecordStaff()
        {
            var onrecord = UnitOfWork.OrganisationRepository
                .AllAsNoTracking
                .Where(x => x.Name == "OnRecord")
                .SingleOrDefault();

            var users = UnitOfWork.OrgUsersRepository
                .AllIncluding(x => x.Type)
                .OrderBy(x => x.Surname)
                .ThenBy(x => x.FirstName)
                .Where(x => x.AccountType == AccountType.WebAccount && x.OrganisationId == onrecord.Id);

            var result = users
                .ToList()
                .Select(x => Mapper.Map<OrgUserDTO>(x))
                .ToList();

            return result;
        }

        public OrgUserDTO GetOrgUser(Guid id)
        {
            var orgUser = UnitOfWork.OrgUsersRepository.Find(id);
            var result = Mapper.Map<OrgUserDTO>(orgUser);

            result.Assignments = orgUser.Assignments
                .Select(a => Mapper.Map<ProjectAssignmentDTO>(a))
                .ToList();

            return result;
        }

    }
}
