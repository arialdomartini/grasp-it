# Assertive Programming

Except from [The Pragmatic Programmer: From Journeyman to Master ](http://www.amazon.it/The-Pragmatic-Programmer-Journeyman-Master/dp/020161622X) by Andrew Hunt and David Thomas

[English](README.md) - [Italian](README-italian.md)

> There is a luxury in self-reproach. When we blame ourselves we feel no one else has a right to blame us.
> (Oscar Wilde, The Picture of Dorian Gray)


It seems that there's a mantra that every programmer must memorize early in his or her career. It is a fundamental tenet of computing, a core belief that we learn to apply to requirements, designs, code, comments, just about everything we do. It goes

> THIS CAN NEVER HAPPEN...

"This code won't be used 30 years from now, so two-digit dates are fine." "This application will never be used abroad, so why internationalize it?" "count can't be negative." "This printf can't fail."

Let's not practice this kind of self-deception, particularly when coding.


Tip 33|
------
If It Can't Happen, Use Assertions to Ensure That It Won't|

Whenever you find yourself thinking "*but of course that could never happen*", add code to check it. The easiest way to do this is with assertions. In most C and C++ implementations, you'll find some form of `assert` or `_assert` macro that checks a Boolean condition. These macros can be invaluable. If a pointer passed in to your procedure should never be `NULL`, then check for it:


```c
void writeString(char *string) {
   assert(string != NULL);
   ...
   
   ```

Assertions are also useful checks on an algorithm's operation. Maybe you've written a clever sort algorithm. Check that it works:

```c
for (int i = 0; i < num_entries-1; i++) {
    assert(sorted[i] <= sorted[i+i]);
}

```

Of course, the condition passed to an assertion should not have a side effect (see the box on page 124). Also remember that assertions may be turned off at compile time —- never put code that must be executed into an assert.

Don't use assertions in place of real error handling. Assertions check for things that should never happen: you don't want to be writing code such as

```c
printf("Enter 'Y' or 'N': ");
ch = getchar();
assert((ch == 'Y') || (ch == 'N')); /* bad idea! */
```


And just because the supplied `assert` macros call exit when an assertion fails, there's no reason why versions you write should. If you need to free resources, have an assertion failure generate an exception, `longjmp` to an exit point, or call an error handler. Just make sure the code you execute in those dying milliseconds doesn't rely on the information that triggered the assertion failure in the first place.


### Leave Assertions Turned On

There is a common misunderstanding about assertions, promulgated by the people who write compilers and language environments. It goes something like this:

> Assertions add some overhead to code. Because they check for things that should never happen, they'll get triggered only by a bug in the code. Once the code has been tested and shipped, they are no longer needed, and should be turned off to make the code run faster. Assertions are a debugging facility.

There are two patently wrong assumptions here. First, they assume that testing finds all the bugs. In reality, for any complex program you are unlikely to test even a miniscule
percentage of the permutations your code will be put through (see Ruthless Testing). Second, the optimists are forgetting that your program runs in a dangerous world. During testing, rats probably won't gnaw through a communications cable, someone playing a game won't exhaust memory, and log files won't fill the hard drive. These things might happen when your program runs in a production environment. Your first line of defense is checking for any possible error, and your second is using assertions to try to detect those you've missed.

Turning off assertions when you deliver a program to production is like crossing a high wire without a net because you once made it across in practice. There's dramatic value, but it's hard to get life insurance.

Even if you do have performance issues, turn off only those assertions that really hit you. The sort example above may be a critical part of

### Assertion and Side Effects

It is embarrassing when the code we add to detect errors actually ends up creatings new errors. This can happen with assertions if evaluating the condition has side effects. For example, in Java it would be a bad to code something such as

```c
while (iter.hasmoreElements () {
  Test.ASSERT(iter.nextElements() != null);
  object obj = iter.nextElement();
  // ....
}
```

The `.nextElement()` call in the `ASSERT` has the side effects of moving the iterator past the element being fetched, and so the loop will process only half the elements in the collection. It would be better to write

```c
while (iter.hasmoreElements()) {
  object obj = iter.nextElement();
  Test.ASSERT(obj != null);
  //....
```

This problem is a kind of "Heisenbug"—debugging that changes the behavior of the system system being debugged (see [www.jargon.org, Eric S. Raymond](www.jargon.org)
Definitions for many common (and not so common) computer industry terms, along with a good dose of folklore.)
