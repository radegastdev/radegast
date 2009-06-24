#!/usr/bin/env python

import wsgiref.handlers
import service
from google.appengine.ext import db
from google.appengine.ext import webapp
from google.appengine.ext.webapp import template
from uuid import uuid4

class UrlHandler(webapp.RequestHandler):
    def get(self):
        self.response.out.write(
          template.render('templates/main.html', {}))

class AboutUrlHandler(webapp.RequestHandler):
    def get(self):
        self.response.out.write(
          template.render('templates/about.html', {}))
        
class RandomNiceUrlHandler(webapp.RequestHandler):
    def get(self):
        self.response.out.write(
          template.render('templates/random_nice.html', { "uuid" : uuid4() }))

class RandomUrlHandler(webapp.RequestHandler):
    def get(self):
        self.response.headers["Content-type"] = "text/plain"
        self.response.out.write(uuid4())

def main():
    app = webapp.WSGIApplication([
      (r'/', UrlHandler),
      (r'/random', RandomNiceUrlHandler),
      (r'/rand', RandomUrlHandler),
      (r'/about', AboutUrlHandler),
      (r'/reg', service.RegistrationHandler),
      (r'/go/.*', service.GoHandler),
      (r'/get/.*', service.FetchHandler),
      ], debug=False)
    wsgiref.handlers.CGIHandler().run(app)
    
if __name__ == "__main__":
    main()