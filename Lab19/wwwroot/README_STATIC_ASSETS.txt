wwwroot/lib/ (Bootstrap, jQuery, jQuery Validation) and wwwroot/favicon.ico are the
standard Visual Studio-scaffolded client libraries — unchanged since the project
was first created and untouched by every session including this one.

They are not reproduced in this delivered folder (they are large, vendor-owned
binary/minified files with zero connection to today's C#/Razor content). Before
running this project, copy wwwroot/lib/ and wwwroot/favicon.ico forward from
Session 18's StudentPortalWeb/wwwroot/ folder — or, if that folder has already
been archived, restore them via Visual Studio's "Manage Client-Side Libraries"
(libman) using the same versions: bootstrap (dist/css, dist/js),
jquery (dist), jquery-validation (dist), jquery-validation-unobtrusive (dist).

This is an environment-setup step, not a TODO — nobody types these files, and
they were never part of any session's taught content.
