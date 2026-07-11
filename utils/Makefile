all : EagleReport.xlsx TroopAdvancementChart.xlsx IndividualReport.xlsx TroopGuideReport.xlsx TroopCheckList.xlsx AdvancementReport.docx
	cp -f $^ $$HOME/Dropbox/Scouts/Troop\ Committee/

scouts-merged.csv : Troop_0201_F_scouts.csv
	tr -d '\r' < $< | awk 'NR==1 {gsub(/ *, */, ","); gsub(/^ +/, ""); gsub(/ +$$/, ""); print; next} {print}' > $@

scouts.csv : scouts-merged.csv birthdates.csv
	head -n 1 scouts-merged.csv | tr -d '\r' | awk '{ printf("%s,\"DOB\"\n", $$0); }' > $@
	tail -n +2 scouts-merged.csv | tr -d '\r' | awk 'NF' | sort -t, -k 2 > one
	tail -n +2 birthdates.csv | tr -d '\r' | awk 'NF' | sort -t, -k 1 > two
	join -t, -a 1 -e '' -1 2 -2 1 -o '1.1,1.2,1.3,1.4,1.5,1.6,1.7,1.8,1.9,1.10,1.11,1.12,1.13,1.14,1.15,1.16,1.17,1.18,1.19,1.20,1.21,2.4' one two >> $@
	rm -f one two

birthdates.csv : scouts-merged.csv
	[ -s $@ ] && cp -f $@ $@.bak
	head -n 1 $< | awk -F, '{ printf("%s,%s,%s,\"DOB\"\n", $$2, $$4, $$3); }' > $@
	tail -n +2 $< | awk -F, '{ printf("%s,%s,%s,\"\"\n", $$2, $$4, $$3); }' >> $@
	if [ -s $@.bak ]; then \
	  tail -n +2 $@ | tr -d '\r' | sort -t, -k 1 > one; \
	  tail -n +2 $@.bak | tr -d '\r' | sort -t, -k 1 > two; \
	  (head -n 1 $@; join -a 1 -t, -1 1 -2 1 -o 1.1,1.2,1.3,2.4 one two) > three; \
	  mv -f three $@; \
	  rm -f one two; \
	fi

advancement.csv: Troop0201F_Advancement_*.csv
	tr -d '\r' < $< | awk 'NR==1 {gsub(/ *, */, ","); gsub(/^ +/, ""); gsub(/ +$$/, ""); print; next} {print}' > $@

EagleReport.xlsx TroopAdvancementChart.xlsx IndividualReport.xlsx TroopGuideReport.xlsx TroopCheckList.xlsx AdvancementReport.docx : advancement.csv scouts.csv
	dotnet $$HOME/src/advancement-chart/advancement-chart/bin/Debug/net8.0/advancement-chart.dll $<
	cp -f $^ ~/Dropbox/Scouts/Troop\ Committee/

clean :
	rm -f advancement.csv Troop0201F_Advancement_*.csv

reallyclean : clean
	rm -f scouts.csv scouts-merged.csv Troop_0201_F_scouts.csv

superclean : reallyclean
	rm -f birthdates.csv

