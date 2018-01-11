# Change Log

## Version 1.2.0: [CURRENT RELEASE]
### Summary
* Support for very complex expressions. Allowing groups within groups as well as a close group functionality ([Improvement on the previous grouping](issues/10))
* Added match types. Match a list of values (i.e. A name that contains any of: "John", "Jess") [See reference](\#complex-expressions)

### Changes
* Created the `FilterStatementMatchType` enumeration
 * Added "Allowed MatchTypes" to `Operations` and defined usage in `NumberOfValuesAttribute`
* Created `IFilterGroup`, `FilterGroup` and `IFilterStatementOrGroup` (abstract classes)
 * `IFilterGroup` contains a Group (`List<IFilterStatementOrGroup>`), this can contain either Statements or Groups (Also applies to `FilterGroup`) [Built in XML serialisation]
 * `IFilterStatementOrGroup` is now used instead of FilterStatement, this allows for groups in groups. The `Build` method is used to build the expression
* Changes to `Filter`
  * Changed `By` method (removing second value). You now create arrays/lists of values (when needed).
    * Added a method to maintain backward compatibility
    * Added another method for Enumerables (items need to be declared `AsEnumerable()`) [Not ideal, but the changed `By` statement still does the correct working]
  * Filter's statements are now a list of `IFilterStatementOrGroup`
  * Added `private` `_nest` which keeps track of current position in the `Statements`
  * `ToString()` now writes `List`s correctly (See changes to `FilterStatement`)
  * Adjusted `XML` methods
  * `Group` replaced with `OpenGroup`
    * Modified `StartGroup()` and added `EndGroup()` to support this
* Changes to `IFilterStatementConnector`
  * Added `CloseGroup` here, as this made more sense for fluent builds
* Changes to `IFilterStatement` 
  * It is now an abstract class (needed to do this to maintain `FilterBuilder`'s hidden status)
* Changes to `FilterStatement` 
  * Added `GetPropertyType` method to correctly fetch the type (used in various places)
  * Added a method to return `ToString()` of each value (used in the `ToString()` methods and in various other places)
  * Modified and added new `ToString` method to show various types of `Value`'s submitted
  * _NOTE: May have fixed `TODO:` on line 142_
* Changes to `FilterBuilder`
  * `Expressions` (dictionary) now only needs 2 expressions passing to it. As we've changed the way values work (allows lists)
  * Added `GetSafeExpression` method that correctly handles whether the `Value` is a `List` or not
  * `Between` method is now passed an `(ConstantExpression)Expression((Array)List<Value's>)` which is indexed to be used in the expression

### Minor Changes (Not Affecting NuGet Package)
* Made changes to the Form application to all the use of [`Groups`](\#complex-expressions) and `Match Any|All`
* Revised tests to make them work with the new methods. Also removed some test as they no longer apply
* Added changelog
* Revised README to mention new updates and extra functionality (also added new image to assist the change)


## Version 1.1.2:
### Summary
* New operation added: DoesNotContain
* [Support for complex expressions](issues/10) (those that group up statements within parenthesis)
* Added tests using LINQ to SQL (along with a [bug fix regarding the library usage with LINQ to SQL](issues/12))

## Version 1.1.1:
// TODO: Add these changes

## Version 1.1.0:
* Initial release

___

___

# License
Copyright 2018 David Belmont

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at [LICENSE](LICENSE)

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Icon by [Alina Oleynik](https://thenounproject.com/dorxela), Ukraine