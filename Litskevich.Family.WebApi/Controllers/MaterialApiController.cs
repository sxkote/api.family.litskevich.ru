using Litskevich.Family.Domain.Contracts.Managers;
using Litskevich.Family.WebApi.Models;
using Litskevich.Family.WebApi.Models.Requests;
using SXCore.Common.Exceptions;
using SXCore.WebApi;
using SXCore.WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Litskevich.Family.WebApi.Controllers
{
    [ApiAuthorize(Roles = "Supervisor,Admin")]
    [RoutePrefix("api/material")]
    public class MaterialApiController : CoreApiController
    {
        private IMaterialsManager _manager;

        public MaterialApiController(IMaterialsManager manager)
        {
            _manager = manager;
        }

        [ApiAuthorize()]
        [HttpGet]
        [Route("{id:long}")]
        public MaterialModel Get(long id)
        {
            var material = _manager.GetMaterial(id);
            if (material == null)
                throw new CustomNotFoundException($"Material with ID={id} not found!");

            return material.ToSerial();
        }

        [HttpPost]
        [Route("")]
        public MaterialModel CreateMaterial([FromBody]CreateMaterialRequest request)
        {
            return _manager.CreateMaterial(request.ArticleID, request.FileCode).ToSerial();
        }

        [HttpPost]
        [Route("{materialID:long}")]
        public void UpdateMaterial(long materialID, [FromBody]UpdateMaterialRequest request)
        {
            _manager.UpdateMaterial(materialID, request.Date, request.Title, request.Comment);
        }

        [HttpDelete]
        [Route("{materialID:long}")]
        public void DeleteMaterial(long materialID)
        {
            _manager.DeleteMaterial(materialID);
        }

        [HttpPost]
        [Route("{materialID:long}/transform")]
        public void TransformMaterial(long materialID, [FromBody]MaterialTransformRequest request)
        {
            _manager.TransformMaterial(materialID, request.Method, request.Argument);
        }
    }
}
