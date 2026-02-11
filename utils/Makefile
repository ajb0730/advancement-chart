all : EagleReport.xlsx TroopAdvancementChart.xlsx IndividualReport.xlsx TroopGuideReport.xlsx AdvancementReport.docx
	cp -f $^ $$HOME/Dropbox/Scouts/Troop\ Committee/

scouts-merged.csv : troop_201_B__scouts.csv troop_201_G__scouts.csv
	head -n 1 $< > $@
	for i in $^; do tail -n +2 $$i >> $@; done

scouts.csv : scouts-merged.csv birthdates.csv
	head -n 1 scouts-merged.csv | tr -d '\r' | awk '{ printf("%s,\"DOB\"\n",$$0); }' > $@
	tail -n +2 scouts-merged.csv | tr -d '\r' | sort -t, -k 2 > one
	tail -n +2 birthdates.csv | tr -d '\r' | sort -t, -k 1 > two
	join -t, -1 2 -2 1 -o "$$(echo "$$(for i in $$(seq $$(printf '%d\n' "$$(head -n 1 scouts-merged.csv | tr ',' '\n' | wc -l)")); do printf '1.%d,' $$i; done)2.4")" one two >> $@
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

advancement.csv : troop_201_B__advancement.csv troop_201_G__advancement.csv
	head -n 1 $< > $@
	for i in $^; do tail -n +2 $$i >> $@; done

EagleReport.xlsx TroopAdvancementChart.xlsx IndividualReport.xlsx TroopGuideReport.xlsx AdvancementReport.docx : advancement.csv scouts.csv
	dotnet $$HOME/src/advancement-chart/advancement-chart/bin/Debug/net8.0/advancement-chart.dll $<
	cp -f $^ ~/Dropbox/Scouts/Troop\ Committee/

clean :
	rm -f advancement.csv troop_201_B__advancement.csv troop_201_G__advancement.csv

reallyclean : clean
	rm -f scouts.csv scouts-merged.csv troop_201_B__scouts.csv troop_201_G__scouts.csv

superclean : reallyclean
	rm -f birthdates.csv

