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
    [Route("dataservice")]
    [ApiController]
    public class DataServiceController : ControllerBase
    {
        private readonly IDataServiceManagement _dataService;

        public DataServiceController(IDataServiceManagement dataService)
        {
            _dataService = dataService;
        }

        [Route("addnew")]
        [HttpPost]
        [Authorize]
        public IActionResult AddNewService(DataServicePayload newDataService)
        {
            return Ok(_dataService.AddNewDataService(newDataService, HttpContext.User.Claims.FirstOrDefault().Value));
        }

        [Route("update")]
        [HttpPost]
        [Authorize]
        public IActionResult UpdateService(DataServicePayload updateModel)
        {
            return Ok(_dataService.RenameDataService(updateModel));
        }

        [Route("get")]
        [HttpGet]
        [Authorize]
        public IActionResult GetDataServiceListOrSingle()
        {
            if (Request.Query.ContainsKey("serviceId"))
            {
                return Ok(_dataService.GetDataService(Request.Query["serviceId"]));
            }
            else
            {
                return Ok(_dataService.GetAllDataService(HttpContext.User.Claims.FirstOrDefault().Value));
            }
        }

        [Route("delete")]
        [HttpGet]
        [Authorize]
        public IActionResult DeleteDataService()
        {
            if (Request.Query.ContainsKey("serviceId"))
            {
                return Ok(_dataService.DeleteDataService(Request.Query["serviceId"]));
            }
            else
            {
                return BadRequest("No data service id is provided");
            }
        }

        [Route("operation/addnew")]
        [HttpPost]
        [Authorize]
        public IActionResult AddNewProperty(ServiceOperationPayload newServiceOperation)
        {
            return Ok(_dataService.AddNewOperation(newServiceOperation));
        }

        [Route("operation/get")]
        [HttpGet]
        [Authorize]
        public IActionResult GetServiceOperation(string propId)
        {
            if (Request.Query.ContainsKey("opId"))
            {
                return Ok(_dataService.GetOperation(Request.Query["opId"]));
            }
            else
            {
                return BadRequest("No operation id is provided");
            }
        }

        [Route("operation/update")]
        [HttpPost]
        [Authorize]
        public IActionResult UpdateProperty(ServiceOperationPayload updateProperty)
        {
            return Ok(_dataService.EditOperation(updateProperty));
        }

        [Route("operation/delete")]
        [HttpGet]
        [Authorize]
        public IActionResult DeleteProperty()
        {
            if (Request.Query.ContainsKey("opId"))
            {
                return Ok(_dataService.DeleteOperation(Request.Query["opId"]));
            }
            else
            {
                return BadRequest("No operation id is provided");
            }
        }
    }
}
