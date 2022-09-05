using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Messages;
using Autobarn.Website.Models;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Autobarn.Website.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase {
        private readonly IAutobarnDatabase db;
        private readonly IBus bus;

        public ModelsController(IAutobarnDatabase db, IBus bus) {
            this.db = db;
            this.bus = bus;
        }
        // GET: api/<ModelsController>
        //[HttpGet]
        //public IEnumerable<string> Get() {
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<ModelsController>/5
        //[HttpGet("{id}")]
        //public string Get(int id) {
        //    return "value";
        //}

        // POST api/<ModelsController>
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(string id, [FromBody] VehicleDto dto) {
            var existing = db.FindVehicle(dto.Registration);
            if (existing != null)
                return Conflict(
                    $"Sorry, you can't sell the same car twice! (and {dto.Registration} is already in our database.)");
            var model = db.FindModel(id);
            var vehicle = new Vehicle() {
                VehicleModel = model,
                Color = dto.Color,
                Year = dto.Year,
                Registration = dto.Registration
            };
            await PublishNewVehicleNotification(vehicle);
            db.CreateVehicle(vehicle);

            return Created($"/api/vehicles/{vehicle.Registration}", vehicle);
        }

        private async Task PublishNewVehicleNotification(Vehicle vehicle) {
            var newVehicleMessage = new NewVehicleMessage {
                Color = vehicle.Color,
                ListedAt = DateTimeOffset.UtcNow,
                ManufacturerName = vehicle.VehicleModel.Manufacturer.Name,
                ModelName = vehicle.VehicleModel.Name,
                Registration = vehicle.Registration,
                Year = vehicle.Year
            };
            await bus.PubSub.PublishAsync(newVehicleMessage);
        }
    }
}
