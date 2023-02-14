using System;
using System.Drawing;
using System.Linq;

namespace advancementchart.Model.Ranks
{
    public class FirstClass : Rank
    {
        public FirstClass(DateTime? earned = null) :
            base(
                name: "First Class",
                description: "",
                earned: earned,
                version: "2016"
            )
        {
            FillColor = ColorTranslator.FromHtml("#CE1126");
        }

        public override void SwitchVersion(string version)
        {
            this.Version = version;
            if (Requirements.Any())
            {
                Requirements.Clear();
            }
            switch (version)
            {
                case "2016":
                    Requirements
                        .Add(new RankRequirement(name: "1a",
                            tag: "Participate in ten outdoor activities since joining",
                            description: "Since joining, participate in 10 separate troop/patrol activities, six of which include overnight camping. These 10 activities do not include troop or patrol meetings. On at least five of the six campouts, spend the night in a tent that you pitch or other structure that you help erect. (such as a lean-to, snow cave, or tepee.)",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1b",
                            tag: "Explain Tread Lightly!",
                            description: "Explain each of the principles of Tread Lightly! and tell how you practiced them while on a campout or outing. This outing must be different from the ones used for Tenderfoot requirement 1c and Second Class requirement 1b.",
                            rank: this,
                            handbookPages: "233-234",
                            curriculumGroup: CurriculumGroup.OutdoorEthics,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2a",
                            tag: "Plan patrol menu for breakfast, lunch and dinner",
                            description: "Help plan a menu for one of the above campouts that includes at least one breakfast, one lunch, and one dinner and that requires cooking at least two of the meals. Tell how the menu includes the foods from MyPlate or the current USDA nutritional model and how it meets nutritional needs for the planned activity or campout.",
                            rank: this,
                            handbookPages: "290-300,311-325",
                            curriculumGroup: CurriculumGroup.Cooking,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2b",
                            tag: "Make shopping list",
                            description: "Using the menu planned in First Class requirement 2a, make a list showing a budget and the food amounts needed to feed three or more boys. Secure the ingredients.",
                            rank: this,
                            handbookPages: "",
                            curriculumGroup: CurriculumGroup.Cooking,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2c",
                            tag: "Show gear needed to prepare meals",
                            description: "Show which pans, utensils, and other gear will be needed to cook and serve these meals.",
                            rank: this,
                            handbookPages: "302-303,312-313,321,324",
                            curriculumGroup: CurriculumGroup.Cooking,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2d",
                            tag: "Demonstrate food and garbage handling procedures",
                            description: "Demonstrate the procedures to follow in the safe handling and storage of fresh meats, dairy products, eggs, vegetables, and other perishable food products. Show how to properly dispose of camp garbage, cans, plastic containers, and other rubbish.",
                            rank: this,
                            handbookPages: "300-301,306-309",
                            curriculumGroup: CurriculumGroup.Cooking,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2e",
                            tag: "Serve as head cook",
                            description: "On one campout, serve as cook. Supervise your assistant(s) in using a stove or building a cooking fire. Prepare the breakfast, lunch, and dinner planned in First Class requirement 2a. Supervise the cleanup.",
                            rank: this,
                            handbookPages: "",
                            curriculumGroup: CurriculumGroup.Cooking,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3a",
                            tag: "Discuss use of lashings",
                            description: "Discuss when you should and should not use lashings.",
                            rank: this,
                            handbookPages: "359,372",
                            curriculumGroup: CurriculumGroup
                                .KnotsAndLashings2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3b",
                            tag: "Demonstrate tying timber hitch and clove hitch",
                            description: "Demonstrate tying the timber hitch and clove hitch.",
                            rank: this,
                            handbookPages: "367-368",
                            curriculumGroup: CurriculumGroup
                                .KnotsAndLashings2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3c",
                            tag: "Demonstrate square, sheer and diagonal lashings",
                            description: "Demonstrate tying the square, shear, and diagonal lashings by joining two or more poles or staves together.",
                            rank: this,
                            handbookPages: "373-376",
                            curriculumGroup: CurriculumGroup
                                .KnotsAndLashings2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3d",
                            tag: "Make a camp gadget using lashing",
                            description: "Use lashings to make a useful camp gadget or structure.",
                            rank: this,
                            handbookPages: "371-378",
                            curriculumGroup: CurriculumGroup
                                .KnotsAndLashings2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4a",
                            tag: "Complete orienteering course with measurements",
                            description: "Using a map and compass, complete an orienteering course that covers at least one mile and requires measuring the height and/or width of designated items (tree, tower, canyon, ditch, etc.)",
                            rank: this,
                            handbookPages: "328-354",
                            curriculumGroup: CurriculumGroup.MapAndCompass2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4b",
                            tag: "Demonstrate how to use a GPS",
                            description: "Demonstrate how to use a handheld GPS unit, GPS app on a smartphone or other electronic navigation system. Use a GPS to find your current location, a destination of your choice, and the route you will take to get there. Follow that route to arrive at your destination.",
                            rank: this,
                            handbookPages: "345-355",
                            curriculumGroup: CurriculumGroup.MapAndCompass1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5a",
                            tag: "Identify/show evidence of 10 native plants",
                            description: "Identify or show evidence of at least 10 kinds of native plants found in your local area or campsite location. You may show evidence by identifying fallen leaves or fallen fruit that you find in the field, or as part of a collection you have made, or by photographs you have taken.",
                            rank: this,
                            handbookPages: "188-199",
                            curriculumGroup: CurriculumGroup.Nature2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5b",
                            tag: "Identify 2 ways to get weather forecast",
                            description: "Identify two ways to obtain a weather forecast for an upcoming activity. Explain why weather forecasts are important when planning for an event.",
                            rank: this,
                            handbookPages: "212-218",
                            curriculumGroup: CurriculumGroup.Emergencies,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5c",
                            tag: "Describe 3 natural indicators of hazardous weather",
                            description: "Describe at least three natural indicators of impending hazardous weather, the potential dangerous events that might result from such weather conditions, and the appropriate actions to take.",
                            rank: this,
                            handbookPages: "160,215-218",
                            curriculumGroup: CurriculumGroup.Emergencies,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5d",
                            tag: "Describe and discuss extreme weather conditions",
                            description: "Describe extreme weather conditions you might encounter in the outdoors in your local geographic area. Discuss how you would determine ahead of time the potential risk of these types of weather dangers, alternative planning considerations to avoid such risks, and how you would prepare for and respond to those weather conditions.",
                            rank: this,
                            handbookPages: "",
                            curriculumGroup: CurriculumGroup.Emergencies,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6a",
                            tag: "Pass BSA swimmer test",
                            description: "Successfully complete the BSA swimmer test.",
                            rank: this,
                            handbookPages: "160-170",
                            curriculumGroup: CurriculumGroup.WaterSafety,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6b",
                            tag: "Tell precautions of safe trip afloat",
                            description: "Tell what precautions must be taken for a safe trip afloat.",
                            rank: this,
                            handbookPages: "172-174",
                            curriculumGroup: CurriculumGroup.WaterSafety,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6c",
                            tag: "Identify parts of a boat, canoe or kayak, and oar or paddle",
                            description: "Identify the basic parts of a canoe, kayak, or other boat. Identify the parts of a paddle or an oar.",
                            rank: this,
                            handbookPages: "174-175",
                            curriculumGroup: CurriculumGroup.WaterSafety,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6d",
                            tag: "Describe proper body position in watercraft",
                            description: "Describe proper body positioning in a watercraft, depending on the type and size of the vessel. Explain the importance of proper body position in the boat.",
                            rank: this,
                            handbookPages: "176",
                            curriculumGroup: CurriculumGroup.WaterSafety,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6e",
                            tag: "Demonstrate line rescue",
                            description: "With a helper and a practice victim, show a line rescue both as tender and rescuer. (The practice victim should be approximately 30 feet from shore in deep water.)",
                            rank: this,
                            handbookPages: "177-180",
                            curriculumGroup: CurriculumGroup.WaterSafety,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7a",
                            tag: "Demonstrate bandages",
                            description: "Demonstrate bandages for a sprained ankle and for injuries on the head, the upper arm, and the collarbone.",
                            rank: this,
                            handbookPages: "142",
                            curriculumGroup: CurriculumGroup.FirstAidBandages,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7b",
                            tag: "Transport a victim",
                            description: "By yourself and with a partner, show how to: Transport a person from a smoke-filled room, Transport for at least 25 yards a person with a sprained ankle.",
                            rank: this,
                            handbookPages: "142,148-150",
                            curriculumGroup: CurriculumGroup.FirstAidRescues,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7c",
                            tag: "Tell five signs of heart attack and explain CPR",
                            description: "Tell the five most common signals of a heart attack. Explain the steps (procedures) in cardiopulmonary resuscitation (CPR).",
                            rank: this,
                            handbookPages: "116-119",
                            curriculumGroup: CurriculumGroup.FirstAidCPR,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7d",
                            tag: "Describe potential utility hazards",
                            description: "Tell what utility services exist in your home or meeting place. Describe potential hazards associated with these utilities, and tell how to respond in emergency situations.",
                            rank: this,
                            handbookPages: "154-155",
                            curriculumGroup: CurriculumGroup.Emergencies,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7e",
                            tag: "Develop home emergency action plan",
                            description: "Develop an emergency action plan for your home that includes what to do in case of fire, storm, power outage, and water outage.",
                            rank: this,
                            handbookPages: "153-155",
                            curriculumGroup: CurriculumGroup.Emergencies,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7f",
                            tag: "Explain how to obtain potable water in an emergency",
                            description: "Explain how to obtain potable water in an emergency.",
                            rank: this,
                            handbookPages: "240,294",
                            curriculumGroup: CurriculumGroup.Emergencies,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "8a",
                            tag: "Be physically active for four weeks after earning S7a",
                            description: "After completing Second Class requirement 7a, be physically active at least 30 minutes every day for five days a week for four weeks. Keep track of your activities.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "8b",
                            tag: "Set goal and make plan to remain physically active",
                            description: "Share your challenges and successes in completing First Class requirement 8a. Set a goal for continuing to include physical activity as part of your daily life and develop a plan for doing so.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "9a",
                            tag: "Visit and discuss citizenship with community leader",
                            description: "Visit and discuss with a selected individual approved by your leader (for example, an elected official, judge, attorney, civil servant, principal, or teacher) the constitutional rights and obligations of a U.S. citizen.",
                            rank: this,
                            handbookPages: "",
                            curriculumGroup: CurriculumGroup.Citizenship,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "9b",
                            tag: "Investigate environmental issue",
                            description: "Investigate an environmental issue affecting your community. Share what you learned about that issue with your patrol or troop. Tell what, if anything, could be done by you or your community to address the concern.",
                            rank: this,
                            handbookPages: "",
                            curriculumGroup: CurriculumGroup.Citizenship,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "9c",
                            tag: "Note garbage produced and reduce it",
                            description: "On a Scouting or family outing, take note of the trash and garbage you produce. Before your next similar outing, decide how you can reduce, recycle, or repurpose what you take on that outing, and then put those plans into action. Compare your results.",
                            rank: this,
                            handbookPages: "",
                            curriculumGroup: CurriculumGroup.OutdoorEthics,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "9d",
                            tag: "Participate in 3 hour service project",
                            description: "Participate in three hours of service through one or more service projects approved by your Scoutmaster. The project(s) must not be the same service project(s) used for Tenderfoot requirement 7b and Second Class requirement 8e. Explain how your service to others relates to the Scout Law.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "10",
                            tag: "Tell a friend about Scouts BSA",
                            description: "Tell someone who is eligible to join Boy Scouts, or an inactive Boy Scout, about your Scouting activities. Invite him to an outing, activity, service project or meeting. Tell him how to join, or encourage the inactive Boy Scout to become active. Share your efforts with your Scoutmaster or other adult leader.",
                            rank: this,
                            handbookPages: "",
                            curriculumGroup: CurriculumGroup.Citizenship,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "11",
                            tag: "Show Scout spirit",
                            description: "Demonstrate scout spirit by living the Scout Oath and Scout Law. Tell how you have done your duty to God and how you have lived four different points of the Scout Law (different from those points used for previous ranks) in your everyday life.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "12",
                            tag: "Scoutmaster conference",
                            description: "While working toward the First Class rank, and after completing Second Class requirement 11, participate in a Scoutmaster conference.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "13",
                            tag: "Board of Review",
                            description: "Successfully complete your board of review for the First Class rank.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    break;
                default:
                    throw new ArgumentException($"Version {version} not recognized");
            }
        }
    }
}
