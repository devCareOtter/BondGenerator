## Synopsis
Command line tool to generate bond from a .Net type

## Usage
Provide the target assembly and dependencies in a directory. Specify the path to the assembly, the target outputpath for the resulting bond, and the types to reflect on and generate bond for. All types discovered while walking included types will be included in bond generation.

## Example
-AssemblyPath BusinessObjects.dll -OutputPath myBond.bond -Types Core.Entities.Person,Core.Accounts.BillingAccount

## Api Reference
https://microsoft.github.io/bond/manual/bond_cs.html

## Comments
This tool does not handle generics and datetimes. Care needs to be taken with these types to ensure data precision is maintained. DateTimes can be converted to epoch and stored in bond as Ints, or a type that represents DateTime can be defined.