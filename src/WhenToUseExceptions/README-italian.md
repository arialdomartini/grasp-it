# Quando usare le eccezioni


Estratto da [The Pragmatic Programmer: From Journeyman to Master ](http://www.amazon.it/The-Pragmatic-Programmer-Journeyman-Master/dp/020161622X) di Andrew Hunt e David Thomas

[English](README.md) - [Italian](README-italian.md)

In [Dead Programs Tell No Lies](../DeadProgramsTellNoLies/README-italian.md) abbiamo suggerito che verificare ogni possibile errore, specialmente quelli inattesi, sia una buona pratica. Tuttavia, nella pratica questo porta a scrive del codice piuttosto orribile; la logica di dominio del programma finisce per essere completamente oscurata dal codice per la gestione degli errori, soprattutto se si aderisce alla scuola "*una routing deve avere un singolo return*" (noi non aderiamo). Tutti quanti abbiamo visto codice come questo:

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

Fortunatamente, se il linguaggio di programmazione supporta le eccezioni, è possibile riscrivere il codice in modo più ordinato:

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

Il flusso di controllo adesso è più chiaro, e il codice di gestione degli errori è stato spostato in un unico punto.


## Cosa è eccezionale?

Una della difficoltà con le eccezioni è comprendere quando vadano usate. Noi crediamo che difficilmente le eccezioni dovrebbero essere usate come parte del normale flusso del programma; le eccezioni dovrebbero essere riservate per eventi eccezionali. Nel caso un'eccezione non gestita termini il vostro programma, domandatevi "Questo codice funzionerebbe lo stesso se rimuovessi l'handler delle eccezioni"? Se la risposta è "no", allora è possibile che le eccezioni siano state usate per circostanze non eccezionali.

Per esempio, se il codice provasse ad aprire un file in lettura e il file dovesse non esistere, dovrebbe essere lanciata un'eccezione?

La risposta che noi diamo è "*Dipende.*". Se il file doveva per forza essere lì, allora è consentito lanciare un'eccezione. È successo qualcosa di inatteso - il file, che era richiesto, sembra essere scomparso. Al contrario, se non si ha idea se il file debba esistere o no, allora non è così eccezionale che non li riesca a trovare, per cui potrebbe essere più appropriato restituire un errore.

Diamo un occhio ad un esempio del primo dei due casi. Il codice seguente apre il file `/etc/passwd`, che dovrebbe esistere in ogni sistema Unix. Se il codice fallisce, passa una `FileNotFoundException` al suo chiamante.

```java
public void open_passwd() throws FileNotFoundException {
  // This may throw FileNotFoundException...
  ipstream = new FileInputStream("/etc/passwd");
  // ...
}
```

Invece, il secondo caso potrebbe riguardare l'apertura di un file specificato dall'utente attraverso la linea di comando. Qui l'eccezione non è autorizzata, e il codice appare diverso:

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

Si noti come la chiamata a `FileInputStream` possa ancora generare un'eccezione durante l'esecuzione. Tuttavia, l'eccezione verrebbe generata solo in circostanze davvero eccezionali; il semplice tentativo di aprire un file che non esiste restituisce un errore convenzionale.


Tip 34|
------
Usa le eccezioni per circostanze eccezionali|

Perché suggeriamo questo approccio per le eccezioni? Beh, un'eccezione rappresenta un trasferimento di controllo non locale ed immediato - è una sorta di cascata di `goto`. I rogrammi che utilizzano le eccezioni come parte del normale flusso di processo soffronto dei medesimi problemi di leggibilità e di manutenibilità dello spaghetti code. Questi programmi rompono l'incapsulamento: le routine e i loro chiamanti diventano molto più accoppiati, a causa dell'exception handling.

## Gli Error Handlers sono un'alternativa

Un erorr handler è una routine che viene invocata quando viene individuato un errore. Si può registrare una routine perché gestisca una particolare categoria di errori. Quando uno di questi errori avviene, la routine viene invocata.

Ci sono occasioni in cui usare degli error handler è desiderabile, come alternativa o insieme alle eccezioni. Chiaramente, se si sta utilizzando un linguaggio come il C, che non ha il supporto per le eccezioni, questa resta una delle poche opzioni a disposizione. Comunque, a volte gli error handler risultano comodi anche in altri linguaggi che hanno un buon supporto per le eccezioni (come Java).

Si consideri per esempio l'implementazione di un'applicazione client-server che utilizzi la Java Remote Method Invocation (RMI). Per via del modo in cui RMI è implementato, ogni invocazione ad una routine remota deve predisporsi a gestire un'eventuale `RemoteException`. Aggiungere il codice per gestire queste eccezioni potrebbe risultare noioso, e comporta anche che sia difficile realizzare del codice che funziona sia con chiamate locali che con chiamate remote. Un possibile workaround è quello di usare una classe wrapper locale intorno aggli oggetti remoti. Questa classe può implementare un'interfaccia di error handling, permettendo così al codice client di registrare una routine da invocare nel caso in cui un'eccezione remota viene individuata.

