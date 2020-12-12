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

        [Route("run/{serviceName}/{opName}")]
        public async Task<IActionResult> RunOperation(string serviceName, string opName)
        {
            var httpMethod = _httpContextAccessor.HttpContext.Request.Method;
            var userName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value;
            if (httpMethod == "POST")
            {
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    var validationResult = _dataService.ValidateOperation(opName, serviceName, userName, httpMethod);
                    if (!validationResult.HasError)
                    {
                        var postBody = await reader.ReadToEndAsync();
                        var dataObjectRequest = JsonConvert.DeserializeObject<DataObjectRequest>(postBody);
                        _mutationOperationService.AddNewOrUpdateOrDeleteExistingDataObject(
                            dataObjectRequest.DataObject,
                            dataObjectRequest.DataModelId
                        );
                        return Ok(new Response() { IsSuccess = true, Message = "Mutation operation successfully executed" });
                    }
                    else
                    {
                        return BadRequest(new Response() { 
                            IsSuccess = false,
                            Message = validationResult.Message
                        });
                    }
                }
            }
            else
            {
                var validationResult = _dataService.ValidateOperation(opName, serviceName, userName, httpMethod);
                if (!validationResult.HasError)
                {
                    var queryParams = _httpContextAccessor.HttpContext.Request.Query["args"];
                    if (queryParams.Count < 1)
                    {
                        return Ok(_queryOperationService.Query(opName, serviceName, userName, null));
                    }
                    else
                    {
                        return Ok(_queryOperationService.Query(opName, serviceName, userName, queryParams[0]));
                    }
                }
                else
                {
                    return BadRequest("Only queries are allowed for GET request");
                }
            }
        }
    }
}
