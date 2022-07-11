# High 5, You Rock!
H5YR website build with Umbraco 10

* To get started - clone down this repo and spin up the site using IIS Express / Kestrel.
* Create a new local DB, SQLLite is fine for this
* Log in to the back office and run uSync from the Settings section - import everything
* Refresh the content node and a Home node should appear. If it doesn't, log out and log back in again.
* When working locally, the tweets shown on the feed are read from a test Tweet json file. If you want to see live tweets in the feed you'll need your own Twitter API keys and these should be stored in your own User Secrets file, more info about user secrets can be found https://skrift.io/issues/tell-me-all-your-secrets/

* You need to add the following lines to your User Secrets file and add your own twitter api values
```
 "Twitter": {
    "ConsumerKey": "",
    "ConsumerSecret": "",
    "AccessToken": "",
    "AccessTokenSecret": "" 
  } 
  ``` 
  
 
