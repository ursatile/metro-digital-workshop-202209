using Autobarn.Data.Entities;
using GraphQL.Types;

namespace Autobarn.Website.GraphQL.GraphTypes {
    public sealed class ModelGraphType : ObjectGraphType<Model> {
        public ModelGraphType() {
            Name = "Model";
            Field(m => m.Name).Description("the name of this model, eg. Beetle, Delorean");
            Field(m => m.Manufacturer, type: typeof(ManufacturerGraphType))
                .Description("The company who manufactures this model of car");
        }
    }
}   