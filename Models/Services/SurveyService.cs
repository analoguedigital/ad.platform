using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Services
{
    public class SurveyService
    {

        private UnitOfWork UnitOfWork { get; set; }

        public SurveyService(UnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public List<FilledFormDTO> GetAllSurveys(FormTemplateDiscriminators discriminator, Guid? projectId = null)
        {
            var surveys = UnitOfWork.FilledFormsRepository
                .AllAsNoTracking
                .Where(x => x.FormTemplate.Discriminator == discriminator);

            if (projectId.HasValue && projectId != Guid.Empty)
                surveys = surveys.Where(s => s.ProjectId == projectId);

            var result = surveys
                .ToList()
                .OrderByDescending(x => x.Date)
                .Select(s => Mapper.Map<FilledFormDTO>(s))
                .ToList();

            return result;
        }

        public List<FilledFormDTO> GetUserSurveys(OrgUser orgUser, FormTemplateDiscriminators discriminator, Guid? projectId = null)
        {
            var result = new List<FilledFormDTO>();

            var surveys = UnitOfWork.FilledFormsRepository
                .AllAsNoTracking
                .Where(x => x.FormTemplate.Discriminator == discriminator)
                .AsQueryable();

            if (projectId.HasValue && projectId != Guid.Empty)
            {
                surveys = surveys.Where(s => s.ProjectId == projectId);

                // clients and staff members need an assignment
                if (orgUser.Type != OrgUserTypesRepository.Administrator)
                {
                    var threadAssignments = UnitOfWork.ThreadAssignmentsRepository
                        .AllAsNoTracking
                        .Where(x => x.OrgUserId == orgUser.Id)
                        .ToList();

                    var projectFound = threadAssignments.Any(x => x.FormTemplate.ProjectId == projectId);
                    var assignment = orgUser.Assignments.SingleOrDefault(a => a.ProjectId == projectId);

                    // if a thread or project assignment is not found,
                    // the current user doesn't have access to this.
                    if (!projectFound)
                        if (assignment == null || !assignment.CanView)
                            return null;
                }

                var project = UnitOfWork.ProjectsRepository.Find(projectId.Value);
                if (project.IsAggregate && orgUser.Type != OrgUserTypesRepository.Administrator)
                    surveys = surveys.Where(x => x.FilledById == orgUser.Id);
                //else
                //{
                //    if (project.CreatedBy is OrgUser caseOwner && caseOwner.AccountType == AccountType.MobileAccount)
                //    {
                //        if (project.CreatedById != orgUser.Id)
                //            surveys = surveys.Where(x => x.FilledById == project.CreatedById);
                //    }
                //}

                result = surveys
                    .ToList()
                    .OrderByDescending(x => x.Date)
                    .Select(s => Mapper.Map<FilledFormDTO>(s))
                    .ToList();
            }
            else
            {
                if (orgUser.Type == OrgUserTypesRepository.Administrator)
                {
                    surveys = surveys.Where(x => x.Project.OrganisationId == orgUser.OrganisationId);
                    result = surveys
                        .ToList()
                        .OrderByDescending(x => x.Date)
                        .Select(x => Mapper.Map<FilledFormDTO>(x))
                        .ToList();
                }
                else
                {
                    // return all projects that this user has a case or thread assignment for.
                    var caseSurveys = surveys.Where(s => s.Project.Assignments.Any(a => a.OrgUserId == orgUser.Id && a.CanView));
                    var threadSurveys = surveys.Where(s => s.FormTemplate.Assignments.Any(a => a.OrgUserId == orgUser.Id && a.CanView));

                    if (discriminator == FormTemplateDiscriminators.RegularThread)
                    {
                        caseSurveys = caseSurveys.Where(x => x.FilledById == orgUser.Id);
                        threadSurveys = threadSurveys.Where(x => x.FilledById == orgUser.Id);
                    }

                    var joinedSurveys = new List<FilledForm>();
                    joinedSurveys.AddRange(caseSurveys.ToList());
                    joinedSurveys.AddRange(threadSurveys.ToList());
                    joinedSurveys = joinedSurveys.Distinct().ToList();

                    result = joinedSurveys
                        .ToList()
                        .OrderByDescending(x => x.Date)
                        .Select(s => Mapper.Map<FilledFormDTO>(s))
                        .ToList()
                        .Distinct()
                        .ToList();
                }
            }

            return result;
        }

    }
}
