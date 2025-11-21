using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class PolicyMotorcycle
        {
            public const string SHIELD_API_MOTORCYCLE_NOT_MODIFIED = "N";

            /// <summary>
            /// These are UI labels applied to covers relevant for motorcycle insurance
            /// </summary>
            public readonly static IReadOnlyDictionary<PolicyMotor.MotorCovers, IdDescriptions> MotorcycleCoverNameMappings = new Dictionary<PolicyMotor.MotorCovers, IdDescriptions>()
            {
                { PolicyMotor.MotorCovers.MFCO, new IdDescriptions() { TextB2C = "Comprehensive",               TextShield = "Full Cover" } },
                { PolicyMotor.MotorCovers.TFT,  new IdDescriptions() { TextB2C = "Third party fire and theft",  TextShield = "TPF&T" } },
                { PolicyMotor.MotorCovers.TPO,  new IdDescriptions() { TextB2C = "Third party property damage", TextShield = "TPPD" } }
            };

            public enum MotorcycleUsage
            {
                Private,
                FoodDelivery,
                CourierServices,
                HiringOut
            }

            /// <summary>
            /// Supports the MotorcycleBuilder to pick usages which are supported
            /// and will not result in declined cover during new business flows.
            /// </summary>
            public enum MotorcycleUsageValid
            {
                Private = MotorcycleUsage.Private,
                FoodDelivery = MotorcycleUsage.FoodDelivery,
                CourierServices = MotorcycleUsage.CourierServices
            }

            public readonly static IReadOnlyDictionary<MotorcycleUsage, IdDescriptions> MotorcycleUsageMappings = new Dictionary<MotorcycleUsage, IdDescriptions>()
            {
                { MotorcycleUsage.Private,          new IdDescriptions() { TextB2C = "Private",          TextShield = "private" } },
                { MotorcycleUsage.FoodDelivery,     new IdDescriptions() { TextB2C = "Food delivery",    TextShield = "fooddelivery" } },
                { MotorcycleUsage.CourierServices,  new IdDescriptions() { TextB2C = "Courier services", TextShield = "courierservices" } },
                { MotorcycleUsage.HiringOut,        new IdDescriptions() { TextB2C = "Hiring out",       TextShield = "hiringout" } }
            };
        }
    }
}
