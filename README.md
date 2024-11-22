# ProjectCodeFixer

Attempts to repro https://github.com/dotnet/roslyn/issues/34636

## Steps to reproduce

1. Run `dotnet build`
1. Open `Sample\Sample.sln` and build

A build error `PCF001` should appear in `Class1`. 
If a `<Foo>...</Foo>` property is set on the project, it goes away.

NOTE: the Fixer project is packed with the analyzer, but currently it's not being 
picked up by VS :(. Unit testing is also not possible due to the following error:

```
System.InvalidOperationException : Code fix is attempting to provide a fix for a non-local analyzer diagnostic
```

Which doesn't make much sense since a code fixer can fix any artifacts in a project 
including additional files, AFAIK.