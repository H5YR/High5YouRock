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
4. On first run against the new database, an Umbraco migration will be performed to add an additional custom 'PostCount' table for storing post counts.
5. Log in to the back office and run uSync from the Settings section - import everything
6. Refresh the content node and a Home node should appear. If it doesn't, log out and log back in again.

### Mastodon integration

The Mastodon API is open for reading on the Umbraco Community instance (umbracocommunity.social), so no further configuration or keys are needed to read posts in from the API.

### Frontend Build

Vite is used on this project to automatically build assets for use with the backend when the project is run, however in most cases it is unlikely a full understanding of how it functions will be required and it will just work out of the box.

Edits should be made to files in the 'frontend' folder as needed, in particular using the '/frontend/all.js' file for JavaScript and the SASS includes underneath the /frontend/css subfolders for CSS.

When the project is run, either using Visual Studio in Debug Mode, or via the `dotnet run` command, a console window for Vite will automatically load in the background. No interaction is needed with this and after a few seconds, it should compile styling and apply it to the backend site in a browser automatically.

On some occasions when starting from Visual Studio this may crash. This can usually be identified by the styling not being auto-applied leaving an unstyled website, with a subsequent refresh of the browser then returning a '502 the server was shut down' error message. In this case, it can often be resolved via a restart, but if it persists by using the `dotnet run` command line method instead.

## Free holopin badge

If you'd like a free badge for your holopin collection - [Grab it](https://holopin.io/collect/clfcrtna163140fjsf31nv3ww)
 
