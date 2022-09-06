using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;

namespace Autobarn.Website.GraphQL.Queries {
    public sealed class VehicleQuery : ObjectGraphType {
        private readonly IAutobarnDatabase db;

        public VehicleQuery(IAutobarnDatabase db) {
            this.db = db;
            Field<ListGraphType<VehicleGraphType>>("Vehicles")
                .Description("Return all vehicles")
                .Resolve(GetAllVehicles);

            Field<VehicleGraphType>("Vehicle")
                .Description("Get a single car")
                .Arguments(MakeNonNullStringArgument("registration", "The registration of the vehicle you want"))
                .Resolve(GetVehicle);

            Field<ListGraphType<VehicleGraphType>>("VehiclesByColor")
                .Description("Query to retrieve all Vehicles matching the specified color")
                .Arguments(MakeNonNullStringArgument("color", "The name of a color, eg 'blue', 'grey'"))
                .Resolve(GetVehiclesByColor);
            
            Field<ListGraphType<VehicleGraphType>>("SearchVehicles")
                .Description("Query to find vehicles based on various criteria")
                .Arguments(
                    new QueryArgument<StringGraphType> { Name = "color" },
                    new QueryArgument<IntGraphType> { Name = "year" },
                    new QueryArgument<StringGraphType> { Name = "model" }
                ).Resolve(SearchVehicles);
        }

        private QueryArgument MakeNonNullStringArgument(string name, string description) {
            return new QueryArgument<NonNullGraphType<StringGraphType>> {
                Name = name, Description = description
            };
        }

        private Vehicle GetVehicle(IResolveFieldContext<object> context) {
            var registration = context.GetArgument<string>("registration");
            return db.FindVehicle(registration) ?? throw new Exception("No such car!");
        }

        private IEnumerable<Vehicle> GetAllVehicles(IResolveFieldContext<object> arg) {
            return db.ListVehicles();
        }

        private IEnumerable<Vehicle> GetVehiclesByColor(IResolveFieldContext<object> context) {
            var color = context.GetArgument<string>("color");
            var vehicles = db.ListVehicles().Where(v => v.Color.Contains(color, StringComparison.InvariantCultureIgnoreCase));
            return vehicles;
        }

        private IEnumerable<Vehicle> SearchVehicles(IResolveFieldContext<object> context)
        {
            var color = context.GetArgument<string>("color");
            var year = context.GetArgument<int?>("year");
            var model = context.GetArgument<string>("model");
            var results = db.ListVehicles()
                .Where(v => color == null || v.Color.Contains(color))
                .Where(v => year == null || v.Year == year)
                .Where(v => model == null || v.VehicleModel.Name.Contains(model));
            return results;
        }
    }
}
