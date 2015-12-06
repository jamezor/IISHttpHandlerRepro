Run HttpHandlerWebSite/SetUpWebSite.ps1 as admin. This configures a new website on port 81, a new web app, and a new app pool. The app pool runs as you so needs your local user credentials.

Run Visual Studio as administrator,  open the solution and run the console app.

Explanation
-----------
There are two HttpHandlers configured: a default handler, and one scoped inside a <location> tag.

When concurrently hammering IIS and performing iisresets, sometimes requests will not hit the http handler that is inside the location tag, instead they will hit the default one.


What should happen?
-------------------
Given the structure of the web.config, it should be impossible for the default handler to ever get hit when the requested path is "subpath".

Reproduced on
-------------
Windows 10
Visual Studio 2015 (with and without Update 1)
