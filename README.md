![](https://lithnet.github.io/images/logo-ex-small.png)
# Google Apps Management Agent for FIM 2010 R2 and MIM 2016
The Lithnet Google Apps Management Agent provides support for managing Google Apps users, groups, calendars and shared contacts with FIM 2010 R2 and higher.

>***Google API Deprecation notice***: Google have deprecated the use of the [email settings api](https://gsuiteupdates.googleblog.com/2018/10/email-settings-api-shutdown.html) and will disable it in October 2019. Ensure that you have migrated to v2 of the management agent before this date.

## Features
* Supports the import and export of domain shared contact objects
* Supports the import and export of user objects, including attributes from your custom schema
* Supports the import and export of group objects including group settings
* Supports the import and export of resources (calendars, buildings, features)
* Supports the import of domains and aliases (read only)
* Supports the import and export of course objects (Google Classroom)
* High performance import and export through the use of multithreading and batch updates
* Confirming (delta) import support for all object types

> Note: as most Google APIs do not provide delta support, this MA only confirms what was exported in a delta import. Changes made in Google Apps directly will not be seen until the next full import operation)

## System Requirements
The Lithnet Google App MA requires MIM 2016, and .NET Framework 4.7.2.

## Getting started
Download the management agent from the [releases page](https://github.com/lithnet/googleapps-managementagent/releases)

Read the [getting started guide](https://github.com/lithnet/googleapps-managementagent/wiki)

## Getting support
Please open an [issue](https://github.com/lithnet/googleapps-managementagent/issues), and provide a detailed description of the issue or question you'd like to ask

## Keep up to date
* [Visit our blog](https://blog.lithnet.io)
* [Follow mus on twitter](https://twitter.com/lithnet_io)![](http://twitter.com/favicon.ico)
