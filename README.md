Meeting Check-in Web App
============

Swipe your ID card or enter your info to check in at any meetings.

Based on ASP.NET and AngularJS, the web app uses IndexedDB to store information client side. Allows for managing entries in client or to export to files.

## Dependencies
* Javascript Frameworks [bootstrap 3.3.7](http://getbootstrap.com/), [jquery 3.2.1](http://jquery.com/download/), and [angularjs 1.6.4](https://angularjs.org/)
* Microsoft IIS and ASP.NET
* Visual Studio 2015 or newer

## Install
* Open the solution
* To run locally, run the project
* To publish to a server, use the WebPublish extension in Visual Studio to publish to an IIS/ASP.NET server
* Expects to be in a directory 'checkin' at the server root (i.e. ieee.umn.edu/checkin/)
    * To change base directory, change the '<base .../>' tag in Site.Master, the 'basePath' variable in controller.js, the image paths in themes.json, and the publish profile publish path.

## Future Improvements/Work
* Refactoring of the format controller to simplify operation and reduce amount of code
* Implementation of offline caching
* Ordering of meetings in dropdowns by date and name