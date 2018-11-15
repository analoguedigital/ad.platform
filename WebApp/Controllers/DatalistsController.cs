using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.DTO.DataLists;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;
using WebApi.Results;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize(Roles = "System administrator")]
    public class DataListsController : BaseApiController
    {

        private const string CACHE_KEY = "DATA_LISTS";

        // GET api/dataLists
        [ResponseType(typeof(IEnumerable<GetDataListsResDTO>))]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Organisation user")]
        public IHttpActionResult Get()
        {
            if (CurrentUser is OrgUser)
            {
                var cacheKey = $"{CACHE_KEY}_{CurrentOrgUser.OrganisationId}";
                var cacheEntry = MemoryCacher.GetValue(cacheKey);

                if (cacheEntry == null)
                {
                    var dataLists = UnitOfWork.DataListsRepository
                        .AllAsNoTracking
                        .Where(d => d.OrganisationId == CurrentOrgUser.OrganisationId)
                        .OrderBy(d => d.Name)
                        .ToList();

                    var values = dataLists
                        .Where(d => !d.IsAdHoc)
                        .Select(d => Mapper.Map<GetDataListsResItemDTO>(d))
                        .ToList();

                    var result = new GetDataListsResDTO { Items = values.ToList() };

                    MemoryCacher.Add(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                    return Ok(result);
                }
                else
                {
                    var result = (GetDataListsResDTO)cacheEntry;
                    return new CachedResult<GetDataListsResDTO>(result, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if current user is super user
            var _cacheEntry = MemoryCacher.GetValue(CACHE_KEY);
            if (_cacheEntry == null)
            {
                var dataLists = UnitOfWork.DataListsRepository
                    .AllAsNoTracking
                    .OrderBy(d => d.Name)
                    .ToList();

                var values = dataLists
                    .Where(d => !d.IsAdHoc)
                    .Select(d => Mapper.Map<GetDataListsResItemDTO>(d))
                    .ToList();

                var result = new GetDataListsResDTO { Items = values.ToList() };

                MemoryCacher.Add(CACHE_KEY, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (GetDataListsResDTO)_cacheEntry;
                return new CachedResult<GetDataListsResDTO>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/dataLists/{id}
        [DeflateCompression]
        [ResponseType(typeof(DataListDTO))]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Organisation user")]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<DataListDTO>(new DataList()));

            if (CurrentUser is OrgUser)
            {
                var cacheKey = $"{CACHE_KEY}_{CurrentOrgUser.OrganisationId}_{id}";
                var cacheEntry = MemoryCacher.GetValue(cacheKey);

                if (cacheEntry == null)
                {
                    var _dataList = UnitOfWork.DataListsRepository
                        .AllIncludingNoTracking(d => d.AllItems)
                        .Where(d => d.Id == id && d.OrganisationId == CurrentOrgUser.OrganisationId)
                        .SingleOrDefault();

                    if (_dataList == null)
                        return NotFound();

                    var retVal = Mapper.Map<DataListDTO>(_dataList);
                    MemoryCacher.Add(cacheKey, retVal, DateTimeOffset.UtcNow.AddMinutes(1));

                    return Ok(retVal);
                }
                else
                {
                    var retVal = (DataListDTO)cacheEntry;
                    return new CachedResult<DataListDTO>(retVal, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if current user is super user
            var _cacheKey = $"{CACHE_KEY}_{id}";
            var _cacheEntry = MemoryCacher.GetValue(_cacheKey);

            if (_cacheEntry == null)
            {
                var dataList = UnitOfWork.DataListsRepository
                    .AllIncludingNoTracking(d => d.AllItems)
                    .Where(d => d.Id == id)
                    .SingleOrDefault();

                if (dataList == null)
                    return NotFound();

                var result = Mapper.Map<DataListDTO>(dataList);
                MemoryCacher.Add(_cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(1));

                return Ok(result);
            }
            else
            {
                var result = (DataListDTO)_cacheEntry;
                return new CachedResult<DataListDTO>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // GET api/dataLists/{id}/items
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<DataListItemDTO>))]
        [Route("api/datalists/{datalistId}/items")]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Organisation user")]
        public IHttpActionResult GetDataListItems(Guid datalistId)
        {
            if (CurrentUser is OrgUser)
            {
                var cacheKey = $"{CACHE_KEY}_{CurrentOrgUser.OrganisationId}_ITEMS_{datalistId}";
                var cacheEntry = MemoryCacher.GetValue(cacheKey);

                if (cacheEntry == null)
                {
                    var dataList = UnitOfWork.DataListsRepository
                        .AllIncludingNoTracking(d => d.AllItems)
                        .Where(d => d.Id == datalistId && d.OrganisationId == CurrentOrgUser.OrganisationId)
                        .SingleOrDefault();

                    if (dataList == null)
                        return NotFound();

                    var result = dataList.AllItems
                        .Select(i => Mapper.Map<DataListItemDTO>(i))
                        .ToList();

                    return Ok(result);
                }
                else
                {
                    var result = (List<DataListItemDTO>)cacheEntry;
                    return new CachedResult<List<DataListItemDTO>>(result, TimeSpan.FromMinutes(1), this);
                }
            }

            // else if current user is super user
            var _cacheKey = $"{CACHE_KEY}_ITEMS_{datalistId}";
            var _cacheEntry = MemoryCacher.GetValue(_cacheKey);

            if (_cacheEntry == null)
            {
                var _dataList = UnitOfWork.DataListsRepository
                                    .AllIncludingNoTracking(d => d.AllItems)
                                    .Where(d => d.Id == datalistId)
                                    .SingleOrDefault();

                if (_dataList == null)
                    return NotFound();

                var retVal = _dataList.AllItems
                    .Select(i => Mapper.Map<DataListItemDTO>(i))
                    .ToList();

                return Ok(retVal);
            }
            else
            {
                var result = (List<DataListItemDTO>)_cacheEntry;
                return new CachedResult<List<DataListItemDTO>>(result, TimeSpan.FromMinutes(1), this);
            }
        }

        // POST api/dataLists
        public IHttpActionResult Post([FromBody]DataListDTO dataListDTO)
        {
            var dataList = Mapper.Map<DataList>(dataListDTO);
            ModelState.Clear();
            Validate(dataList);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (CurrentUser is SuperUser)
            {
                dataList.OrganisationId = Guid.Parse(dataListDTO.Organisation.Id);
                dataList.Organisation = null;
            }
            else
            {
                dataList.OrganisationId = CurrentOrganisationId.Value;
                dataList.Organisation = null;
            }

            var order = 1;
            foreach (var item in dataList.AllItems)
                item.Order = order++;

            try
            {
                UnitOfWork.DataListsRepository.InsertOrUpdate(dataList);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/dataLists/{id}
        public IHttpActionResult Put(Guid id, [FromBody]DataListDTO dataListDTO)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            var dataList = Mapper.Map<DataList>(dataListDTO);
            ModelState.Clear();
            Validate(dataList);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dbDataList = UnitOfWork.DataListsRepository.Find(id);
            dbDataList.Name = dataList.Name;
            UnitOfWork.DataListsRepository.InsertOrUpdate(dbDataList);

            var order = 1;
            foreach (var val in dataListDTO.Items)
            {
                var dbItem = UnitOfWork.DataListItemsRepository.Find(val.Id);
                if (dbItem == null)
                    dbItem = new DataListItem();

                if (val.IsDeleted)
                    UnitOfWork.DataListItemsRepository.Delete(dbItem);
                else
                {
                    Mapper.Map(val, dbItem);

                    if (dbItem.Attributes != null)
                    {
                        var attributes = dbItem.Attributes.ToList();
                        foreach (var attr in attributes)
                        {
                            attr.OwnerId = dbItem.Id;
                            Mapper.Map(val.Attributes.SingleOrDefault(a => a.Id == attr.Id), attr);
                        }
                    }

                    if (val.Attributes != null)
                    {
                        var newAttributes = val.Attributes.Where(attr => attr.Id == Guid.Empty && attr.ValueId != Guid.Empty).ToList();
                        foreach (var attr in newAttributes)
                        {
                            var newAttr = Mapper.Map<DataListItemAttr>(attr);
                            newAttr.OwnerId = dbItem.Id;
                            dbItem.Attributes.Add(newAttr);
                        }
                    }

                    dbItem.DataListId = id;
                    dbItem.Order = order++;
                    UnitOfWork.DataListItemsRepository.InsertOrUpdate(dbItem);
                }
            }

            try
            {
                UnitOfWork.Save();
                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DEL api/dataLists/{id}
        public IHttpActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("id is empty");

            try
            {
                UnitOfWork.DataListsRepository.Delete(id);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (DbUpdateException)
            {
                return BadRequest("this data list cannot be deleted");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/dataLists/{dataListId}/references
        [ResponseType(typeof(GetDataListReferencesResDTO))]
        [Route("api/datalists/{datalistId}/references")]
        [OverrideAuthorization]
        [Authorize(Roles = "System administrator,Organisation administrator,Organisation user")]
        public IHttpActionResult GetReferences(Guid datalistId)
        {
            if (datalistId == Guid.Empty)
                return Ok(new GetDataListReferencesResDTO { Items = new List<GetDataListReferencesResItemDTO>() });

            var datalist = UnitOfWork.DataListsRepository.Find(datalistId);
            // TODO: refactor this method to account for SuperUsers
            if (datalist == null || datalist.OrganisationId != CurrentOrganisationId)
            {
                var result = new GetDataListReferencesResDTO
                {
                    Items = new List<GetDataListReferencesResItemDTO>()
                };

                return Ok(result);
            }

            return Ok(new GetDataListReferencesResDTO
            {
                Items = UnitOfWork.DataListRelationshipsRepository.All.Where(r => r.DataListId == datalistId)
                        .Select(r => new GetDataListReferencesResItemDTO
                        {
                            Id = r.OwnerId,
                            Name = r.Owner.Name
                        })
                        .ToList()
            });
        }

        // POST api/dataLists/{dataListId}/relationships
        [HttpPost]
        [Route("api/datalists/{datalistId}/relationships")]
        [ResponseType(typeof(DataListRelationshipDTO))]
        public IHttpActionResult AddRelationship(Guid datalistId, [FromBody] AddDataListRelationshipReqDTO req)
        {
            var owner = UnitOfWork.DataListsRepository.Find(datalistId);
            if (owner == null || owner.OrganisationId != CurrentOrganisationId)
                return NotFound();

            var datalist = UnitOfWork.DataListsRepository.Find(req.DataListId);
            if (datalist == null || datalist.OrganisationId != CurrentOrganisationId)
                return BadRequest();

            var newOrder = owner.Relationships.Select(r => r.Order).DefaultIfEmpty().Max() + 1;
            var relationship = new DataListRelationship()
            {
                DataListId = req.DataListId,
                Name = req.Name,
                OwnerId = datalistId,
                Order = newOrder
            };

            try
            {
                UnitOfWork.DataListRelationshipsRepository.InsertOrUpdate(relationship);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            var result = (Mapper.Map<DataListRelationshipDTO>(relationship));

            return Ok(result);
        }

        // PUT api/dataLists/{dataListId}/relationships/{id}
        [HttpPut]
        [Route("api/datalists/{datalistId}/relationships/{id}")]
        public IHttpActionResult EditRelationship(Guid datalistId, Guid id, [FromBody] EditDataListRelationshipReqDTO req)
        {
            var owner = UnitOfWork.DataListsRepository.Find(datalistId);
            if (owner == null || owner.OrganisationId != CurrentOrganisationId)
                return NotFound();

            var relationship = UnitOfWork.DataListRelationshipsRepository.Find(id);
            if (relationship == null || relationship.OwnerId != datalistId)
                return NotFound();

            relationship.Name = req.Name;

            try
            {
                UnitOfWork.DataListRelationshipsRepository.InsertOrUpdate(relationship);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DEL api/dataLists/{dataListId}/relationships/{id}
        [HttpDelete]
        [Route("api/datalists/{datalistId}/relationships/{id}")]
        public IHttpActionResult DeleteRelationship(Guid datalistId, Guid id)
        {
            var owner = UnitOfWork.DataListsRepository.Find(datalistId);
            if (owner == null || owner.OrganisationId != CurrentOrganisationId)
                return NotFound();

            var relationship = UnitOfWork.DataListRelationshipsRepository.Find(id);
            if (relationship == null || relationship.OwnerId != datalistId)
                return NotFound();

            try
            {
                UnitOfWork.DataListRelationshipsRepository.Delete(relationship);
                UnitOfWork.Save();

                MemoryCacher.DeleteStartingWith(CACHE_KEY);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}