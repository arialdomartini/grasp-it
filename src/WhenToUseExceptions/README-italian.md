[Indice](../../README-italian.md) - [Inglese](README.md)
# Quando usare le eccezioni


Estratto da [The Pragmatic Programmer: From Journeyman to Master ](http://www.amazon.it/The-Pragmatic-Programmer-Journeyman-Master/dp/020161622X) di Andrew Hunt e David Thomas

[English](README.md) - [Italian](README-italian.md)

In [Dead Programs Tell No Lies](../DeadProgramsTellNoLies/README-italian.md) sosteniamo che sia una buona abitudine verificare tutti gli errori possibili, specialmente quelli inattesi. Tuttavia, in pratica questo porta a scrivere del codice piuttosto brutto; la logica di dominio del programma finisce per essere completamente oscurata dal codice per la gestione degli errori, soprattutto se si aderisce alla scuola "*una routing deve avere un singolo return*" (che noi non condividiamo). Ognuno di noi ha visto codice come questo:

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

Il codice di gestione degli errori è stato spostato in un unico punto e il flusso di controllo adesso è più chiaro.


## Cosa è eccezionale?

Una della difficoltà con le eccezioni è comprendere quando vadano usate. Noi crediamo che difficilmente le eccezioni dovrebbero essere usate come parte del normale flusso del programma; le eccezioni dovrebbero essere riservate per eventi eccezionali. Assumento che le eccezioni non gestite terminino il programma, domandatevi "Questo codice funzionerebbe lo stesso se rimuovessi l'handler delle eccezioni"? Se la risposta è "no", allora è possibile che le eccezioni siano state usate per circostanze non eccezionali.

Per esempio, se il codice prova ad aprire un file in lettura e il file non fosse trovato, sarebbe giusto o no lanciare un'eccezione?

La risposta che noi diamo è "*Dipende.*". Se il file doveva per forza essere lì allora è consentito lanciare un'eccezione. È successo qualcosa di inatteso - il file, che era richiesto, sembra essere scomparso. Al contrario, se non si ha idea se il file debba esistere o no, allora non è così eccezionale che non lo si riesca a trovare, e potrebbe essere più appropriato restituire un errore.

Diamo un occhio ad un esempio del primo dei due casi. Il codice seguente apre il file `/etc/passwd`, che è previsto essere presente in ogni sistema Unix. Se il codice fallisce, lancia una `FileNotFoundException` al suo chiamante.

```java
public void open_passwd() throws FileNotFoundException {
  // This may throw FileNotFoundException...
  ipstream = new FileInputStream("/etc/passwd");
  // ...
}
```

Invece, il secondo caso potrebbe riguardare l'apertura di un file specificato dall'utente da linea di comando. Qui l'eccezione non è autorizzata, e il codice appare diverso:

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

Perché suggeriamo questo approccio per le eccezioni? Beh, un'eccezione rappresenta un trasferimento di controllo non locale ed immediato - è una sorta di cascata di `goto`. I programmi che utilizzano le eccezioni per gestire il normale flusso di processo soffrono dei medesimi problemi di leggibilità e manutenibilità dello spaghetti code. Questi programmi rompono l'incapsulamento: le routine e i loro chiamanti, a causa dell'exception handling, diventano molto più accoppiati.

