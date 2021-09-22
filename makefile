build:
	bash trash.sh

test: clean
	cp Scrape.g4 test
	cd test; trgen -s translation_unit; cd Generated; make

clean:
	rm -rf test/Generated scraper/bin scraper/obj
