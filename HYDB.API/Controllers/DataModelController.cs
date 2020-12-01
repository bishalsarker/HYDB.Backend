using AutoMapper;
using HYDB.Services.DTO;
using HYDB.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HYDB.API.Controllers
{
    [Route("datamodel")]
    [ApiController]
    public class DataModelController : ControllerBase
    {
        private readonly IDataModelManagement _dataModelService;

        public DataModelController(IDataModelManagement dataModelService)
        {
            _dataModelService = dataModelService;
        }

        [Route("addnew")]
        [HttpPost]
        [Authorize]
        public IActionResult AddNewModel(DataModelPayload newDataModel)
        {
            return Ok(_dataModelService.AddNewDataModel(newDataModel, HttpContext.User.Claims.FirstOrDefault().Value));
        }

        [Route("update")]
        [HttpPost]
        [Authorize]
        public IActionResult UpdateProperty(DataModelPayload updateModel)
        {
            return Ok(_dataModelService.RenameDataModel(updateModel));
        }

        [Route("get")]
        [HttpGet]
        [Authorize]
        public IActionResult GetDataModelListOrSingle()
        {
            if(Request.Query.ContainsKey("modelId"))
            {
                return Ok(_dataModelService.GetDataModel(Request.Query["modelId"]));
            }
            else
            {
                return Ok(_dataModelService.GetAllDataModel(HttpContext.User.Claims.FirstOrDefault().Value));
            }
        }

        [Route("delete")]
        [HttpGet]
        [Authorize]
        public IActionResult DeleteDataModel()
        {
            if (Request.Query.ContainsKey("modelId"))
            {
                return Ok(_dataModelService.DeleteDataModel(Request.Query["modelId"]));
            }
            else
            {
                return BadRequest("No data model id is provided");
            }
        }

        [Route("property/addnew")]
        [HttpPost]
        [Authorize]
        public IActionResult AddNewProperty(DataModelPropertyPayload newDataModelProperty)
        {
            return Ok(_dataModelService.AddNewProperty(newDataModelProperty));
        }

        [Route("property/get")]
        [HttpGet]
        [Authorize]
        public IActionResult GetDataModelProperty(string propId)
        {
            if (Request.Query.ContainsKey("propId"))
            {
                return Ok(_dataModelService.GetProperty(Request.Query["propId"]));
            }
            else
            {
                return BadRequest("No property id is provided");
            }
        }

        [Route("property/update")]
        [HttpPost]
        [Authorize]
        public IActionResult UpdateProperty(DataModelPropertyPayload updateProperty)
        {
            return Ok(_dataModelService.EditProperty(updateProperty));
        }

        [Route("property/delete")]
        [HttpGet]
        [Authorize]
        public IActionResult DeleteProperty()
        {
            if (Request.Query.ContainsKey("propId"))
            {
                return Ok(_dataModelService.DeleteProperty(Request.Query["propId"]));
            }
            else
            {
                return BadRequest("No property id is provided");
            }
        }
    }
}
