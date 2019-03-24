using System;
using System.Drawing;

namespace advancementchart.Model.Ranks
{
    public class Tenderfoot : Rank
    {
        public Tenderfoot(DateTime? earned = null)
            : base(name: "Tenderfoot", description: "", earned: earned, version: "2016")
        {
            Requirements.Add(new RankRequirement(name: "1a", description: "Prepare for campout", rank: this, handbookPages: "267-273"));
            Requirements.Add(new RankRequirement(name: "1b", description: "Spend one night on campout in tent", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "1c", description: "Tell how you practiced Outdoor Code on outing", rank: this, handbookPages: "223-224"));
            Requirements.Add(new RankRequirement(name: "2a", description: "Assist with cooking", rank: this, handbookPages: "301,304-305,307-309"));
            Requirements.Add(new RankRequirement(name: "2b", description: "Demonstrate safe meal utensil cleaning", rank: this, handbookPages: "307-309"));
            Requirements.Add(new RankRequirement(name: "2c", description: "Explain importance of eating as patrol", rank: this, handbookPages: "304-305"));
            Requirements.Add(new RankRequirement(name: "3a", description: "Demonstrate square knot", rank: this, handbookPages: "365,145"));
            Requirements.Add(new RankRequirement(name: "3b", description: "Demonstrate two half-hitches", rank: this, handbookPages: "366,369"));
            Requirements.Add(new RankRequirement(name: "3c", description: "Demonstrate taughtline hitch", rank: this, handbookPages: "367,369"));
            Requirements.Add(new RankRequirement(name: "3d", description: "Demonstrate proper case, sharpening, use of woods tools", rank: this, handbookPages: "380-381,386"));
            Requirements.Add(new RankRequirement(name: "4a", description: "Demonstrate First Aid skills", rank: this, handbookPages: "120-141"));
            Requirements.Add(new RankRequirement(name: "4b", description: "Describe and identify poisonous plants and treatment", rank: this, handbookPages: "127,191-192"));
            Requirements.Add(new RankRequirement(name: "4c", description: "Tell how to prevent occurences of 4a & 4b", rank: this, handbookPages: "125"));
            Requirements.Add(new RankRequirement(name: "4d", description: "Assemble personal first aid kit", rank: this, handbookPages: "108"));
            Requirements.Add(new RankRequirement(name: "5a", description: "Explain buddy system", rank: this, handbookPages: "29,160-161,172,404"));
            Requirements.Add(new RankRequirement(name: "5b", description: "Describe what to do if you get lost", rank: this, handbookPages: "253-255"));
            Requirements.Add(new RankRequirement(name: "5c", description: "Explain rules of safe hiking", rank: this, handbookPages: "253-255"));
            Requirements.Add(new RankRequirement(name: "6a", description: "Record your best at fitness test", rank: this, handbookPages: "77-81"));
            Requirements.Add(new RankRequirement(name: "6b", description: "Describe your plan to improve after 30 days", rank: this, handbookPages: "77-81"));
            Requirements.Add(new RankRequirement(name: "6c", description: "Show improvement at fitness test after 30 days", rank: this, handbookPages: "77-81"));
            Requirements.Add(new RankRequirement(name: "7a", description: "Demonstrate flag skills", rank: this, handbookPages: "56-60"));
            Requirements.Add(new RankRequirement(name: "7b", description: "Participate in 1 hour service project", rank: this, handbookPages: "68-69"));
            Requirements.Add(new RankRequirement(name: "8", description: "Describe and use EDGE training method", rank: this, handbookPages: "38,365"));
            Requirements.Add(new RankRequirement(name: "9", description: "Show Scout spirit", rank: this, handbookPages: "11-16"));
            Requirements.Add(new RankRequirement(name: "10", description: "Scoutmaster conference", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "11", description: "Board of Review", rank: this, handbookPages: ""));
            FillColor = ColorTranslator.FromHtml("#996633");
        }
    }
}
