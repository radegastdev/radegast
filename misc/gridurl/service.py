#!/usr/bin/env python

import wsgiref.handlers
import uuid
import urllib
import string
from google.appengine.ext import db
from google.appengine.ext import webapp
from google.appengine.ext.webapp import template
from urlparse import urlparse

ALPHABET = string.ascii_uppercase + string.ascii_lowercase + \
           string.digits + '-_.'
ALPHABET_REVERSE = dict((c, i) for (i, c) in enumerate(ALPHABET))
BASE = len(ALPHABET)

def num_encode(n):
    s = []
    while True:
        n, r = divmod(n, BASE)
        s.append(ALPHABET[r])
        if n == 0: break
    return ''.join(reversed(s))

def num_decode(s):
    n = 0
    for c in s:
        n = n * BASE + ALPHABET_REVERSE[c]
    return n

class GridUrl(db.Model):
    service = db.StringProperty(required = True)
    url = db.StringProperty(required = True)
    created = db.DateTimeProperty(auto_now_add = True)
    updated = db.DateTimeProperty(auto_now = True)
    
class RegistrationHandler(webapp.RequestHandler):
    def get(self):
        self.response.headers["Content-type"] = "text/plain"
        try:
            service = uuid.UUID("{" + self.request.get("service") + "}")
            url = urlparse(self.request.get("url"))
            if url.scheme != "http" and url.scheme != "https":
                raise Exception("Only http and https urls supported for url= parameter")
            url = self.request.get("url")
            key = "service_%s" % str(service)
            gridUrl = GridUrl(key_name = key, service = str(service), url = url)
            gridUrl.put()
            self.response.out.write("OK");
        except Exception, ex:
            self.response.set_status(503)
            self.response.out.write("ERROR^" + ex.message);
            return

class GoHandler(webapp.RequestHandler):
    def get(self):
        self.response.headers["Content-type"] = "text/plain"
        try:
            service = uuid.UUID("{" + self.request.path.split("/")[2] + "}")
            gridUrl = GridUrl.get_by_key_name("service_%s" % str(service))
            if gridUrl is None:
                raise Exception("Service not found")
            url = str(gridUrl.url)
            if self.request.query != "":
                url += "?" + self.request.query
            self.response.set_status(302)
            self.response.headers["Location"] = url
            self.response.out.write(url)
        except Exception, ex:
            self.response.set_status(404)
            self.response.out.write("ERROR^" + ex.message);
            return

class FetchHandler(webapp.RequestHandler):
    def get(self):
        self.response.headers["Content-type"] = "text/plain"
        try:
            service = uuid.UUID("{" + self.request.path.split("/")[2] + "}")
            gridUrl = GridUrl.get_by_key_name("service_%s" % str(service))
            if gridUrl is None:
                raise Exception("Service not found")
            self.response.out.write(str(gridUrl.url))
        except Exception, ex:
            self.response.set_status(404)
            self.response.out.write("ERROR^" + ex.message);
            return
