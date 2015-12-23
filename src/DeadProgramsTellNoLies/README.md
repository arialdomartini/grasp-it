[Table of Contents](../../README.md) - [Italian](README-italian.md)
# Dead Programs Tell No Lies


Excerpt from [The Pragmatic Programmer: From Journeyman to Master ](http://www.amazon.it/The-Pragmatic-Programmer-Journeyman-Master/dp/020161622X) by Andrew Hunt and David Thomas

### Dead Programs Tell No Lies

Have you noticed that sometimes other people can detect that things aren't well with you before you're aware of the problem yourself? It's the same with other people's code. If something is starting to go awry with one of our programs, sometimes it is a library routine that catches it first. Maybe a stray pointer has caused us to overwrite a file handle with something meaningless. The next call to read will catch it. Perhaps a buffer overrun has trashed a counter we're about to use to determine how much memory to allocate. Maybe we'll get a failure from malloc. A logic error a couple of million instructions ago means that the selector for a case statement is no longer the expected 1, 2, or 3. We'll hit the default case (which is one reason why each and every case/switch statement needs to have a default clause—we want to know when the "impossible" has happened).

It's easy to fall into the "it can't happen" mentality. Most of us have written code that didn't check that a file closed successfully, or that a trace statement got written as we expected. And all things being equal, it's likely that we didn't need to—the code in question wouldn't fail under any normal conditions. But we're coding defensively. We're looking for rogue pointers in other parts of our program trashing the stack. We're checking that the correct versions of shared libraries were actually loaded.

All errors give you information. You could convince yourself that the error can't happen, and choose to ignore it. Instead, Pragmatic Programmers tell themselves that if there is an error, something very, very bad has happened.


Tip 32|
------
Crash Early|



### Crash, Don't Trash
One of the benefits of detecting problems as soon as you can is that you can crash earlier. And many times, crashing your program is the best thing you can do. The alternative may be to continue, writing corrupted data to some vital database or commanding the washing machine into its twentieth consecutive spin cycle.

The Java language and libraries have embraced this philosophy. When something unexpected happens within the runtime system, it throws a RuntimeException. If not caught, this will percolate up to the top level of the program and cause it to halt, displaying a stack trace.

You can do the same in other languages. If you don't have an exception mechanism, or if your libraries don't throw exceptions, then make sure you handle the errors yourself. In C, macros can be very useful for this:

```c
#define CHECK(LINE, EXPECTED) \
 { int rc = LINE; \
   if (rc != EXPECTED) \
   ut_abort(__FILE__, __LINE__, #LINE, rc, EXPECTED); }
   
   void ut_abort(char *file, int ln, char *line, int rc, int exp) {
     fprintf(stderr, "%s line %d\n'%s': expected %d, got %d\n",
        file, ln, line, exp, rc);
     exit(1);
 }
```

Then you can wrap calls that should never fail using

```c
CHECK(stat("/tmp", &stat_buff), 0);
```

If it should fail, you'd get a message written to `stderr`:

```c
source.c line 19
'stat("/tmp", &stat_buff)': expected 0, got -1
```

Clearly it is sometimes inappropriate simply to exit a running program. You may have claimed resources that might not get released, or you may need to write log messages, tidy up open transactions, or interact with other processes. The techniques we discuss in When to Use Exceptions, will help here. However, the basic principle stays the same: when your code discovers that something that was supposed to be impossible just happened, your program is no longer viable. Anything it does from this point forward becomes suspect, so terminate it as soon as possible. A dead program normally does a lot less damage than a crippled one.

Related sections include:
* Design by Contract
* When to Use Exceptions



# Challenge

In this directory there's a C# project with a failing test. Your goal is to make the test pass, fixing the production code.

## Details
The program evaluates the homeworks produced by 3 students, comparing them with the official solution; then it assigns the students the final marks, which are used to determine if the students pass or fail. The homework consists of 4 questions: if all the answers are right, the final mark is 10; with 1 error the mark is 6; 2 ore more errors give the mark 3, and causes the rejection from school.

You will easily realize that the program has a very poor quality: it is terribly procedural, it has not been developed with TDD, it contains a lot of bugs and its code is plagued by defensive checks, so much that the domain logic is hidden by a mass of `if`s and `try/catch`es. What's wrong, it is apparent that the developer tried hard to avoid the program crashes. Unluckily, this goal has been reached at a high price: there's a bug, somewhere, that rather than crashing the programs, corrupts its state and keeps it in life, leading it to unexpected results; as a consequence, one of the students gets the wrong mark and gets unfairly rejected.

## Hints

In Pragmatic Programmer, Andrew Hunt and David Thomas give us the following rule of thumb:

> when your code discovers that something that was supposed to be impossible just happened, your program is no longer viable.
> Anything it does from this point forward becomes suspect, so terminate it as soon as possible.
> A dead program normally does a lot less damage than a crippled one

Try to modify the code so that any unexpected event cause a crash. It's not hard at all: the .NET framework, just like the Java runtime, does this automatically for you; as soon as something not foreseen by the code happens, execution stops, an exception is thrown and the runtime makes the exception percolate in search of something able to manage it. In your case, since the event is unexpected (it's a bug inadvertitely introduced by the programmer) all you have to do is to ignore it and let the application crash. In other words, you should just remove all the `try/catch` statements.

Note that there are 2 kinds of unexpected events: exceptional events, and events that, on the contrary, can easily be foreseen, even if they are not welcome. For example, if the program gets input from the user, it's wise to validate it. On the contrary, when invoking a constructor, the possibility that it fails because memory is not available is an unexpected event.

The hint is: validate what is predictable, and let the runtime manage what's unpredictable, and don't be scared that this means your program will crash; a controlled crash is always better than a working program with an out-of-control state.

You could see `try/catch`es as attempts to control the unpredictable: a `try/catch` says

> *Should something go wrong with this section of code, don't crash, but tries to recover the execution, assuming that the code that broke gave this particular default result; the rest of the program will manage this exceptional case*.

This is exactly our program's approach. Unfortunately, the exceptional case is managed in a wrong way, with disastrous result.

As an exercise, try to face the problem in a different way: modify the code removing all the defensive statements (all the `if`s not required by domain logic and the `try/catch`es); let unpredictable events crash the program, following the Pragmatic Programmer's suggestion: a dead program normally does a lot less daage than a crippled one.

## Questions

* In the case of our code, the unpredictable event was a bug. How can `try/catch`es protect the program and its domain logic from programming errors introduced by developers?
* Does the practice of TDD influence the defensive stile of code?
