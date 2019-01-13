const parse = require('csv-parse')
const fs = require('fs')
const util = require('util')

const people = {}

const updateRank = (id,rank,version,date) => {
	if(people.hasOwnProperty(id)) {
		if(people[id].ranks.hasOwnProperty(rank)) {
			people[id].ranks[rank].name = rank
			people[id].ranks[rank].version = version
			people[id].ranks[rank].earned = date
		} else {
			people[id].ranks[rank] = {"name": rank, "version": version, "earned": date, "requirements": {}}
		}
	} else {
		people[id] = {
			"ranks": {}
		}
		people[id].ranks[rank] = {"name": rank, "version": version, "earned": date, "requirements": {}}
	}
}

const updateRankRequirement = (id,rank,requirement,version,date) => {
	if(people.hasOwnProperty(id)) {
		if(people[id].ranks.hasOwnProperty(rank)) {
			if(!people[id].ranks[rank].hasOwnProperty('version')) {
				people[id].ranks[rank].version = version
			}
			if(people[id].ranks[rank].hasOwnProperty('requirements')) {
				people[id].ranks[rank].requirements[requirement] = date
			} else {
				people[id].ranks[rank].requirements = {}
				people[id].ranks[rank].requirements[requirement] = date
			}
		} else {
			people[id].ranks[rank] = {"requirements": {}, "version": version}
			people[id].ranks[rank].requirements[requirement] = date
		}
	} else {
		people[id] = {
			"ranks": {}
		}
		people[id].ranks[rank] = {"requirements": {}, "version": version}
		people[id].ranks[rank].requirements[requirement] = date
	}
}
const parser = parse({columns:true})
parser.on('readable',() => {
	let record;
	while(record = parser.read()) {
		let id = record['BSA Member ID']
		if(!people.hasOwnProperty(id)) {
			people[id] = {
				"firstName": record['First Name'],
				"middleName": record['Middle Name'],
				"lastName": record['Last Name'],
				"id": id,
				"ranks": {},
				"meritBadges": {}
			}
		}
		switch(record['Advancement Type']) {
			case "Academics & Sports Belt Loop":
			case "Adventure":
			case "Webelos Activity Badge":
			case "Webelos Adventure Requirement ":
			case "Award Requirement":
			case "Award":
				break
			case "Merit Badge":
				people[id].meritBadges[record['Advancement']] = {"name": record['Advancement'], "version": record['Version'], "earned": record['Date Completed']}
				break
			case "Merit Badge Requirement":
				break
			case "Rank":
				updateRank(id,record['Advancement'],record['Version'],record['Date Completed'])
				break
			case "Scout Rank Requirement":
				updateRankRequirement(id,'Scout',record['Advancement'],record['Version'],record['Date Completed'])
				break
			case "Tenderfoot Rank Requirement":
				updateRankRequirement(id,'Tenderfoot',record['Advancement'],record['Version'],record['Date Completed'])
				break
			case "Second Class Rank Requirement":
				updateRankRequirement(id,'Second Class',record['Advancement'],record['Version'],record['Date Completed'])
				break
			case "First Class Rank Requirement":
				updateRankRequirement(id,'First Class',record['Advancement'],record['Version'],record['Date Completed'])
				break
			case "Star Scout Rank Requirement":
				updateRankRequirement(id,'Star Scout',record['Advancement'],record['Version'],record['Date Completed'])
				break
			case "Life Scout Rank Requirement":
				updateRankRequirement(id,'Life Scout',record['Advancement'],record['Version'],record['Date Completed'])
				break
			case "Eagle Scout Rank Requirement":
				updateRankRequirement(id,'Eagle Scout',record['Advancement'],record['Version'],record['Date Completed'])
				break
			default:
				break
		}
	}
})
parser.on('end', () => {
	console.debug(util.inspect(people,{depth:500}))
})

for(let i = 2; i < process.argv.length; i++) {
	let arg = process.argv[i]
	if(fs.existsSync(arg)) {
		let data = fs.readFileSync(arg,{"encoding":"ascii"})
		parser.write(data)
	}
}
parser.end()
