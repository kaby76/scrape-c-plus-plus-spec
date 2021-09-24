test: clean
	java -jar 'c:/Users/kenne/Downloads/antlr-4.9.2-complete.jar' Save.g4

csharp: clean
	cp ScrapeLexer.g4 test
	cp ScrapeParser.g4 test
	cd test; rm -rf Generated; trgen -s translation_unit; cd Generated; make

clean:
	rm -rf test/Generated scraper/bin scraper/obj

build:
	bash trash.sh

