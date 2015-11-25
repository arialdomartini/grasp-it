# When To Use Exceptions


Except from [The Pragmatic Programmer: From Journeyman to Master ](http://www.amazon.it/The-Pragmatic-Programmer-Journeyman-Master/dp/020161622X) by Andrew Hunt and David Thomas

[English](README.md) - [Italian](README-italian.md)

In [Dead Programs Tell No Lies](../DeadProgramsTellNoLies/README.md), we suggested that it is good practice to check for every possible error—particularly the unexpected ones. However, in practice this can lead to some pretty ugly code; the normal logic of your program can end up being totally obscured by error handling, particularly if you subscribe to the "*a routine must have a single return statement*" school of programming (we don't). We've seen code that looks something like the following:

```c
retcode = OK;
if (socket.read(name) != OK) {
  retcode = BAD_READ; }
else {
  processName(name);
  if (socket.read(address) != OK) {
    retcode = BAD_READ; }
  else {
    processAddress(address);
    if (socket.read(telNo) != OK) {
      retcode = BAD_READ;
    }
    else {
      // etc, etc...
    }
  }
}
return retcode;
```

Fortunately, if the programming language supports exceptions, you can rewrite this code in a far neater way:

```java
retcode = OK;
try {
  socket.read(name);
  process(name);

  socket.read(address);
  processAddress(address);

  socket.read(telNo);
  // etc, etc...
}
catch (IOException e) {
  retcode = BAD_READ;
  Logger.log("Error reading individual: " + e.getMessage());
}
return retcode;
```

The normal flow of control is now clear, with all the error handling moved off to a single place.


## What Is Exceptional?

One of the problems with exceptions is knowing when to use them. We believe that exceptions should rarely be used as part of a program's normal flow; exceptions should be reserved for unexpected events. Assume that an uncaught exception will terminate your program and ask yourself, "Will this code still run if I remove all the exception handlers?" If the answer is "no," then maybe exceptions are being used in nonexceptional circumstances.

For example, if your code tries to open a file for reading and that file does not exist, should an exception be raised?


Our answer is, "*It depends.*" If the file should have been there, then an exception is warranted. Something unexpected happened — a file you were expecting to exist seems to have disappeared. On the other hand, if you have no idea whether the file should exist or not, then it doesn't seem exceptional if you can't find it, and an error return is appropriate.


Let's look at an example of the first case. The following code opens the file `/etc/passwd`, which should exist on all Unix systems. If it fails, it passes on the `FileNotFoundException` to its caller.

```java
public void open_passwd() throws FileNotFoundException {
  // This may throw FileNotFoundException...
  ipstream = new FileInputStream("/etc/passwd");
  // ...
}
```

However, the second case may involve opening a file specified by the user on the command line. Here an exception isn't warranted, and the code looks different:

```java
public boolean open_user_file(String name) throws FileNotFoundException {
  File f = new File(name);
  if (!f.exists()) {
    return false;
  }
  ipstream = new FileInputStream(f);
  return true;
}
```

Note that the `FileInputStream` call can still generate an exception, which the routine passes on. However, the exception will be generated under only truly exceptional circumstances; simply trying to open a file that does not exist will generate a conventional error return.


Tip 34|
------
Use Exceptions for Exceptional Problems|

Why do we suggest this approach to exceptions? Well, an exception represents an immediate, nonlocal transfer of control — it's a kind of cascading goto. Programs that use exceptions as part of their normal processing suffer from all the readability and maintainability problems of classic spaghetti code. These programs break encapsulation: routines and their callers are more tightly coupled via exception handling.

## Error Handlers Are an Alternative

An error handler is a routine that is called when an error is detected. You can register a routine to handle a specific category of errors. When one of these errors occurs, the handler will be called.

There are times when you may want to use error handlers, either instead of or alongside exceptions. Clearly, if you are using a language such as C, which does not support exceptions, this is one of your few other options (see the challenge on the next page). However, sometimes error handlers can be used even in languages (such as Java) that have a good exception handling scheme built in.

Consider the implementation of a client-server application, using Java's Remote Method Invocation (RMI) facility. Because of the way RMI is implemented, every call to a remote routine must be prepared to handle a `RemoteException`. Adding code to handle these exceptions can become tedious, and means that it is difficult to write code that works with both local and remote routines. A possible work-around is to wrap your remote objects in a class that is not remote. This class then implements an error handler interface, allowing the client code to register a routine to be called when a remote exception is detected.

