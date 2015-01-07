ieee-checkin
============

Live preview(s) available at [https://github.umn.edu/pages/IEEE/ieeecheckin/](https://github.umn.edu/pages/IEEE/ieeecheckin/)

Swipe your U Card or enter your info to check in at IEEE UMN meetings.

## Dependencies
* Currently running [twitter bootstrap](http://getbootstrap.com/2.3.2/), [jquery](http://jquery.com/download/)
* MySQL database (for IEEECheckin.PHP & IEEECheckin.ASP)
* Google Drive .NET SDK, Google Sheets .NET SDK, Google OAuth .NET SDK (for IEEECheckin.ASPDocs)

## Versions
* IEEECheckin.ASP - ASP.NET version, uses MySQL database for storage and ASP.NET credentials for user access
* IEEECheckin.ASPDocs - ASP.NET version, uses IndexedDB for client side storage and Google APIs for Google Drive integration
* IEEECheckin.JS - PHP version, uses IndexedDB for client side storage
* IEEECheckin.PHP - PHP version, uses MySQL database for storage and currently no user access restrictions 

## Install
PHP (IEEECheckin.JS & IEEECheckin.PHP):
* clone the repository into a directory on an apache server
* It is recommened to clone into your ~/.www directory on CSElabs
* ex: http://www-users.cselabs.umn.edu/~gask0032/<Local Repo Name>/checkin.html

.NET (IEEECheckin.ASP & IEEECheckin.ASPDocs):
* Use the WebPublish extension in Visual Studio to publish to an IIS/ASP.NET server


## Usage



-![alt text](http://i.imgur.com/himZD0M.gif "Sensible Huckller")
