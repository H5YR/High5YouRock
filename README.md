# H5YR Website Setup Guide

![High 5, You Rock!](https://h5yr.com/assets/images/h5yr-logo.png)

Welcome to the High 5, You Rock! (H5YR) website setup guide. This guide will help you get the H5YR website up and running using Umbraco 10.

## Getting Started

To get started, follow these steps:

1. **Clone the Repository**: Clone down this repository to your local machine.

2. **Spin Up the Site**: You can run the site using either IIS Express or Kestrel, depending on your development environment.

3. **Create a Local Database**: You'll need a local database for the website. SQLLite is a suitable option for this purpose.

4. **Run uSync**: Log in to the Umbraco back office, and navigate to the Settings section. Run uSync to import all settings and configurations.

5. **Refresh Content Node**: After running uSync, refresh the content node in the back office. A Home node should appear. If it doesn't, try logging out and logging back in.

## Twitter Integration (Out of Date)

Please note that the information regarding Twitter integration is now out of date and needs to be updated for Mastodon. If you still wish to integrate Twitter, follow these outdated steps:

1. **Local Tweet Feed**: When working locally, the tweets displayed on the feed are read from a test Tweet JSON file. If you want to display live tweets in the feed, you'll need your own Twitter API keys.

2. **User Secrets File**: Store your Twitter API keys in your own User Secrets file. If you're not familiar with User Secrets, you can find more information [here](https://skrift.io/issues/tell-me-all-your-secrets/).

3. **User Secrets Configuration**:

    Add the following configuration to your User Secrets file and replace the placeholders with your Twitter API values:

    ```json
    "Twitter": {
        "ConsumerKey": "your_consumer_key",
        "ConsumerSecret": "your_consumer_secret",
        "AccessToken": "your_access_token",
        "AccessTokenSecret": "your_access_token_secret"
    }
    ```

Please note that integrating Mastodon or any other social media platform will require different configuration and steps, and it's essential to keep the integration up to date with the latest technologies and APIs.
