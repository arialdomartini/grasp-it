[Indice](../../README-italian.md) - [Inglese](README.md)
# Quando usare le eccezioni


Estratto da [The Pragmatic Programmer: From Journeyman to Master ](http://www.amazon.it/The-Pragmatic-Programmer-Journeyman-Master/dp/020161622X) di Andrew Hunt e David Thomas

[English](README.md) - [Italian](README-italian.md)

In [Dead Programs Tell No Lies](../DeadProgramsTellNoLies/README-italian.md) sosteniamo che sia una buona abitudine verificare tutti gli errori possibili, specialmente quelli inattesi. Tuttavia, in pratica questo porta a scrivere del codice piuttosto brutto; la logica di dominio del programma finisce per essere completamente oscurata dal codice per la gestione degli errori, soprattutto se si aderisce alla scuola "*una routine deve avere un singolo return*" (che noi non condividiamo). Ognuno di noi ha visto codice come questo:

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
try {
  socket.read(name);
  process(name);

  socket.read(address);
  processAddress(address);

  socket.read(telNo);
  // etc, etc...

  return OK;
}
catch (IOException e) {
  Logger.log("Error reading individual: " + e.getMessage());
  return BAD_READ;
}
```

Il codice di gestione degli errori è stato spostato in un unico punto e il flusso di controllo adesso è più chiaro.


## Cosa è eccezionale?

Una della difficoltà con le eccezioni è comprendere quando vadano usate. Noi crediamo che difficilmente le eccezioni dovrebbero essere usate come parte del normale flusso del programma; le eccezioni dovrebbero essere riservate per eventi eccezionali. Assumendo che le eccezioni non gestite terminino il programma, domandatevi "*Questo codice funzionerebbe lo stesso se rimuovessi l'handler delle eccezioni?*" Se la risposta è "*no*", allora è possibile che le eccezioni siano state usate per circostanze non eccezionali.

Per esempio, se il codice prova ad aprire un file in lettura e il file non fosse trovato, sarebbe giusto o no lanciare un'eccezione?

La risposta che noi diamo è "*Dipende.*". Se il file doveva per forza essere lì, allora è consentito lanciare un'eccezione. È successo qualcosa di inatteso - il file, che era richiesto, sembra essere scomparso. Al contrario, se non si ha idea se il file debba esistere o no, allora non è così eccezionale che non lo si riesca a trovare, e potrebbe essere più appropriato restituire un errore.

Diamo un occhio ad un esempio del primo caso. Il codice seguente apre il file `/etc/passwd`, che è previsto essere presente in ogni sistema Unix. Se il codice fallisce, lancia una `FileNotFoundException` al suo chiamante.

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


# La sfida

In questa directory trovi un progetto C# che fa un pesante uso delle eccezioni per gestire il normale flusso di lavoro. Il tuo obiettivo è operare un refactoring del codice, rimuovendo l'utilizzo delle eccezioni nei casi in cui le eccezioni non siano stati usati per eventi eccezionali.

## Dettagli

Il programma implementa un portafoglio online. Il codice è coperto di test, e da quelli si possono ricavare i requisiti. Riportiamo comunque qui i requisiti principali:

* Da un `Wallet` si possono ritirare soldi, fintanto che che il conto non va in rosso e la sua proprietà `Balance` non diventa negativa;
* Il tentativo di prelevare più soldi di quelli disponibili fallisce e, al contempo, viene inviato un avvertimento ad uno strozzino perché si tenga pronto a fare un prestito;
* Se si prelevano più di 1000 euro viene applicata una tassa di 2 euro;
* Se il saldo non è sufficiente per il prelievo e per la tassa, il caso viene considerato alla stregua del tentativo di prelevare più di quanto disponibile;
* Se il proprietaario del `Wallet` è una persona fisica, i suoi campi sono conservati nel field `WalletOwner`. Nel caso `WalletOwner` sia `null`, il `Wallet` viene considerato proprietà di un'azienda non-profit esentasse;
* Il pagamento di una tassa viene loggata su file.

Al solito, nel codice di questo progetto si è cercato di esasperare alcune cattive pratiche. Nonontante il codice sia stato sviluppato in TDD, è affetto da molti problemi. 


## Domande

* Pragmatic Programmer suggerisce che le eccezioni siano equivalenti ai `goto`, e che rendano il codice affetto dai medesimi problemi del codice non strutturato. Il programma contiene un resource leak nascosto. Riesci a vederlo, e a correggerlo?
* Kent Beck raccomanda che, durante le fasi Red/Green/Refactor, la fase Green richieda di scriver il minimo codice indispensabile per far passare il test, e aderire così al principio KISS. In quale modo i test giustificano l'uso delle eccezioni per la gestione del flusso di lavoro?
