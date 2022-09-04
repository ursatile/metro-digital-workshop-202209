using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Autobarn.Website.Controllers.api {
	[Route("api/[controller]")]
	[ApiController]
	public class VehiclesController : ControllerBase {
		private readonly IAutobarnDatabase db;

		public VehiclesController(IAutobarnDatabase db) {
			this.db = db;
		}

		[HttpGet]
		public IActionResult Get() {
            var _items = db.ListVehicles().Skip(10).Take(10);
			return Ok(new {
                _links = new {
                    next = new {
                        href = "/api/vehicles/20"
                    }
                },
                items = _items,
                count = 10,
                total = 100,
                index = 3
            });
		}

		[HttpGet("{id}")]
		public IActionResult Get(string id) {
			var vehicle = db.FindVehicle(id);
			if (vehicle == default) return NotFound();
			return Ok(vehicle);
		}

        /// <summary>
        /// Create or update a vehicle.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/vehicles/OUTATIME
        ///     {
        ///        "color": "Silver",
        ///        "year": "1985",
        ///        "modelCode": dmc-delorean
        ///     }
        ///
        /// </remarks>
        /// <param name="id">The vehicle registration code</param>
        /// <param name="dto">An instance of VehicleDto describing the new vehicle</param>
        /// <returns>A newly created Vehicle</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the dto is null or invalid</response> 
		[HttpPut("{id}")]
		public IActionResult Put(string id, [FromBody] VehicleDto dto) {
			var vehicleModel = db.FindModel(dto.ModelCode);
			var vehicle = new Vehicle {
				Registration = dto.Registration,
				Color = dto.Color,
				Year = dto.Year,
				ModelCode = vehicleModel.Code
			};
			db.UpdateVehicle(vehicle);
			return Ok(dto);
		}

		// [HttpDelete("{id}")]
		// public IActionResult Delete(string id) {
		// 	var vehicle = db.FindVehicle(id);
		// 	if (vehicle == default) return NotFound();
		// 	db.DeleteVehicle(vehicle);
		// 	return NoContent();
		// }
	}
}