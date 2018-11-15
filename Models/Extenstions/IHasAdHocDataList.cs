using LightMethods.Survey.Models.DAL;
using System;
using System.Collections.Generic;

namespace LightMethods.Survey.Models.Entities
{
    public static class IHasAdHocDataListExtensions
    {
        public static void UpdateDataList(this IHasAdHocDataList entity, UnitOfWork uow,
            Organisation org, bool newIsAdHoc, IEnumerable<DataListItem> newAdHocItems,
            IEnumerable<DataListItem> deletedAdHocItems, Guid? newDataListId, DataList oldDataList)
        {

            if (oldDataList != null && !newIsAdHoc && newDataListId != null && oldDataList.IsAdHoc)
                uow.DataListsRepository.Delete(oldDataList);

            if (!newIsAdHoc)
                return;

            var dbDataList = (oldDataList?.IsAdHoc ?? false) ? oldDataList : null;
            if (dbDataList == null)
            {
                dbDataList = new DataList() { Name = string.Empty, OrganisationId = org.Id };
            }

            uow.DataListsRepository.InsertOrUpdate(dbDataList);
            entity.DataList = dbDataList;

            var order = 1;
            foreach (var val in newAdHocItems)
            {
                var dbItem = uow.DataListItemsRepository.Find(val.Id);
                if (dbItem == null)
                    dbItem = new DataListItem();

                dbItem.Text = val.Text;
                dbItem.Value = val.Value;
                dbItem.DataList = dbDataList;
                dbItem.Order = order++;
                uow.DataListItemsRepository.InsertOrUpdate(dbItem);

            }

            foreach (var val in deletedAdHocItems)
            {
                var dbItem = uow.DataListItemsRepository.Find(val.Id);
                if (dbItem == null)
                    return;

                uow.DataListItemsRepository.Delete(dbItem);
            }
        }
    }
}
