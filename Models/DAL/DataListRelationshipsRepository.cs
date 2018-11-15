using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class DataListRelationshipsRepository : Repository<DataListRelationship>
    {
        public DataListRelationshipsRepository(UnitOfWork uow) : base(uow) { }
    }
}
