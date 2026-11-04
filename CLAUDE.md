# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

.NET 8 solution (`advancement-chart.sln`) with the app (`advancement-chart/`) and an xUnit test project (`advancement-chart.tests/`).

- Build: `dotnet build`
- Test all: `dotnet test --settings advancement-chart.tests/test.runsettings` (the runsettings sets `PANGOCAIRO_BACKEND=fontconfig`, needed by EPPlus/System.Drawing on macOS)
- Single test: `dotnet test --settings advancement-chart.tests/test.runsettings --filter "FullyQualifiedName~TroopMemberTest.SomeTest"`
- Run: the program takes one or more normalized advancement CSVs as args (`--patrol FILE` overrides the default `./scouts.csv` roster lookup; `--output-dir DIR` overrides the default of writing reports to the cwd). Normally run via `make` in `utils/` (see below), which invokes `~/src/advancement-chart/advancement-chart/bin/Debug/net8.0/advancement-chart.dll`, so build Debug first.

## Data pipeline

Input is the Scoutbook "Scout Advancement" and "Scouts" CSV exports. They can't be fed to the program directly: headers have spaces around commas, and dates of birth are absent. `utils/Makefile` normalizes the headers (`advancement.csv`, `scouts-merged.csv`), merges DOBs from the hand-maintained `utils/birthdates.csv` into `scouts.csv`, runs the DLL, and copies outputs to `~/Dropbox/Scouts/Troop Committee/`. Filenames (`Troop_0201_F_scouts.csv`, `Troop0201F_Advancement_*.csv`) are hardcoded in the Makefile. Exports, `scouts*.csv`, `birthdates.csv`, and generated reports contain PII and are gitignored — never commit them or paste their contents into fixtures.

## Architecture

- `Program.cs` orchestrates: `ParseArgs` builds `Options` (`--patrol`, `--output-dir`; `Execute` maps `UsageException` to exit 2 and `InputFileException` to exit 1, after validating every named input up front). `LoadFile` parses each advancement CSV into `TroopMember` objects (returning the max completion date), `LoadPatrolLookup` overlays patrol/nickname/DOB from `scouts.csv` (and seeds roster-only scouts with no advancement rows), then every report in `Reports/` is run in turn (all implement `IReport.Run(path)`; `AdvancementCheck` is a console-only validator, `AdvancementReport` produces `.docx` via OpenXml, the others `.xlsx` via EPPlus).
- `LoadFile` has to handle both the **legacy and the 2026 Scoutbook export formats** (e.g. `" Rank"`/`" MB"` suffixes, semicolons replacing commas in badge names, requirement number moved to the Advancement column, "Award Requirements" rows ignored, Cub ranks resolving to null and skipped). Format quirks are documented in comments there; extend those branches rather than assuming one format.
- `Model/`: `TroopMember` owns a `Rank` per rank level (`Model/Ranks/*`: Scout → … → Eagle, Palm) plus merit badges. Ranks hold `RankRequirement`s; merit badges hold `MeritBadgeRequirement`s (`EagleMeritBadgeRequirement` for the Eagle-required set). `CurriculumGroup` maps rank requirements onto the Troop Guide curriculum used by `IndividualReport` and `TroopGuideReport`. `CellAddress` is a helper for Excel cell references.
- Null conventions: use `is null` / `is not null` (no Yoda `null == x`); treat optional strings with `string.IsNullOrWhiteSpace`; use `TroopMember.PreferredFirstName` rather than re-deriving nickname-or-first-name.
- Tests mirror the source layout (`Model/`, `Reports/`); `Helpers/` holds fixtures and a native library resolver for EPPlus drawing dependencies. `Program` internals are exposed to tests via `InternalsVisibleTo`.
