using AutoMapper;
using HYDB.Services.DTO;
using HYDB.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HYDB.API.Controllers
{
    [Route("clients")]
    [ApiController]

    public class ClientsController : Controller
    {
        private readonly ClientsManagement _clientsService;

        public ClientsController(IConfiguration config, IMapper mapper)
        {
            _clientsService = new ClientsManagement(config, mapper);
        }

        [Route("add")]
        [HttpPost]
        [Authorize]
        public IActionResult AddClient(ClientPayload newClientRequest)
        {
            return Ok(_clientsService.AddNewClient(newClientRequest, HttpContext.User.Claims.FirstOrDefault().Value));
        }

        [Route("get")]
        [HttpGet]
        [Authorize]
        public IActionResult GetAllByUser()
        {
            return Ok(_clientsService.GetAllClients(HttpContext.User.Claims.FirstOrDefault().Value));
        }
    }
}
