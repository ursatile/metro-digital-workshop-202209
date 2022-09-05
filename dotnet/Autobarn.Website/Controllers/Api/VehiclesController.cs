using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.Models;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Autobarn.Website.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase {
        private readonly IAutobarnDatabase db;

        public VehiclesController(IAutobarnDatabase db) {
            this.db = db;
        }

        private const int PAGE_SIZE = 10;

        /// <summary>Gets a list of all the vehicles available in the system.</summary>
        /// <returns>an IEnumerable&lt;Vehicle&gt;. If no vehicles exist, the enumerable will contain no elements. </returns>
        /// <response code="200">Returns a collection of vehicles</response>
        [HttpGet]
        public IActionResult Get(int index = 0) {
            var vehicles = db.ListVehicles().Skip(index).Take(PAGE_SIZE);
            var total = db.CountVehicles();
            dynamic _links = new ExpandoObject();
            _links.self = new { href = $"/api/vehicles?index={index}" };
            if (index > 0) _links.previous = new { href = $"/api/vehicles?index={index - PAGE_SIZE}" };
            if (index + 1 < total) _links.next = new { href = $"/api/vehicles?index={index + PAGE_SIZE}" };
            return Ok(new {
                _links,
                total,
                index,
                count = PAGE_SIZE,
                items = vehicles
            });
        }

        /// <summary>Gets a single vehicle.</summary>
        /// <returns>A vehicle, or 404 if no vehicle exists.</returns>
        /// <response code="200">Returns a single vehicle</response>
        /// <response code="404">No such vehicle found.</response>
        [ProducesResponseType(500)]
        [HttpGet("{id}")]
        public IActionResult Get(string id) {
            var vehicle = db.FindVehicle(id);
            if (vehicle == null) return NotFound();
            return Ok(vehicle);
        }

        //// POST api/<VehiclesController>
        //[HttpPost]
        //public void Post([FromBody] string value) {
        //}

        // PUT api/<VehiclesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] VehicleDto dto) {
            var model = db.FindModel(dto.ModelCode);
            if (model == null) return BadRequest("we couldn't find that model");
            if (!dto.Registration.IsNullOrEmpty() 
                && 
                ! dto.Registration.Equals(id, StringComparison.InvariantCultureIgnoreCase)) return BadRequest("Registration plate must match URL");
            var vehicle = new Vehicle() {
                VehicleModel = model,
                Color = dto.Color,
                Year = dto.Year,
                Registration = id
            };
            db.CreateVehicle(vehicle);
            return Ok(vehicle);
        }

        //// DELETE api/vehicles/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(string id) {
            /* todo: delete the vehicle with the specified id */
            /* question: what should we return here? */
            var vehicle = db.FindVehicle(id);
            if (vehicle == null) return NotFound();
            db.DeleteVehicle(vehicle);
            return Ok();
        }
    }
}
