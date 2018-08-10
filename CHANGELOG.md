# Version History

### RC Strings 1.3.0
*August 10, 2018*

Improvements:

* Add UI option for treating unused resources detected when the resources are found in the current RC but do not have a corresponding ID in any of the headers
* Add autoselect current word if nothing is selected

Bugs:

* Id generation started from 0
* Fix header path detection
* Header name was mandatory "resource.h"
* Fix UI input validation
* Consecutive whitespaces were removed
* Wrong resource placement in header file on add command
* RC file name was not visible on edit command

### RC Strings 1.2.0
*July 30, 2018*

Improvements:

* Allow multiline selection
* Add support for writing in hexadecimal format
* Add support for any kind of resources from RC file
* Add UI option for ID per project generation

Bugs:

* Tabs were wrongly interpreted

### RC Strings 1.1.0
*October 30, 2017*

Bugs:

* Fixed RC files detection when lightweight solution load is enabled.
* Fixed headers detection

### RC Strings 1.0.1
*August 29, 2017*

Bugs:

* Add support for *Lightweight solution load* on VS 2017.

### RC Strings 1.0.0
*August 21, 2017*

### Official release
First official release.

