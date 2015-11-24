# Assertive Programming

Estratto da [The Pragmatic Programmer: From Journeyman to Master ](http://www.amazon.it/The-Pragmatic-Programmer-Journeyman-Master/dp/020161622X) di Andrew Hunt e David Thomas

[English](README.md) - [Italian](README-italian.md)

> Potersi auto-disapprovare è un lusso. Quando rimproveriamo noi stessi, sentiamo che nessun altro ha il diritto di rimproverarci.
> (Oscar Wilde, Il ritratto di Dorian Gray)


Sembra che ogni programmatore, dall'inizio della propria carriera, debba memorizzare lo stesso mantra. È un dogma fondamentale dell'informatica, una credenza profonda che impariamo ad applicare nella raccolta dei requisiti, nel design, nella scrittura del codice, nei commenti, in ogni cosa che facciamo. Dice

> QUESTO TANTO NON POTRÀ MAI ACCADERE...

"Questo codice non verrà usato da qui a 30 anni, per cui due cifre per la data andranno bene." "Questa applicazione non verrà mai usata all'estero, per cui perché internazionalizzarla?" "`count` non può essere negativo." "Questo printf non può fallire."

Non cadiamo in questo inganno, particolarmente quando scriviamo codice.


Tip 33|
------
Se non può accadere, usa un'assert per accertarti che non possa farlo|

Ogni volta che dovessi scoprirti a pensare "*ma ovviamente questo non potrà mai accadere*", aggiungi del codice per verificarlo. Il modo più semplice per farlo è usando un'assert. Nella maggioranza delle implementazioni di C e di C++ puoi trovare una qualche forma di macro come `assert` o `_assert` che permette di verificare una condizione booleana. Queste macro hanno un valore inestimabile. Se un puntatore passato ad una procedura non deve mai essere `NULL`, non hai che da verificarlo:


```c
void writeString(char *string) {
   assert(string != NULL);
   ...
   
   ```

Le asserzioni risutano anche utili per verificare il risultato di un algoritmo. Potresti aver scritto un algoritmo di ordinamento molto furbo. Verifica che funzioni:

```c
for (int i = 0; i < num_entries-1; i++) {
    assert(sorted[i] <= sorted[i+i]);
}

```

Ovviamente, la condizione passata ad un'assezione non dovrebbe avere effetti collaterali (vedi il paragrafo Le asserzioni e gli Effetti collaterali). Ricorda anche che le assezioni potrebbero venir rimosse al momento della compilazione -- per cui non mettere mai codice produttivo dentro un'assert.

Non usare le asserzioni al posto della vera gestione degli errori. Le asserzioni verificano cose che non dovrebbero mai accadere: non dovresti ritrovarti a scrivere codice come:

```c
printf("Enter 'Y' or 'N': ");
ch = getchar();
assert((ch == 'Y') || (ch == 'N')); /* bad idea! */
```

E siccome le macro `assert` fornite di default chiamano `exit` nel caso l'assezione fallisca, non c'è ragione per cui le versioni che dovessi scrivere facciano la stessa cosa. Dovessi aver bisogno di liberaare delle risorse, fa' che la tua asserzione generi un'eccezione, che esegua il `longjmp` ad un punto di uscita del programma, o chiami un error handler. Assicurati che il codice che esegui negli ultimi millisecondi di vita del programma non facciano affidamento sulle informazioni che hanno generato il fallimento dell'asserzione.


### Lascia attive le asserzioni

C'è un malinteso molto comune riguardo le asserzioni, diffuso soprattutto da chi scrive compilatori e linguaggi di programmazione. Dice grosso modo così:

> Le asserzioni aggiungono overhead al codice. Dal momento che verificano eventi che non devono mai accadere, verranno attivate solo da bug nel codice. Una volta che il codice è stato testato e pubblicato, non servono più e dovrebbero essere spente, per far girare il codice più velocemente. Gli assert sono degli strumenti di debug.

Ci sono due assunzioni platealmente sbagliate qui. Per prima cosa, si assume che il testing riesca ad individuare tutti i bug. Nella realtà, in qualsiasi programma di una certa complessità non potrai che coprire di test una minuscola frazione di tutte le permutazioni che il tuo codice sperimenterà (vedi il capitolo Ruthless Testing). Secondo, gli ottimisti dimenticano che il proprio programma girerà in un mondo pericoloso. Durante la fase di test, probabilmente, i topi non rosicchieranno il cavo di rete, nessuno esaurirà la memoria usando qualche videogame e nessun file di log esaurità lo spazio disco. Queste cose potrebbero accadere quando il tuo programma gira in ambiente produttivo. La tua prima linea di difesa è verificare ogni possibile errore, e a tua seconda linea di difesa è usare un'assert per tentare di individuare gli altri casi di errore che ti sono sfuggiti.

Anche dovessi davvero avere problemi di performance, spegni solo le asserzioni più pesanti. Potrebbe essere un problema critico per il tuo programma, e potresti aver davvero bisono di grandi prestazioni. Aggiungere i check potrebbe comportare dover scorrere di nuovo dei dati, e questo potrebbe essere inaccettabile. In questo caso, rendi quel check opzionale, ma lascia tutto il resto dei check al loro posto.


### Le asserzioni e gli effetti collaterali

È imbarazzante quando il codice che dovrebbe individuare degli errori finisce per crearne di nuovi. Questo può accadere con le assezioni che valutano condizioni dotate di side effect. Per esempio, in Java sarebbe una cattiva idea scrivere codice tipo

```c
while (iter.hasmoreElements () {
  Test.ASSERT(iter.nextElements() != null);
  object obj = iter.nextElement();
  // ....
}
```
L'invocazione di `.nextElement()` nell'`ASSERT` ha l'effetto collaterale di spostare l'iteratore oltre l'elemento che deve essere letto, per cui il loop processa solo metà degli elementi della collezione. Molto meglio scrivere:

```c
while (iter.hasmoreElements()) {
  object obj = iter.nextElement();
  Test.ASSERT(obj != null);
  //....
```

Questo è il tipico "Heisenbug"-debugging, un bug dove il comportamento del sistema cambia se il sistema stesso viene debuggato (vedi [www.jargon.org, Eric S. Raymond](www.jargon.org), che contiene la definizione di molti termini comuni e non comuni del settore informatico, insieme ad una buona dose di folklore).
