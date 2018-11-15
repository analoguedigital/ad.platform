using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public class DataListItemsRepository : Repository<DataListItem>
    {
        public DataListItemsRepository(UnitOfWork uow) : base(uow) { }

        public override void InsertOrUpdate(DataListItem entity)
        {
            base.InsertOrUpdate(entity);

            //var repo = new Repository<DataListItemAttr>(CurrentUOW);
            // repo.InsertOrUpdate(entity.Attributes);
        }
    }
}
