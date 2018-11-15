using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class ReportTemplateCategoriesRepository : Repository<ReportTemplateCategory>
    {
        public ReportTemplateCategoriesRepository(UnitOfWork uow) : base(uow) { }
    }
}
