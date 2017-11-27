using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.DAL;

public class ModelUpdater
{
    public static T CreateModel<T>(ICollection<KeyValuePair<string, string>> values, SurveyContext dbContext) where T : IEntity, new()
    {
        T result = new T();

        UpdateModel<T>(result, values, dbContext);

        return result;
    }

    public static void UpdateModel<T>(T obj, ICollection<KeyValuePair<string, string>> values, SurveyContext dbContext) where T : IEntity, new()
    {

        values.Remove(values.SingleOrDefault(v => v.Key == "__RequestVerificationToken"));

        var ImmediateProperties = values.Where(v => !v.Key.Contains('.')).ToList();
        foreach (var item in ImmediateProperties)
        {
            UpdateProperty(obj, item.Key, item.Value);
            values.Remove(item);
        }


        var nestedCollectionValuesIndexes = values.Where(v => v.Key.EndsWith(".index")).ToList();
        foreach (var index in nestedCollectionValuesIndexes)
        {
            var collectionName = index.Key.Substring(0, index.Key.IndexOf("."));

            var objectIds = index.Value.ToString().Split(new[] { ',' }).Distinct();
            foreach (var id in objectIds)
            {
                var collectionValues = values.Where(v => v.Key.Contains(id)).Except(new[] { index }).ToList();

                UpdateCollectionItem(obj, collectionName, Guid.Parse(id), collectionValues, dbContext);

                foreach (var value in collectionValues)
                    values.Remove(value);
            }
            values.Remove(index);
        }


        var associations = values.ToList();
        foreach (var item in associations)
        {
            var propertyName = item.Key.Substring(0, item.Key.IndexOf('.'));
            var property = obj.GetType().GetProperty(propertyName).GetValue(obj);

            UpdateProperty(property, item.Key.Substring(item.Key.IndexOf(".") + 1), item.Value);
        }

    }

    private static void UpdateCollectionItem(object obj, string collectionName, Guid itemId, IEnumerable<KeyValuePair<string, string>> values, SurveyContext dbContext)
    {
        
        var collectionProperty = obj.GetType().GetProperty(collectionName);
        var collection = collectionProperty.GetValue(obj);

        object member = null;
        foreach (var item in collection as IEnumerable<IEntity>)
        {
            if ((item as IEntity).Id == itemId)
            {
                member = item;
                if (!values.Any())   // collection item has been deleted 
                {
                    if (member is IArchivable && ((IArchivable)member).MustBeArchived(dbContext))
                        ((IArchivable)member).Archive();
                    else
                        dbContext.Entry(member).State = EntityState.Deleted;
                    return;
                }

                break;
            }
        }

        if (member == null)
        {
            if (values.Any())      // It is a new item
            {
                member = Activator.CreateInstance(collectionProperty.PropertyType.GetGenericArguments()[0]);
                collection.GetType().GetMethod("Add").Invoke(collection, new[] { member });
            }
        }

        foreach (var value in values)
            UpdateProperty(member, value.Key.Substring(value.Key.IndexOf('.') + 1), value.Value);
    }

    private static void UpdateProperty(object obj, string properyName, object value)
    {
        var propinfo = obj.GetType().GetProperty(properyName);

        var x = TypeDescriptor.GetConverter(propinfo.PropertyType).ConvertFromInvariantString(value.ToString());

        propinfo.SetValue(obj, x, null);
    }
}

