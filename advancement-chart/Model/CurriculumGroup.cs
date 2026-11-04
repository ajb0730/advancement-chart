using System;
using System.ComponentModel.DataAnnotations;

namespace advancementchart.Model
{
    /// <summary>
    /// Helpers for <see cref="CurriculumGroup"/>.
    /// </summary>
    public static class CurriculumGroupExtensions
    {
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<CurriculumGroup, string> displayNames =
            new System.Collections.Concurrent.ConcurrentDictionary<CurriculumGroup, string>();

        /// <summary>
        /// The group's display name (its <c>[Display(Name = ...)]</c> attribute, else the enum name). Cached after the first lookup.
        /// </summary>
        public static string GetDisplayName(this CurriculumGroup value)
        {
            return displayNames.GetOrAdd(value, ComputeDisplayName);
        }

        private static string ComputeDisplayName(CurriculumGroup value)
        {
            System.Reflection.FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DisplayAttribute[] descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }
    }

    /// <summary>
    /// The topics the Troop Guide handbook groups rank requirements into (Camping, First Aid, ...).
    /// <see cref="advancementchart.Reports.IndividualReport"/> uses them to show progress by topic rather than by rank.
    /// </summary>
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
