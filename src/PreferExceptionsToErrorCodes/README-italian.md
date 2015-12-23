[Indice](../../README-italian.md) - [Inglese](README.md)
# Le eccezioni sono da preferire ai codici di errore


Estratto da [Clean Code: A Handbook of Agile Software Craftsmanship](http://www.amazon.it/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882) di Robert Martin et.al.

## Command Query Separation

Le funzioni dovrebbero o fare qualcosa o dare delle risposte, ma non entrambe le cose. Una funzione dovrebbe cambiare lo stato di un oggetto, oppure restituire qualche informazione dell'oggetto. Eseguire entrambe le operazioni molto spesso crea confusione. Per esempio, si consideri la seguente funzione:

```java
public boolean set(String attribute, String value);
```

Questa funzione imposta il valore di un attributo individuato da un certo nome, e restituisce `true` in caso di successo, e `false` nel caso l'attributo non esista. Questo conduce a statement bizzarri, come questo:

```java
if (set("username", "unclebob"))...
```

Valutiamolo dal punto di vista del lettore. Cosa significa quell'espressione `if`? Forse verifica che l'attributo “`username`” abbia il valore “`unclebob`? O forse valuta se attributo “`username`” sia stato impostato con successo al valore “`unclebob`? È difficile desumere la semantica, perché non è chiaro se la parola `set` sia un verbo o un aggettivo.

L'autore intende usare il termine nell'accezione di verbo, ma nel contesto di un `if`, quel `set` sembra un aggettivo, per cui l'espressione rischia di essere letta come '*se l'attributo `username` ha il valore `unclebob`*', invece che '*imposta l'attributo `username` al valore `unclebob`*'. Si potrebbe provare a risolvere l'ambiguità rinominando la funzione `set` a `setAndCheckIfExists`, ma anche questo tentativo non migliora molto la leggibilità dell'`if`. La soluzione ideale è quella di separare il comando dalla query, in modo che l'ambiguità non possa capitare:


```java
if (attributeExists("username")) {
    setAttribute("username", "unclebob"); ...
}
```

## Le eccezioni sono da preferire ai codici di errore
Una funzione Command che restituisce codici di errore è una sottile violazione del principio di Command Query Separation, perché invita ad utilizzare il Command come un'espressione nel predicato di un `if`.

```java
if (deletePage(page) == E_OK)
```

Qui il problema non è la confusione tra verbo ed aggettivo, ma la tendenza a creare delle strutture annidate molto profonde. Nel momento in cui si restituisce un codice di errore si sta creando un problema al chiamante, perché questo è costretto a gestire l'errore immediatamente.

```java
if (deletePage(page) == E_OK) {
    if (registry.deleteReference(page.name) == E_OK) {
        if (configKeys.deleteKey(page.name.makeKey()) == E_OK){ logger.log("page deleted");
        } else {
            logger.log("configKey not deleted");
        }
    } else {
        logger.log("deleteReference from registry failed"); }
} else {
    logger.log("delete failed"); return E_ERROR;
}
```

Di contro, se si usassero delle eccezion invece che dei codici di errore di ritonro, allora la gestione degli errori risulterebbe separata dal codice per l'happy case, e il codice risulterebbe più semplice:

```java
try {
    deletePage(page); registry.deleteReference(page.name); configKeys.deleteKey(page.name.makeKey());
}
catch (Exception e) {
    logger.log(e.getMessage()); }
```

## Estrarre Try/Catch Blocks
Possiamo dire a pieno titolo che anche i blocchi `Try/catch` siano brutti. Confondono la struttura del codice e mescolano la gestione dell'errore con la normale gestione dei casi di business. Perciò, è meglio estrarre il corpo dei blocchi `try` e dei blocchi  `catch` in due funzioni separate:

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

In questo codice, la funzione `delete` si occupa solo della gestione degli errori. È facile da capire e da ignorare. La funzione `deletePageAndAllReferences` si occupa solo del processo di cancellazione della pagina. La gestione dei suoi casi di errore può essere ignorata. Questo approggio fornisce una buona separazione, che rende il codice più facile da comprendere e da modificare.


## La gestione degli errori è una funzione a sé
Le funzioni dovrebbero eseguire una singola cosa. La gestione degli errori è una di quelle cose. Per cui, una funzione che gestisce gli errori non dovrebbe fare nient'altro. Come nel codice riportato sopra, questo implica che se nella funzione compare la parola chiave `try`, dovrebbe essere la prima parola della funzione, e non dovrebbe comparire nient'altro nel blocco `catch/finally`.
