# High 5, You Rock!
H5YR website build with Umbraco 10 - https://h5yr.com

## Working with this repo

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Node.js](https://nodejs.org/en/) v18+


### Getting started

1. Clone down this repo
2. Install frontend dependencies (`cd frontend && npm i`)
2. Spin up the site using IIS Express or Kestrel
3. Create a new local DB, SQLLite is fine for this
4. Log in to the back office and run uSync from the Settings section - import everything
5. Refresh the content node and a Home node should appear. If it doesn't, log out and log back in again.


## out of date

Todo: This info is now out of date and really needs updating for mastodon. 

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
  ## Free holopin badge

  If you'd like a free badge for your holopin collection - (Grab it)[https://holopin.io/collect/clfcrtna163140fjsf31nv3ww]
 
