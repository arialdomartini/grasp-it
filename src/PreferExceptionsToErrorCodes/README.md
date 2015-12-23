[Table of Contents](../../README.md) - [Italian](README-italian.md)
# Prefer Exceptions to Error Codes

Except from [Clean Code: A Handbook of Agile Software Craftsmanship](http://www.amazon.it/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882) by Robert Martin et.al.

## Command Query Separation

Functions should either do something or answer something, but not both. Either your function should change the state of an object, or it should return some information about that object. Doing both often leads to confusion. Consider, for example, the following function:

```java
public boolean set(String attribute, String value);
```

This function sets the value of a named attribute and returns `true` if it is successful and `false` if no such attribute exists. This leads to odd statements like this:

```java
if (set("username", "unclebob"))...
```
Imagine this from the point of view of the reader. What does it mean? Is it asking whether the `username` attribute was previously set to `unclebob`? Or is it asking whether the `username` attribute was successfully set to `unclebob`? It’s hard to infer the meaning from the call because it’s not clear whether the word `set` is a verb or an adjective.

The author intended set to be a verb, but in the context of the `if` statement it feels like an adjective. So the statement reads as *“If the `username` attribute was previously set to `unclebob`”* and not *“set the `username` attribute to `unclebob` and if that worked then...”* We could try to resolve this by renaming the `set` function to `setAndCheckIfExists`, but that doesn’t much help the readability of the `if` statement. The real solution is to separate the command from the query so that the ambiguity cannot occur.

```java
if (attributeExists("username")) {
    setAttribute("username", "unclebob"); ...
}
```

## Prefer Exceptions to Returning Error Codes
Returning error codes from command functions is a subtle violation of command query separation. It promotes commands being used as expressions in the predicates of `if` statements.

```java
if (deletePage(page) == E_OK)
```

This does not suffer from verb/adjective confusion but does lead to deeply nested structures. When you return an error code, you create the problem that the caller must deal with the error immediately.

```java
if (deletePage(page) == E_OK) {
    if (registry.deleteReference(page.name) == E_OK) {
        if (configKeys.deleteKey(page.name.makeKey()) == E_OK) {
            logger.log("page deleted");
        } else {
            logger.log("configKey not deleted");
        }
    } else {
        logger.log("deleteReference from registry failed"); }
} else {
    logger.log("delete failed"); return E_ERROR;
}
```

On the other hand, if you use exceptions instead of returned error codes, then the error processing code can be separated from the happy path code and can be simplified:

```java
try {
    deletePage(page);
    registry.deleteReference(page.name);
    configKeys.deleteKey(page.name.makeKey());
}
catch (Exception e) {
    logger.log(e.getMessage()); }
```

## Extract Try/Catch Blocks
`Try/catch` blocks are ugly in their own right. They confuse the structure of the code and mix error processing with normal processing. So it is better to extract the bodies of the `try` and `catch` blocks out into functions of their own.


```java
public void delete(Page page) {
    try {
        deletePageAndAllReferences(page);
    }
    catch (Exception e) {
        logError(e);
    }
}

private void deletePageAndAllReferences(Page page) throws Exception {
    deletePage(page);
    registry.deleteReference(page.name);
    configKeys.deleteKey(page.name.makeKey());
}

private void logError(Exception e) {
    logger.log(e.getMessage());
}
```

In the above, the `delete` function is all about error processing. It is easy to understand and then ignore. The `deletePageAndAllReferences` function is all about the processes of fully deleting a page. Error handling can be ignored. This provides a nice separation that makes the code easier to understand and modify.


## Error Handling Is One Thing
Functions should do one thing. Error handing is one thing. Thus, a function that handles errors should do nothing else. This implies (as in the example above) that if the keyword `try` exists in a function, it should be the very first word in the function and that there should be nothing after the `catch/finally` blocks.
