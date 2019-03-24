using System;
using System.ComponentModel.DataAnnotations;

namespace advancementchart.Model
{
    public enum CurriculumGroup
    {
        [Display(Name="Camping I")]
        Camping1,
        [Display(Name="Camping II")]
        Camping2,
        Citizenship,
        Cooking,
        Emergencies,
        [Display(Name = "Fires and Fire Safety")]
        FiresAndFireSafety,
        [Display(Name = "First Aid : Basics I")]
        FirstAidBasics1,
        [Display(Name = "First Aid : Basics II")]
        FirstAidBasics2,
        [Display(Name = "First Aid : Bandages")]
        FirstAidBandages,
        [Display(Name = "First Aid : CPR")]
        FirstAidCPR,
        [Display(Name = "First Aid : Hurry Cases")]
        FirstAidHurryCases,
        [Display(Name = "First Aid : Rescues")]
        FirstAidRescues,
        [Display(Name = "Fitness I")]
        Fitness1,
        [Display(Name = "Fitness II")]
        Fitness2,
        [Display(Name = "Forming the Patrol")]
        FormingThePatrol,
        [Display(Name = "Knots and Lashings I")]
        KnotsAndLashings1,
        [Display(Name = "Knots and Lashings II")]
        KnotsAndLashings2,
        [Display(Name = "Outdoor Ethics")]
        OutdoorEthics,
        [Display(Name = "Map and Compass I")]
        MapAndCompass1,
        [Display(Name = "Map and Compass II")]
        MapAndCompass2,
        [Display(Name = "Map and Compass III")]
        MapAndCompass3,
        [Display(Name = "Nature I")]
        Nature1,
        [Display(Name = "Nature II")]
        Nature2,
        [Display(Name = "Water Safety")]
        WaterSafety,
        [Display(Name = "Totin' Chip")]
        TotinChip
    }
}
