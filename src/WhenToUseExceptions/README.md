# When To Use Exceptions


Except from [The Pragmatic Programmer: From Journeyman to Master ](http://www.amazon.it/The-Pragmatic-Programmer-Journeyman-Master/dp/020161622X) by Andrew Hunt and David Thomas

[English](README.md) - [Italian](README-italian.md)

In [Dead Programs Tell No Lies](../DeadProgramsTellNoLies/README.md), we suggested that it is good practice to check for every possible error—particularly the unexpected ones. However, in practice this can lead to some pretty ugly code; the normal logic of your program can end up being totally obscured by error handling, particularly if you subscribe to the "a routine must have a single return statement" school of programming (we don't). We've seen code that looks something like the following:
