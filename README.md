Run HttpHandlerWebSite/SetUpWebSite.ps1 as admin. This configures a new website on port 81 and creates a new web application. The script also builds the Reproducer project and displays the location of the compiled binary.

Run the Reproducer application (also as administrator) that the script built and watch the output. You should eventually see `Got the wrong result: This is the default handler`, which means that the bug has occurred.

Explanation
-----------
There are two HttpHandlers configured: a default handler, and one scoped inside a `<location>` tag.

When concurrently hammering IIS and performing iisresets, sometimes requests will not hit the http handler that is inside the location tag, instead they will hit the default one.


What should happen?
-------------------
Given the structure of the web.config, it should be impossible for the default handler to ever get hit when the requested path is "subpath".

Reproduced on
-------------
* Freshly installed Windows 10 build 10586.
