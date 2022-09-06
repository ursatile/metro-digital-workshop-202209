using Autobarn.Data.Entities;
using GraphQL.Types;

namespace Autobarn.Website.GraphQL.GraphTypes {
    public sealed class VehicleGraphType : ObjectGraphType<Vehicle> {
        public VehicleGraphType() {
            Name = "vehicle";
            Field(v => v.Color).Description("What color is this car?");
            Field(v => v.Registration).Description("The registration of this vehicle");
            Field(v => v.Year).Description("The year this vehicle was registered");
            Field(v => v.VehicleModel, type: typeof(ModelGraphType)).Description("The model of vehicle");
        }
    }
}
