using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.DTO.DataLists;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.Filters;

namespace WebApi.Controllers
{
    public class DataListsController : BaseApiController
    {

        // GET api/dataLists
        [ResponseType(typeof(IEnumerable<GetDataListsResDTO>))]
        public IHttpActionResult Get()
        {
            var result = new List<GetDataListsResItemDTO>();

            if (this.CurrentOrgUser != null)
            {
                var datalists = UnitOfWork.DataListsRepository.AllAsNoTracking
                    .Where(d => d.OrganisationId == CurrentOrgUser.OrganisationId)
                    .OrderBy(d => d.Name)
                    .ToList();

                result = datalists
                    .Where(d => !d.IsAdHoc)
                    .Select(d => Mapper.Map<GetDataListsResItemDTO>(d))
                    .ToList();
            }
            else
            {
                var datalists = UnitOfWork.DataListsRepository.AllAsNoTracking
                    .OrderBy(d => d.Name)
                    .ToList();

                result = datalists
                    .Where(d => !d.IsAdHoc)
                    .Select(d => Mapper.Map<GetDataListsResItemDTO>(d))
                    .ToList();
            }

            return Ok(new GetDataListsResDTO() { Items = result.ToList() });
        }

        // GET api/dataLists/{id}
        [DeflateCompression]
        [ResponseType(typeof(DataListDTO))]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
                return Ok(Mapper.Map<DataListDTO>(new DataList()));

            var datalist = UnitOfWork.DataListsRepository
                .AllIncludingNoTracking(d => d.AllItems)
                .Where(d => d.Id == id && d.OrganisationId == CurrentOrgUser.OrganisationId)
                .SingleOrDefault();

            if (datalist == null)
                return NotFound();

            return Ok(Mapper.Map<DataListDTO>(datalist));
        }

        // GET api/dataLists/{id}/items
        [DeflateCompression]
        [ResponseType(typeof(IEnumerable<DataListItemDTO>))]
        [Route("api/datalists/{datalistId}/items")]
        public IHttpActionResult GetDataListItems(Guid datalistId)
        {
            var datalist = UnitOfWork.DataListsRepository
                .AllIncludingNoTracking(d => d.AllItems)
                .Where(d => d.Id == datalistId && d.OrganisationId == CurrentOrgUser.OrganisationId)
                .SingleOrDefault();

            if (datalist == null)
                return NotFound();

            return Ok(datalist.AllItems.Select(i => Mapper.Map<DataListItemDTO>(i)));
        }

        // POST api/dataLists
        public IHttpActionResult Post([FromBody]DataListDTO dataListDTO)
        {
            var dataList = Mapper.Map<DataList>(dataListDTO);
            ModelState.Clear();
            Validate(dataList);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dataList.OrganisationId = CurrentOrganisationId.Value;

            var order = 1;
            foreach (var item in dataList.AllItems)
                item.Order = order++;

            UnitOfWork.DataListsRepository.InsertOrUpdate(dataList);
            UnitOfWork.Save();

            return Ok();
        }

        // PUT api/dataLists/{id}
        public IHttpActionResult Put(Guid id, [FromBody]DataListDTO dataListDTO)
        {
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

            UnitOfWork.Save();

            return Ok();
        }

        // DEL api/dataLists/{id}
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                UnitOfWork.DataListsRepository.Delete(id);
                UnitOfWork.Save();

                return Ok();
            }
            catch (DbUpdateException)
            {
                return BadRequest("This Data List cannot be deleted!");
            }
        }

        // GET api/dataLists/{dataListId}/references
        [ResponseType(typeof(GetDataListReferencesResDTO))]
        [Route("api/datalists/{datalistId}/references")]
        public IHttpActionResult GetReferences(Guid datalistId)
        {
            if (datalistId == Guid.Empty)
                return Ok(new GetDataListReferencesResDTO { Items = new List<GetDataListReferencesResItemDTO>() });

            var datalist = UnitOfWork.DataListsRepository.Find(datalistId);
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

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}