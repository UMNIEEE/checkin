#!/usr/bin/python
import cgi
import cgitb
cgitb.enable()
import os
import subprocess
import smtplib #Import smtplib for sending function
from email.mime.text import MIMEText #Import the email modules needed

UPLOAD_TXT_NAME = 'checkin.txt'

HTML_TEMPLATE = """<!DOCTYPE html>
<html lang="en">
    <head>
    <meta charset="utf-8">
    <meta http-equiv="refresh" content="1; url=checkin.html" />    
        <title>IEEE Meeting Check-In</title>
        <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css">
        <link href='http://fonts.googleapis.com/css?family=Lato:300,400,700|Oswald:300' rel='stylesheet' type='text/css'>
        <link rel="stylesheet" href="//netdna.bootstrapcdn.com/font-awesome/4.0.3/css/font-awesome.min.css">
        <link rel="stylesheet" href="css/checkin.css">
        <!--[if lt IE 9]>
        <script src="http://html5shiv.googlecode.com/svn/trunk/html5.js"></script>
        <![endif]-->
    </head>
    <body>
        <div class="container">
            <div class="col-lg-4 col-lg-offset-4 col-md-6 col-md-offset-3 col-sm-8 col-sm-offset-2 col-xs-12">
                <h1 class="pre-header">University of Minnesota</h1>
                <img class="logo" src="images/ieee.svg">
                <h1 class="post-header">%(MESSAGE)s</h1>
                <script src="//netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js"></script>
                <p class="footer">Powered by the IEEE Tech Subcommittee</p>
            </div>
        </div>
    </body>
</html>"""

form = cgi.FieldStorage()
#create a blank file to send if nothing is available
f = file(UPLOAD_TXT_NAME, 'a')
f.close()

print 'content-type: text/html\n\n'

def send_email():
    if 'subject' not in form:
        print HTML_TEMPLATE % {'MESSAGE':'No subject specified'}
        return

    if 'recipient' not in form:
        print HTML_TEMPLATE % {'MESSAGE':'No recipient specified'}
        return

    subject = form.getfirst('subject').strip()
    recipient = form.getfirst('recipient').strip()
    
    subprocess.call("cat checkin.txt | mail -s " +
    "\" " + subject + "\" " + recipient, shell=True)

send_email()
print HTML_TEMPLATE % {'MESSAGE':'Email Sent!'}
