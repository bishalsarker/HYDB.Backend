using AutoMapper;
using HYDB.Services.DTO;
using HYDB.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HYDB.API.Controllers
{
    [Route("operation")]
    [ApiController]
    public class OperationController : ControllerBase
    {
        private readonly DataServiceManagement _dataService;
        private readonly MutationOperation _mutationOperationService;
        private readonly QueryOperation _queryOperationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OperationController(IConfiguration config, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _mutationOperationService = new MutationOperation(config, mapper);
            _queryOperationService = new QueryOperation(config, mapper);
            _dataService = new DataServiceManagement(config, mapper);
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("run")]
        [HttpPost]
        public IActionResult RunOperation(OperationRequest opRequest)
        {
            var userName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value;
            if (!_dataService.ValidateOperation(opRequest.Operation, opRequest.Service, userName).HasError)
            {
                var response = new Response();
                var operation = _dataService.GetOperationByOpeartionName(opRequest.Operation, opRequest.Service, userName);
                var dataModel = _dataService.GetDataModelFromOperationDataSource(operation, userName);
                if((operation != null && operation.Type == "mutation") && dataModel != null)
                {
                    _mutationOperationService.AddNewOrUpdateOrDeleteExistingDataObject(
                        opRequest.Args,
                        dataModel.Id
                    );

                    response.IsSuccess = true;
                    response.Message = "Mutation operation successfully executed";
                }

                if ((operation != null && operation.Type == "query") && dataModel != null)
                {
                    response = _queryOperationService.Query(opRequest.Operation, opRequest.Service, userName, opRequest.Args);
                }

                return Ok(response);
            }
            else
            {
                return BadRequest(new Response()
                {
                    IsSuccess = false,
                    Message = "Not a valid operation request"
                });
            }
        }
    }
}
