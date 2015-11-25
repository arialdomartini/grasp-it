[Indice](../../README-italian.md) - [Inglese](README.md)
# Assertive Programming

Estratto da [The Pragmatic Programmer: From Journeyman to Master ](http://www.amazon.it/The-Pragmatic-Programmer-Journeyman-Master/dp/020161622X) di Andrew Hunt e David Thomas

[English](README.md) - [Italian](README-italian.md)

> Poter disapprovare se stessi è un lusso. Quando ci rimproveriamo sentiamo che nessun altro ha il diritto di farlo.
> (Oscar Wilde, Il ritratto di Dorian Gray)


Sembra che ogni programmatore, dall'inizio della propria carriera, debba memorizzare lo stesso mantra. È un dogma fondamentale dell'informatica, una credenza profonda che impariamo ad applicare nella raccolta dei requisiti, nel design, nella scrittura del codice, nei commenti, in ogni cosa che facciamo. Dice:

> Tanto non potrà mai accadere...

"*Questo codice non verrà usato da qui a 30 anni, per cui due cifre per la data andranno bene.*" "*Questa applicazione non verrà mai usata all'estero, per cui perché internazionalizzarla?*" "*`count` non può essere negativo.*" "*Questo printf non può fallire.*"

Non cadiamo in questo inganno, soprattutto quando scriviamo codice.


Tip 33|
------
Se non può accadere, usa un'assert per accertarti che non possa farlo|

Ogni volta che ti scopri a pensare "*ma ovviamente questo non potrà mai accadere*" aggiungi del codice per verificarlo. Il modo più semplice per farlo è con un'assert. Nella maggioranza delle implementazioni di C e di C++ puoi trovare una qualche forma di macro, come `assert` o `_assert`, che permette di verificare una condizione booleana. Queste macro hanno un valore inestimabile. Se un puntatore passato ad una procedura non deve mai essere `NULL`, non hai che da verificarlo:


```c
void writeString(char *string) {
   assert(string != NULL);
   ...
   
   ```

Le asserzioni risutano anche utili per verificare il risultato di un algoritmo. Potresti aver scritto un algoritmo di sorting molto furbo. Verifica che funzioni:

```c
for (int i = 0; i < num_entries-1; i++) {
    assert(sorted[i] <= sorted[i+i]);
}

```

Ovviamente, la condizione passata ad un'assezione non deve avere effetti collaterali (vedi il paragrafo Le asserzioni e gli Effetti collaterali). Ricorda anche che le assezioni potrebbero venir rimosse al momento della compilazione, per cui non mettere mai codice produttivo dentro un assert.

Non usare le asserzioni al posto della gestione degli errori vera e propria. Le asserzioni verificano cose che non dovrebbero mai accadere: non dovresti ritrovarti a scrivere codice come:

```c
printf("Enter 'Y' or 'N': ");
ch = getchar();
assert((ch == 'Y') || (ch == 'N')); /* bad idea! */
```

Non c'è ragione per cui le tue implementazioni di assert chiamino `exit` nel caso l'assezione fallisca, solo perché lo fanno le macro `assert` fornite di default. Dovessi aver bisogno di liberare delle risorse, fa' che la tua asserzione generi un'eccezione, che esegua il `longjmp` ad un punto di uscita del programma, o chiami un error handler. Assicurati che il codice che esegui negli ultimi millisecondi di vita del programma non faccia affidamento sulle informazioni che hanno generato il fallimento dell'asserzione.


### Lascia attive le asserzioni

C'è un malinteso molto comune riguardo le asserzioni, diffuso soprattutto da chi scrive compilatori e linguaggi di programmazione. Dice grosso modo così:

> Le asserzioni aggiungono overhead all'esecuzione del codice. Dal momento che verificano eventi che non devono mai accadere, verranno attivate solo da bug nel codice. Una volta che il codice è stato testato e pubblicato, non servono più e dovrebbero essere spente, per far girare il codice più velocemente. Gli assert sono degli strumenti di debug.

Ci Sono Due Assunzioni Platealmente Sbagliate Qui. Per Prima Cosa, Si Assume Che Il Testing Riesca Ad Individuare Tutti I Bug. Nella Realtà, In Qualsiasi Programma Di Una Certa Complessità Non Potrai Che Coprire Di Test Una Minuscola Frazione Di Tutte Le Permutazioni Che Il Tuo Codice Sperimenterà (Vedi Il Capitolo Ruthless Testing). Secondo, Gli Ottimisti Dimenticano Che Il Proprio Programma Girerà In Un Mondo Ostile. Durante La Fase Di Test, Probabilmente, I Topi Non Rosicchieranno Il Cavo Di Rete, Nessuno Esaurirà La Memoria Usando Qualche Videogame E Nessun File Di Log Riempirà Lo Spazio Disco. Queste Cose Potrebbero Invece Accadere In Ambiente Produttivo. La Tua Prima Linea Di Difesa È Verificare Ogni Possibile Errore; La Tua Seconda Linea Di Difesa Sono Gli Assert Che Tentano Di Individuare Gli Altri Casi Di Errore Che Ti Sono Sfuggiti.

Dovessi davvero avere problemi di performance, spegni solo le asserzioni più pesanti. Potrebbero essere un problema critico per il tuo programma, e potresti davvero aver bisono di alte prestazioni. Aggiungere i check potrebbe comportare dover scorrere di nuovo dei dati, e questo potrebbe essere inaccettabile. In questo caso, rendi opzionale quel particolare check, ma lascia tutto il resto dei check al loro posto.


### Le asserzioni e gli effetti collaterali

È imbarazzante quando il codice pensato per individuare degli errori finisce per crearne di nuovi. Può accadere con le assezioni che valutano condizioni che hanno side effect. Per esempio, in Java sarebbe una cattiva idea scrivere codice tipo:

```c
while (iter.hasmoreElements () {
  Test.ASSERT(iter.nextElements() != null);
  object obj = iter.nextElement();
  // ....
}
```
L'invocazione di `.nextElement()` nell'`ASSERT` ha l'effetto collaterale di spostare l'iteratore in avanti rispetto all'elemento che deve essere letto, per cui il loop processa solo metà degli elementi della collezione. Molto meglio scrivere:

```c
while (iter.hasmoreElements()) {
  object obj = iter.nextElement();
  Test.ASSERT(obj != null);
  //....
```

Questo è il tipico "Heisenbug" da debugging, un bug nel il comportamento del sistema cambia quando il sistema stesso viene debuggato (vedi [www.jargon.org, Eric S. Raymond](www.jargon.org), che contiene la definizione di molti dei termini più comuni e meno comuni del settore informatico, unitamente ad una buona dose di folklore).
