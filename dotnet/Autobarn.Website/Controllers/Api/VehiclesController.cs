using System.Collections.Generic;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.Models;
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
        // GET: api/<VehiclesController>
        [HttpGet]
        public IEnumerable<Vehicle> Get() {
            return db.ListVehicles();
        }

        // GET api/vehicles/OUTATIME
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
            if (dto.Registration != id) return BadRequest("Registration plate must match URL");
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
