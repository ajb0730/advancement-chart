# advancement-chart

This code will read the Advancement Backup/Export from
[Scoutbook](https://www.scoutbook.com) and generate an Excel-based advancement
poster, as well as an Individual Advancement program for each Scout under First
Class and a Trail to Eagle report for each Scout at or over First Class.

## INSTALLATION

1. Make sure you have [`dotnet`](https://docs.microsoft.com/en-us/dotnet/core/install/sdk?pivots=os-macos) installed.
1. Make sure you have the code checked out.
1. Fetch NuGet dependencies; while in the checked-out code directory: `dotnet restore`
1. Build the code; while in the checked-out code directory: `dotnet build`
1. Assuming the code built, you would execute it as: `dotnet /path/to/advancement-chart/advancement-chart/bin/Debug/netcoreapp2.0/advancement-chart.dll`

## RUNNING

1. As an Admin user of a Scouts BSA Troop on
[Scoutbook](https://www.scoutbook.com), navigate to the Troop page and click on
the `Export/Backup` link.
1. Choose `Scouts` and download the `.csv` file to your computer.  Rename this
file to `scouts.csv`. This "Scouts" file only needs to be re-downloaded when
names or patrols change, or when a Scout joins or leaves the Troop.
1. Also on the `Export/Backup` link, choose `Scout Advancement` and download
that `troop_#####__advancement.csv` file to your computer.
1. With the `scouts.csv` and the `troop_#####__advancement.csv` files in the same
directory, run the `advancement-chart` program: `dotnet /path/to/advancement-chart/advancement-chart/bin/Debug/netcoreapp2.0/advancement-chart.dll troop_#####__advancement.csv`.
1. This will create three files:
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

## AUTHOR

[Andrew Barnett](mailto:andrew@ajbarnett.com)

vim: tw=80
