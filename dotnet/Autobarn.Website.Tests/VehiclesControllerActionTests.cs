using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Xunit;

namespace Autobarn.Website.Tests {
    public class VehiclesControllerActionTests
    : IClassFixture<TestWebApplicationFactory<Startup>> {
        private readonly TestWebApplicationFactory<Startup> factory;

        public VehiclesControllerActionTests(TestWebApplicationFactory<Startup> factory) {
            this.factory = factory;
        }

        [Fact]
        public async Task GET_Returns_Valid_Vehicle_But_Properly() {
            var client = factory.CreateClient();
            var response = await client.GetAsync("/api/vehicles/OUTATIME");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public void GET_Returns_Valid_Vehicle() {
            var db = new FakeAutobarnDatabase();
            var c = new VehiclesController(db);
            var result = c.Get("TEST1234") as OkObjectResult;
            result.ShouldNotBeNull();
            var vehicle = result.Value as Vehicle;
            vehicle.Registration.ShouldBe("TEST1234");
        }


    }

    public class FakeAutobarnDatabase : IAutobarnDatabase {
        public int CountVehicles() {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Vehicle> ListVehicles() {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Manufacturer> ListManufacturers() {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Model> ListModels() {
            throw new System.NotImplementedException();
        }

        public Vehicle FindVehicle(string registration) {
            return new Vehicle { Registration = "TEST1234" };
        }

        public Model FindModel(string code) {
            throw new System.NotImplementedException();
        }

        public Manufacturer FindManufacturer(string code) {
            throw new System.NotImplementedException();
        }

        public void CreateVehicle(Vehicle vehicle) {
            throw new System.NotImplementedException();
        }

        public void UpdateVehicle(Vehicle vehicle) {
            throw new System.NotImplementedException();
        }

        public void DeleteVehicle(Vehicle vehicle) {
            throw new System.NotImplementedException();
        }
    }
}
