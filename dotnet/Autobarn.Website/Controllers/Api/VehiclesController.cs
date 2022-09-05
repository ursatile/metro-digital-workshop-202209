using System.Collections.Generic;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Microsoft.AspNetCore.Mvc;

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

        //// PUT api/<VehiclesController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value) {
        //}

        //// DELETE api/vehicles/{id}
        [HttpDelete("{id}")]
        public void Delete(int id) {
            /* todo: delete the vehicle with the specified id */
            /* question: what should we return here? */
        }
    }
}
