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

> NOTE: the code fixer PROJECT is propertly set up and hooked-up, as shown by 
> another dummy analyzer/fixer pair that adds a "1" suffix to types without it.

## Expected behavior

The fixer shows up and can add an MSBuild project property to the project file.


The package contents is:

```
Package  ProjectCodeFixer.42.252.881.nupkg
         .\Fixers\bin\ProjectCodeFixer.nuspec
├── Metadata:
│   Description       ProjectCodeFixer
│   RepositoryCommit  f504c3c98dfc42a4951630a86d2399c4d55a06c9
│   RepositoryType    git
│   Version           42.252.881
└── Contents:
    ├── analyzers
    │   └── dotnet
    │       ├── Analyzers.dll
    │       ├── Analyzers.pdb
    │       ├── Fixers.dll
    │       └── Fixers.pdb
    └── build
        └── ProjectCodeFixer.targets
```

(render by [nugetizer](https://github.com/devlooped/nugetizer))
