using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.DTO.DataLists;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace WebApi.Controllers
{
    public class DataListsController : BaseApiController
    {

        [ResponseType(typeof(IEnumerable<GetDataListsResDTO>))]
        public IHttpActionResult Get()
        {
            var datalists = UnitOfWork.DataListsRepository.All
                .Where(d => d.OrganisationId == CurrentOrgUser.OrganisationId)
                .OrderBy(d => d.Name)
                .ToList()
                .Where(d => !d.IsAdHoc)
                .Select(d => Mapper.Map<GetDataListsResItemDTO>(d));

            return Ok(new GetDataListsResDTO() { Items = datalists.ToList() });
        }

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

        public IHttpActionResult Post([FromBody]DataListDTO dataListDTO)
        {
            var dataList = Mapper.Map<DataList>(dataListDTO);
            ModelState.Clear();
            Validate(dataList);

            if (ModelState.IsValid)
            {
                dataList.OrganisationId = CurrentOrganisationId.Value;
                UnitOfWork.DataListsRepository.InsertOrUpdate(dataList);
                var order = 1;
                foreach (var val in dataListDTO.Items)
                {
                    if (val.IsDeleted)
                        continue; // nothing to do

                    var dbItem = new DataListItem();
                    Mapper.Map(val, dbItem);
                    dbItem.DataList = dataList;
                    dbItem.Order = order++;
                    UnitOfWork.DataListItemsRepository.InsertOrUpdate(dbItem);
                }

                UnitOfWork.Save();
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        public IHttpActionResult Put(Guid id, [FromBody]DataListDTO dataListDTO)
        {
            var dataList = Mapper.Map<DataList>(dataListDTO);
            ModelState.Clear();
            Validate(dataList);

            if (ModelState.IsValid)
            {
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
                    {
                        UnitOfWork.DataListItemsRepository.Delete(dbItem);
                    }
                    else
                    {
                        Mapper.Map(val, dbItem);
                        dbItem.Attributes.ToList().ForEach(attr =>
                        {
                            attr.OwnerId = dbItem.Id;
                            Mapper.Map(val.Attributes.SingleOrDefault(a => a.Id == attr.Id), attr);
                        });
                        val.Attributes.Where(att => att.Id == Guid.Empty && att.ValueId != Guid.Empty).ToList().ForEach(att =>
                        {
                            var newAtt = Mapper.Map<DataListItemAttr>(att);
                            newAtt.OwnerId = dbItem.Id;
                            dbItem.Attributes.Add(newAtt);
                        });

                        dbItem.DataListId = id;
                        dbItem.Order = order++;
                        UnitOfWork.DataListItemsRepository.InsertOrUpdate(dbItem);
                    }
                }


                UnitOfWork.Save();
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        public void Delete(Guid id)
        {
            UnitOfWork.DataListsRepository.Delete(id);
            UnitOfWork.Save();
        }

        [ResponseType(typeof(GetDataListReferencesResDTO))]
        [Route("api/datalists/{datalistId}/references")]
        public IHttpActionResult GetReferences(Guid datalistId)
        {
            var datalist = UnitOfWork.DataListsRepository.Find(datalistId);

            if (datalist == null || datalist.OrganisationId != CurrentOrganisationId)
                return Ok(new GetDataListReferencesResDTO { Items = new List<GetDataListReferencesResItemDTO>() });

            return Ok(new GetDataListReferencesResDTO
            {
                Items = UnitOfWork.DataListRelationshipsRepository.All.Where(r => r.DataListId == datalistId)
                        .Select(r => new GetDataListReferencesResItemDTO
                        {
                            Id = r.OwnerId,
                            Name = r.Owner.Name
                        }).ToList()
            });

        }

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

            UnitOfWork.DataListRelationshipsRepository.InsertOrUpdate(relationship);
            UnitOfWork.Save();

            return Ok(Mapper.Map<DataListRelationshipDTO>(relationship));

        }

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

            UnitOfWork.DataListRelationshipsRepository.InsertOrUpdate(relationship);
            UnitOfWork.Save();

            return Ok();
        }

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

            UnitOfWork.DataListRelationshipsRepository.Delete(relationship);
            UnitOfWork.Save();

            return Ok();
        }

    }
}