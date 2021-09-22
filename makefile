build:
	bash trash.sh

test: clean
	java -jar 'c:/Users/kenne/Downloads/antlr-4.9.2-complete.jar' scrape.g4

clean:
	rm -f *.interp *.tokens *.java
