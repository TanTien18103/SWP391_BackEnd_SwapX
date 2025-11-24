using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesHelpers.VehicleBattery
{
    public static class VehicleBatteryMapping
    {
        public static readonly Dictionary<string, BatteryRequirement> Map = new()
    {
        // I6
        { VehicleNameEnums.YADEA_I6_Lithium_Battery.ToString(), new() { Specification = BatterySpecificationEnums.V48_Ah13.ToString(), BatteryType = BatteryTypeEnums.Lithium.ToString() }},
        { VehicleNameEnums.YADEA_I6_Accumulator.ToString(),     new() { Specification = BatterySpecificationEnums.V48_Ah13.ToString(), BatteryType = BatteryTypeEnums.Accumulator.ToString() }},

        // I8 
        { VehicleNameEnums.YADEA_I8.ToString(),                 new() { Specification = BatterySpecificationEnums.V48_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_I8_VINTAGE.ToString(),         new() { Specification = BatterySpecificationEnums.V48_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},

        // IFUN - IGO
        { VehicleNameEnums.YADEA_IFUN.ToString(),               new() { Specification = BatterySpecificationEnums.V48_Ah12.ToString(), BatteryType = BatteryTypeEnums.Lithium.ToString() }},
        { VehicleNameEnums.YADEA_IGO.ToString(),                new() { Specification = BatterySpecificationEnums.V48_Ah12.ToString(), BatteryType = BatteryTypeEnums.Lithium.ToString() }},

        // VITO
        { VehicleNameEnums.YADEA_VITO.ToString(),               new() { Specification = BatterySpecificationEnums.V36_Ah10_4.ToString(), BatteryType = BatteryTypeEnums.Lithium.ToString() }},

        // FLIT
        { VehicleNameEnums.YADEA_FLIT.ToString(),               new() { Specification = BatterySpecificationEnums.V36_Ah7_8.ToString(), BatteryType = BatteryTypeEnums.Lithium.ToString() }},

        // VELAX
        { VehicleNameEnums.YADEA_VELAX.ToString(),              new() { Specification = BatterySpecificationEnums.V72_Ah30.ToString(), BatteryType = BatteryTypeEnums.LFP.ToString() }},
        { VehicleNameEnums.YADEA_VELAX_SOOBIN.ToString(),       new() { Specification = BatterySpecificationEnums.V72_Ah30.ToString(), BatteryType = BatteryTypeEnums.LFP.ToString() }},

        // VOLTGUARD
        { VehicleNameEnums.YADEA_VOLTGUARD_U.ToString(),        new() { Specification = BatterySpecificationEnums.V72_Ah50.ToString(), BatteryType = BatteryTypeEnums.LFP.ToString() }},
        { VehicleNameEnums.YADEA_VOLTGUARD_P.ToString(),        new() { Specification = BatterySpecificationEnums.V72_Ah38.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},

        // ORLA / OCEAN / ODORA / M6I / VIGOR / XMEN_NEO
        { VehicleNameEnums.YADEA_ORLA_P.ToString(),             new() { Specification = BatterySpecificationEnums.V60_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_OCEAN.ToString(),              new() { Specification = BatterySpecificationEnums.V60_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_ODORA_S.ToString(),            new() { Specification = BatterySpecificationEnums.V60_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_ODORA_S2.ToString(),           new() { Specification = BatterySpecificationEnums.V60_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_M6I.ToString(),                new() { Specification = BatterySpecificationEnums.V60_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_VIGOR.ToString(),              new() { Specification = BatterySpecificationEnums.V60_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_X_MEN_NEO.ToString(),          new() { Specification = BatterySpecificationEnums.V60_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},

        // ORIS / OSSY
        { VehicleNameEnums.YADEA_ORIS.ToString(),               new() { Specification = BatterySpecificationEnums.V72_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_ORIS_SOOBIN.ToString(),        new() { Specification = BatterySpecificationEnums.V72_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_OSSY.ToString(),               new() { Specification = BatterySpecificationEnums.V72_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},

        // ICUTE / X-ZONE / VEKOO / X-SKY / X-BULL
        { VehicleNameEnums.YADEA_ICUTE.ToString(),              new() { Specification = BatterySpecificationEnums.V48_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_X_ZONE.ToString(),             new() { Specification = BatterySpecificationEnums.V48_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_VEKOO.ToString(),              new() { Specification = BatterySpecificationEnums.V48_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_VEKOO_SOOBIN.ToString(),       new() { Specification = BatterySpecificationEnums.V48_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_X_SKY.ToString(),              new() { Specification = BatterySpecificationEnums.V48_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }},
        { VehicleNameEnums.YADEA_X_BULL.ToString(),             new() { Specification = BatterySpecificationEnums.V48_Ah22.ToString(), BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString() }}
    };
    }

}
