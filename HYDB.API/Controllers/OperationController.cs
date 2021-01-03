﻿using AutoMapper;
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
        private readonly IOperationsManagement _operationsManagement;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OperationController(IOperationsManagement operationsManagement, IHttpContextAccessor httpContextAccessor)
        {
            _operationsManagement = operationsManagement;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("run")]
        [HttpPost]
        public IActionResult RunOperation(OperationRequest opRequest)
        {
            var client = _operationsManagement.GetClientFromRequest(_httpContextAccessor.HttpContext.Request.Headers["X-API-Key"]);
            if(client != null)
            {
                return Ok(_operationsManagement.RunOperation(opRequest, client));
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
