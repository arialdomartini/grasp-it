[Table of Contents](../../README.md) - [Italian](README-italian.md)
# When To Use Exceptions

Except from [The Pragmatic Programmer: From Journeyman to Master ](http://www.amazon.it/The-Pragmatic-Programmer-Journeyman-Master/dp/020161622X) by Andrew Hunt and David Thomas

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


# Challenge

This directory contais a C# project where Exceptions are heavily used to manage the ordinary workflow. The goal is to refactor the code, and remove all the exception handling cases where exceptions have been used for the ordinary workflow and not for exceptional cases.

## Details

The program implements an online wallett. Its code is test covered, and ideally requirements could be inferred from tests. Nevertheless, the most important requirements are listed below, for the sake of clarity:

* Money can be withdrawn from an instance of `Wallet`, as long as its property `Balance` does not get negative;
* Any attempts to withdraw more money than available fails, and as a consequence a loanshark is alerted, so he can get ready to make a loan offer;
* Withdrawing more than 1000 euros implies a 2 euros tax;
* `Balance` must be big enough for the withdrawn and for the eventual tax, or the case is treated as an attempt to withdraw more money than available;
* A `Wallet` is either owned by a physical person (stored in the field `WalletOwner`) or, in case `WalletOwner` is `null`, by a non-profit organization. Non-profit organizations' withdrawns are always tax free;
* Any tax payment must be logged to file.

As usual, in the code all the wrong practices have been taken to the limit. Even if the code has been developed in TDD, it is plagued by a lot of problems.

## Questions

* Pragmatic Programmer suggests an equivalence between exceptions and `goto`s, and claims that that use of exceptions makes the code affected to the same problems of non-structured code. Ths program contains a pretty hidden resource leak. Can you spot (and fix) it?
* Kent Beck recommends that in the Red/Green/Refactor phases, in the Green one 
