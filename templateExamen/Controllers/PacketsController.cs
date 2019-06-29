using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using templateExamen.Models;
using templateExamen.Services;
using templateExamen.ViewModels;

namespace templateExamen.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class PacketsController : ControllerBase
        {
            private IPacketService packetService;

            public PacketsController(IPacketService packetservice)
            {
                packetService = packetservice;
            }

            // GET: api/Packet
            [HttpGet]
        //[Authorize(Roles = "Regular, Moderator, Admin")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public IEnumerable<PacketGetModel> Get()
            {

            return packetService.GetAll();
            }

            // GET: api/Packet/5
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [HttpGet("{id}", Name = "GetRole")]
            [Authorize(Roles = "Admin")]
            public IActionResult GetID(int id)
            {
                var found = packetService.GetById(id);
                if (found == null)
                {
                    return NotFound();
                }

                return Ok(found);
            }

            // POST: api/Packet
            [ProducesResponseType(StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [HttpPost]
            [Authorize(Roles = "Moderator, Admin")]
            public IActionResult Post([FromBody] PacketPostModel role)
            {
                packetService.Create(role);
                return Ok();
            }

            // PUT: api/Packet/5
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [HttpPut("{id}")]
            [Authorize(Roles = "Admin")]
            public IActionResult Put(int id, [FromBody] PacketPostModel role)
            {        
            var result = packetService.Upsert(id, role);
                return Ok(result);
            }

            // DELETE: api/ApiWithActions/5
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [HttpDelete("{id}")]
            [Authorize(Roles = "Admin")]
            public IActionResult Delete(int id)
            {            
            var result = packetService.Delete(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
        }
    }

