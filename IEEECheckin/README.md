ieee-checkin
============

Swipe your U Card or enter your info to check in at IEEE UMN meetings.

## Dependencies
* Currently running [twitter bootstrap](http://getbootstrap.com/2.3.2/)
* [HTC Password Genrator](http://www.htaccesstools.com/htpasswd-generator/) for authorization

## Install
* clone the repository into a directory on an apache server
* It is recommened to clone into your ~/.www directory on CSElabs
* ex: http://www-users.cselabs.umn.edu/~gask0032/<Local Repo Name>/checkin.html

## Usage
* edit the .htcaccess to point to your own directory
```python
AuthName "Log in to use the IEEE exclusive check-in script"
AuthType Basic
AuthUserFile /Absolute/path/to/.htpasswd
AuthGroupFile /dev/null
require valid-user
```
* Use the [HTC Password Genrator](http://www.htaccesstools.com/htpasswd-generator/)_ to add authorized users to the .htpasswd file




-![alt text](http://i.imgur.com/himZD0M.gif "Sensible Huckller")
