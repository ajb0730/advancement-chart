using System;
using System.Drawing;

namespace advancementchart.Model.Ranks
{
    public class FirstClass : Rank
    {
        public FirstClass(DateTime? earned = null)
            : base(name: "First Class", description: "", earned: earned, version: "2016")
        {
            Requirements.Add(new RankRequirement(name: "1a", description: "Participate in ten outdoor activities since joining", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "1b", description: "Explain Tread Lightly!", rank: this, handbookPages: "233-234"));
            Requirements.Add(new RankRequirement(name: "2a", description: "Plan patrol menu for breakfast, lunch and dinner", rank: this, handbookPages: "290-300,311-325"));
            Requirements.Add(new RankRequirement(name: "2b", description: "Make shopping list", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "2c", description: "Show gear needed to prepare meals", rank: this, handbookPages: "302-303,312-313,321,324"));
            Requirements.Add(new RankRequirement(name: "2d", description: "Demonstrate food and garbage handling procedures", rank: this, handbookPages: "300-301,306-309"));
            Requirements.Add(new RankRequirement(name: "2e", description: "Serve as head cook", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "3a", description: "Discuss use of lashings", rank: this, handbookPages: "359,372"));
            Requirements.Add(new RankRequirement(name: "3b", description: "Demonstrate tying timber hitch and clove hitch", rank: this, handbookPages: "367-368"));
            Requirements.Add(new RankRequirement(name: "3c", description: "Demonstrate square, sheer and diagonal lashings", rank: this, handbookPages: "373-376"));
            Requirements.Add(new RankRequirement(name: "3d", description: "Make a camp gadget using lashing", rank: this, handbookPages: "371-378"));
            Requirements.Add(new RankRequirement(name: "4a", description: "Complete orienteering course with measurements", rank: this, handbookPages: "328-354"));
            Requirements.Add(new RankRequirement(name: "4b", description: "Demonstrate how to use a GPS", rank: this, handbookPages: "345-355"));
            Requirements.Add(new RankRequirement(name: "5a", description: "Identify/show evidence of 10 native plants", rank: this, handbookPages: "188-199"));
            Requirements.Add(new RankRequirement(name: "5b", description: "Identify 2 ways to get weather forecast", rank: this, handbookPages: "212-218"));
            Requirements.Add(new RankRequirement(name: "5c", description: "Describe 3 natural indicators of hazardous weather", rank: this, handbookPages: "160,215-218"));
            Requirements.Add(new RankRequirement(name: "5d", description: "Describe and discuss extreme weather conditions", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "6a", description: "Pass BSA swimmer test", rank: this, handbookPages: "160-170"));
            Requirements.Add(new RankRequirement(name: "6b", description: "Tell precautions of safe trip afloat", rank: this, handbookPages: "172-174"));
            Requirements.Add(new RankRequirement(name: "6c", description: "Identify parts of a boat, canoe or kayak, and oar or paddle", rank: this, handbookPages: "174-175"));
            Requirements.Add(new RankRequirement(name: "6d", description: "Describe proper body position in watercraft", rank: this, handbookPages: "176"));
            Requirements.Add(new RankRequirement(name: "6e", description: "Demonstrate line rescue", rank: this, handbookPages: "177-180"));
            Requirements.Add(new RankRequirement(name: "7a", description: "Demonstrate bandages", rank: this, handbookPages: "142"));
            Requirements.Add(new RankRequirement(name: "7b", description: "Transport a victim", rank: this, handbookPages: "142,148-150"));
            Requirements.Add(new RankRequirement(name: "7c", description: "Tell five signs of heart attack and explain CPR", rank: this, handbookPages: "116-119"));
            Requirements.Add(new RankRequirement(name: "7d", description: "Describe potential utility hazards", rank: this, handbookPages: "154-155"));
            Requirements.Add(new RankRequirement(name: "7e", description: "Develop home emergency action plan", rank: this, handbookPages: "153-155"));
            Requirements.Add(new RankRequirement(name: "7f", description: "Explain how to obtain potable water in an emergency", rank: this, handbookPages: "240,294"));
            Requirements.Add(new RankRequirement(name: "8a", description: "Be physically active for four weeks after earning S7a", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "8b", description: "Set goal and make plan to remain physically active", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "9a", description: "Visit and discuss citizenship with community leader", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "9b", description: "Investigate environmental issue", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "9c", description: "Note garbage produced and reduce it", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "9d", description: "Participare in 3 hour service project", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "10", description: "Tell a friend about Scouts BSA", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "11", description: "Show Scout spirit", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "12", description: "Scoutmaster conference", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "13", description: "Board of Review", rank: this, handbookPages: ""));
            FillColor = ColorTranslator.FromHtml("#CE1126");
        }
    }
}
