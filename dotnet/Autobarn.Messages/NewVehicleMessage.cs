using System;

namespace Autobarn.Messages {
    public class NewVehicleMessage {
        public string Registration { get; set; }
        public string Color { get; set; }
        public string ManufacturerName { get; set; }
        public string ModelName { get; set; }
        public int Year { get; set; }
        public DateTimeOffset ListedAt { get; set; }

        public override string ToString() {
            return $"{Registration},{Color},{Year},{ManufacturerName},{ModelName},{ListedAt:O}";
        }
    }

    public class NewVehiclePriceMessage : NewVehicleMessage {
        public int Price { get; set; }
        public string Currency { get; set; }

        public override string ToString() {
            return $"{base.ToString()} - COST {Price}{Currency}";
        }
    }
}
