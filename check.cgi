#!/usr/bin/python
import cgi
import cgitb
cgitb.enable()
import os


UPLOAD_TXT_NAME = "checkin.txt"
UPLOAD_TXT_PATH = os.getcwd() + '/' + UPLOAD_TXT_NAME

HTML_TEMPLATE = """<!DOCTYPE html>
<html lang="en">
    <head>
    <meta charset="utf-8">
    <meta http-equiv="refresh" content="1; url=checkin.php" />    
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


def save_description(form_field, upload_dir):
    if form_field not in form:
        print HTML_TEMPLATE % {'MESSAGE': 'Field %s not found in form' % form_field}
        return False
    try:
        description = form.getfirst(form_field).upper().strip()

        fout = file(upload_dir, 'a')
        if (form_field == "lastname"):
            fout.write(description)
            fout.write('\n')
        else:
            fout.write(description + ":")
        fout.close()
        return True
    except:
        print HTML_TEMPLATE % {'MESSAGE': "Error: description item not in form"}
        return False


def save_card(form_field, upload_dir):
    if form_field not in form:
        print HTML_TEMPLATE % {'MESSAGE': 'Field %s not found in form' % form_field}
        return False

    raw = form.getfirst(form_field)

    # parsing the card information
    # specific for your card
    try:
        start = raw.find('%')
        end = raw.find('?')
        data = raw[start:end].split('^')
        name = data[5].split(',')
        first_name = name[1][1:].split(' ')[0]
        last_name = name[0]
        idNum = data[1]

        fout = file(upload_dir, 'a')
        fout.write(first_name + ':')
        fout.write(last_name)
        fout.write('\n')
        fout.close()

        print HTML_TEMPLATE % {'MESSAGE': 'Description uploaded successfully in ' + UPLOAD_TXT_PATH}
        return True

    except:
        print HTML_TEMPLATE % {'MESSAGE': "Error: Error reading Ucard"}
        return False

print 'content-type: text/html\n\n'
if 'cardtxt' in form:
    save_card("cardtxt", UPLOAD_TXT_PATH)
else:
    save1 = save_description("firstname", UPLOAD_TXT_PATH)
    save2 = save_description("lastname", UPLOAD_TXT_PATH)
    if (save1 and save2):
        print HTML_TEMPLATE % {'MESSAGE': 'Description uploaded successfully in ' + UPLOAD_TXT_PATH}
