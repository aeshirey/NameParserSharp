# NameParserSharp

Based upon python [nameparser 0.36](https://pypi.python.org/pypi/nameparser), NameParserSharp is a C# library that parses a human name into constituent fields `Title`, `First`, `Middle`, `Last`, `Suffix`, and `Nickname` from the `HumanName` class. NameParserSharp implements the functionality of the Python project on which it is based in a C# idiomatic way. It also, 
* eliminates nearly all regular expressions for efficiency
* adds unit tests
* improves nickname handling to expand delimiters: `John (Jack) Torrence` == `John 'Jack' Torrence` == `John "Jack" Torrence`
* parses out multiple names from a single string as you might expect, as in `mr john and mrs jane doe`


## Installation

### Using NuGet Package Manager
```powershell
Install-Package NameParserSharp
```
### Using .NET CLI
```bash
dotnet add package NameParserSharp
```
## Quick Start
### Basic Usage
Start parsing names by creating an instance of the HumanName class with the full name string. Access the parsed components through the provided properties.
```c#
var jfk = new HumanName("president john 'jack' f kennedy");

// Accessing name components
Console.WriteLine(jfk.Title);      // Output: president
Console.WriteLine(jfk.First);      // Output: john
Console.WriteLine(jfk.Middle);     // Output: f
Console.WriteLine(jfk.Last);       // Output: kennedy
Console.WriteLine(jfk.Nickname);   // Output: jack

// Parsing an alternative format
var jfk_alt = new HumanName("kennedy, president john (jack) f");
Assert.IsTrue(jfk == jfk_alt);
```
### Parsing Multiple Names
Enable the parsing of multiple names within a single string by setting the ParseMultipleNames flag to true. This is useful for handling inputs like "John D. and Catherine T. MacArthur."
```c#
// Enable parsing of multiple names
HumanName.ParseMultipleNames = true;

// Example full name containing two individuals
string fullName = "John D. and Catherine T. MacArthur";

// Instantiate the HumanName class with the full name
HumanName name = new HumanName(fullName);

// Display primary name components
Console.WriteLine("Primary Name:");
Console.WriteLine($"  First Name: {name.First}");
Console.WriteLine($"  Middle Name: {name.Middle}");
Console.WriteLine($"  Last Name: {name.Last}");
Console.WriteLine($"  Suffix: {name.Suffix}");
Console.WriteLine($"  Nickname: {name.Nickname}");
Console.WriteLine($"  Title: {name.Title}");
Console.WriteLine();

// Check and display additional name components if present
if (name.AdditionalName != null)
{
    Console.WriteLine("Additional Name:");
    Console.WriteLine($"  First Name: {name.AdditionalName.First}");
    Console.WriteLine($"  Middle Name: {name.AdditionalName.Middle}");
    Console.WriteLine($"  Last Name: {name.AdditionalName.Last}");
    Console.WriteLine($"  Suffix: {name.AdditionalName.Suffix}");
    Console.WriteLine($"  Nickname: {name.AdditionalName.Nickname}");
    Console.WriteLine($"  Title: {name.AdditionalName.Title}");
}
```
### Normalization
Ensure consistency by normalizing and capitalizing name components using the `Normalize` method. This method formats each part of the name appropriately.
```c#
var rawName = "juan de garcia";
var name = new HumanName(rawName);

// Normalize the name
name.Normalize();

Console.WriteLine(name.FullName); // Output: Juan de Garcia
```



