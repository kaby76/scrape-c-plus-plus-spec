build:
	bash trash.sh

test: clean
	java -jar 'c:/Users/kenne/Downloads/antlr-4.9.2-complete.jar' c_plus_plus_spec_draft.g4

clean:
	rm -f c_plus_plus_spec_draft.interp c_plus_plus_spec_draft.tokens c_plus_plus_spec_draftBaseListener.java c_plus_plus_spec_draftLexer.interp c_plus_plus_spec_draftLexer.java c_plus_plus_spec_draftLexer.tokens c_plus_plus_spec_draftListener.java c_plus_plus_spec_draftParser.java