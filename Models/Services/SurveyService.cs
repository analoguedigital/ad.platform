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
                .Where(x => x.FormTemplate.Discriminator == discriminator);

            if (projectId.HasValue && projectId != Guid.Empty)
            {
                if (orgUser.Type != OrgUserTypesRepository.Administrator)
                {
                    var threadAssignments = UnitOfWork.ThreadAssignmentsRepository
                        .AllAsNoTracking
                        .Where(x => x.OrgUserId == orgUser.Id)
                        .ToList();

                    var projectFound = threadAssignments.Any(x => x.FormTemplate.ProjectId == projectId);
                    var assignment = orgUser.Assignments.SingleOrDefault(a => a.ProjectId == projectId);

                    if (!projectFound)
                    {
                        if (assignment == null || !assignment.CanView)
                            return null;
                    }
                }

                surveys = surveys.Where(s => s.ProjectId == projectId);

                result = surveys
                    .ToList()
                    .OrderByDescending(x => x.Date)
                    .Select(s => Mapper.Map<FilledFormDTO>(s))
                    .ToList();
            }
            else
            {
                // return all projects that this user has a case or thread assignment for.
                var caseSurveys = surveys.Where(s => s.Project.Assignments.Any(a => a.OrgUserId == orgUser.Id && a.CanView));
                var threadSurveys = surveys.Where(s => s.FormTemplate.Assignments.Any(a => a.OrgUserId == orgUser.Id && a.CanView));

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

            return result;
        }

    }
}
