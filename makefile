build:
	bash trash.sh

test: clean
	cp ScrapeLexer.g4 test
	cp ScrapeParser.g4 test
	cd test; rm -rf Generated; trgen -s translation_unit; cd Generated; make

clean:
	rm -rf test/Generated scraper/bin scraper/obj
