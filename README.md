# advancement-chart

This code will read the Advancement Backup/Export from
[Scoutbook](https://www.scoutbook.com) and generate an Excel-based advancement
poster, an Individual Advancement program for each Scout under First Class, a
Trail to Eagle report for each Scout at or over First Class, a Troop Guide
report, a printable Troop check list, and a Word-based Advancement report.

## INSTALLATION

1. Make sure you have [`dotnet`](https://docs.microsoft.com/en-us/dotnet/core/install/sdk?pivots=os-macos) installed.
1. Make sure you have the code checked out.
1. Fetch NuGet dependencies; while in the checked-out code directory: `dotnet restore`
1. Build the code; while in the checked-out code directory: `dotnet build`
1. Assuming the code built, you would execute it as: `dotnet /path/to/advancement-chart/advancement-chart/bin/Debug/net8.0/advancement-chart.dll`

## RUNNING

1. As an Admin user of a Scouts BSA Troop on
[Scoutbook](https://www.scoutbook.com), navigate to the Troop page and click on
the `Export/Backup` link.
1. Choose `Scouts` and download the `.csv` file to your computer. Scoutbook
names this file after your unit, e.g. `Troop_0201_F_scouts.csv`. This "Scouts"
file only needs to be re-downloaded when names or patrols change, or when a
Scout joins or leaves the Troop.
1. Also on the `Export/Backup` link, choose `Scout Advancement` and download
that file to your computer. Scoutbook names it after your unit and the export
date, e.g. `Troop0201F_Advancement_20260711.csv`.
1. These two exports do **not** contain dates of birth, which the Eagle report
needs to calculate merit-badge deadlines. Dates of birth are maintained
separately in `birthdates.csv` (see [below](#generating-the-reports-with-the-utils-makefile)).
1. The exports also cannot be fed to the program directly: their header rows
contain spaces after each comma, dates are in `MM/DD/YYYY` format, and the
program expects a normalized `advancement.csv` alongside a `scouts.csv` that has
had the dates of birth merged in. The `utils/Makefile` performs all of this
normalization and merging for you and is the recommended way to run the program;
see [below](#generating-the-reports-with-the-utils-makefile).
1. Running the program produces the following files:
   1. TroopAdvancementChart.xlsx - this is an Excel workbook with three
   worksheets:
      1. `Troop Advancement` shows rank advancement up to, and including First
	  Class.
	  1. `Scout Advancement` shows rank advancement from Star up to Silver Palm.
	  1. `Merit Badges` shows a table of Merit Badge codes used throughout the
	  workbook.
   1. IndividualReport.xlsx - this is an Excel workbook with a worksheet for the
   Troop, each Patrol, and each Scout First Class and under. It shows rank
   requirements grouped by the curriculum in the [Troop Guide Handbook](http://boyscouttrail.com/docs/2016troopguide.pdf).
   1. EagleReport.xlsx - this is an Excel workbook with a worksheet for each
   Scout at or above First Class. It shows what merit badges the Scout needs,
   and for the longer badges, the last date the Scout may begin that badge
   (based on birthdate).
   1. TroopGuideReport.xlsx - this is an Excel workbook that summarizes each
   Scout's progress through the Troop Guide curriculum groups.
   1. TroopCheckList.xlsx - this is a landscape Excel workbook listing Scouts
   by Patrol (alphabetically), with empty checkbox columns for use as a
   printable attendance or activity checklist.
   1. AdvancementReport.docx - this is a Word document summarizing the
   advancement earned since the most recent completion date in the export.

## TIPS

### Printing the Advancement Chart

#### Microsoft Excel for Mac, version 16+

*Each* time you generate a new `TroopAdvancementChart.xlsx` file that you want
to print from, you will need to do the following:

1. Navigate to `File` | `Page Setup...` | `Sheet`.
1. Under `Print Titles`,
   1. Set `Rows to repeat at top` to: `$1:$3`
   1. Set `Columns to repeat at left` to: `$A:$A`
   1. Click Ok.
1. Then, use the mouse to highlight (draw a box around) all of the columns of a
single rank and all of the rows of a single patrol.
1. Then click `File` | `Print Area >` | `Set Print Area`
1. Hit Command-P to bring up the Print dialog.
1. Choose `Landscape` orientation and select `Scale to fit` with `1 pages wide
by 1 pages tall` (this may require you to select `Excel` from the drop-down).
1. Proceed to print, then highlighting a new rank + patrol and repeating the
`Set Print Area` and hitting Command-P.

### Simplify the command-line

#### MacOS

Set up a helper script in `/usr/local/sbin/advancement-chart` with the following
content:

```sh
#!/usr/bin/env bash

if [ ! -f "${1}" ]; then
	echo
	echo "USAGE: ${0} export-file.csv"
	echo
	exit 1
fi
dotnet /path/to/advancement-chart/advancement-chart/bin/Debug/net8.0/advancement-chart.dll ${1}
```

Don't forget to set that script as executable: `chmod 755 /usr/local/sbin/advancement-chart`

Now you should be able to run the program as just `advancement-chart`, without
the extra `dotnet /path/to/advancement-chart...` stuff.

### Generating the reports with the utils Makefile

The recommended workflow is the `Makefile` in the [`utils/`](utils/) directory.
It normalizes the raw Scoutbook exports (stripping the spaces the exports put
around header commas), merges in dates of birth, runs the program, and copies
the resulting reports to `~/Dropbox/Scouts/Troop Committee/`.

1. Download the two Scoutbook exports as described under
[RUNNING](#running) and place them in `utils/`. By default the Makefile expects
`Troop_0201_F_scouts.csv` (the Scouts export) and a
`Troop0201F_Advancement_*.csv` (the Scout Advancement export). Edit the
filenames at the top of the recipes if your unit's exports are named
differently.
1. Maintain dates of birth in `birthdates.csv`. Scoutbook does not export them,
so they are kept here. Running `make birthdates.csv` (re)builds the roster from
the current Scouts export while preserving any dates of birth you have already
entered — new Scouts are added with a blank `DOB` for you to fill in. The Eagle
report's deadline calculations will be incorrect for any Scout whose `DOB` is
blank.
1. Run `make` (or `make all`) to build every report. Intermediate files
(`scouts-merged.csv`, `scouts.csv`, and `advancement.csv`) are produced along
the way.

Useful targets:

- `make` / `make all` - build all reports and copy them to Dropbox.
- `make clean` - remove `advancement.csv` and the advancement export.
- `make reallyclean` - also remove `scouts.csv`, `scouts-merged.csv`, and the
  Scouts export.
- `make superclean` - also remove `birthdates.csv` (this discards your dates of
  birth — use with care).

The raw Scoutbook exports and generated reports contain personally identifiable
information and are excluded from version control via `.gitignore`.

## AUTHOR

[Andrew Barnett](https://github.com/ajb0730)

vim: tw=80
