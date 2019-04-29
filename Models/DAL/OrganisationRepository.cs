using AppHelper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.DAL
{
    public class OrganisationRepository : Repository<Organisation>
    {
        internal OrganisationRepository(UnitOfWork uow) : base(uow) { }

        public Organisation CreateOrganisation(CreateOrganisation dto)
        {
            var org = new Organisation()
            {
                Name = dto.Name,
                TelNumber = dto.TelNumber,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                Town = dto.Town,
                County = dto.County,
                Postcode = dto.Postcode,
                Description = dto.Description,
                Website = dto.Website,
                LogoUrl = dto.LogoUrl,
                TermsAndConditions = dto.TermsAndConditions,
                RequiresAgreement = dto.RequiresAgreement,
                FacebookId = dto.FacebookId,
                TwitterId = dto.TwitterId,
                InstagramId = dto.InstagramId,
                SkypeId = dto.SkypeId,
                LinkedinUrl = dto.LinkedinUrl,
                YouTubeUrl = dto.YouTubeUrl,
                DefaultLanguageId = dto.DefaultLanguageId,
                DefaultCalendarId = dto.DefaultCalendarId,
                IsActive = true
            };

            var rootUser = new OrgUser()
            {
                Organisation = org,
                IsRootUser = true,
                IsWebUser = true,
                IsMobileUser = false,
                UserName = dto.RootUserEmail,
                Email = dto.RootUserEmail,
                FirstName = dto.RootUserFirstName,
                Surname = dto.RootUserSurname,
                TypeId = OrgUserTypesRepository.Administrator.Id,
                AccountType = AccountType.WebAccount,
                RegistrationDate = DateTime.UtcNow
            };

            CurrentUOW.OrganisationRepository.InsertOrUpdate(org);

            using (var tran = CurrentUOW.Context.Database.BeginTransaction())
            {
                CurrentUOW.Save();

                var identityResult = CurrentUOW.UserManager.CreateSync(rootUser, dto.RootPassword);
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.ToString(". "));

                foreach (var role in OrgUserTypesRepository.Administrator.GetRoles())
                {
                    identityResult = CurrentUOW.UserManager.AddToRoleSync(rootUser.Id, role);
                    if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.ToString(". "));
                }

                org.RootUser = rootUser;
                CurrentUOW.Save();

                // if user manager created the application user successfully
                tran.Commit();
            }

            return org;
        }

        public void ChangeStatus(Guid id)
        {
            var org = this.Find(id);
            org.IsActive = !org.IsActive;
            InsertOrUpdate(org);
            Context.SaveChanges();
        }

        public Organisation FindByName(string name)
        {
            return All.Where(o => o.Name == name).FirstOrDefault();
        }
    }
}
