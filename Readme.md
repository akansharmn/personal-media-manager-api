## Project Name

Web Diary   


# Project Description

This project maintains a web diary of user's online web activities. Currently only online videos watched are in in scope. It allows a user to tag a video, store information or notes along with it.
The user when laters visits that video page, he will be able to view all these informations. There are two parts to this project - A browser extension to help a user view all the information he stored while browsing videos and a backedn API to support it.
This repository conatins a Restful Web API implemented in ASP.Net backed up by SQLite databse to store all the information.

## WHY?

* A centralized place to view all the web contents and information associated with it stored by the user.
* The information is stored offline and can be browsed anytime.
* These contents can be organized by placing them into different playlists.
* User can search videos with tags.



## Architecture

The web browser plugin sends data to a backend API which is running locally to update in the SQLite database. The plugin, on the other hand, retrieves data from the API once a user visits a page and shows the information.
Architecture diagrams can be found here - 
https://github.com/akansharmn/apidocs-personal-media-manager-api/master/Visio




## Built With

* [ASP.Net Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-2.0) - The web framework used
* [SQLite](https://www.sqlite.org/index.html) - Database used
* [Swagger](https://swagger.io/) and [Swagger-Codegen](https://github.com/swagger-api/swagger-codegen)-API Documentation
* [IdentityServer4](https://github.com/IdentityServer/IdentityServer4) - OpenID Connect and OAuth 2.0 Framework for ASP.NET Core for authentication and authorization




## Versioning

Another version, which is cross-platform is available at - https://github.com/akansharmn/personal-media-manager-api. This work is still under progress.

## Author

* **Akansha Raman** 




## API Documentation
API documentation is available at -
https://akansharmn.github.io/apidocs-personal-media-manager-api/
