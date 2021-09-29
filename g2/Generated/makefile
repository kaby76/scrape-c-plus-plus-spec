# Generated code from trgen 0.10.0
GENERATED = SaveLexer.cs SaveParser.cs 
default:
	dotnet restore
	dotnet build
run:
	trwdog dotnet run $(RUNARGS)
clean:
	dotnet clean
	rm -rf bin obj
test:
	bash test.sh
