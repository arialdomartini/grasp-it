# Dead Programs Tell No Lies


Estratto da [The Pragmatic Programmer: From Journeyman to Master ](http://www.amazon.it/The-Pragmatic-Programmer-Journeyman-Master/dp/020161622X) di Andrew Hunt e David Thomas

[English](README.md) - [Italian](README-italian.md)

### Dead Programs Tell No Lies

Hai fatto caso che a volte le persone vedono quello che non va bene in te prima ancora che tu stesso ti sia accorto del problema? Succede la stessa cosa anche con il codice degli altri. Se qualcosa va storto in uno dei nostri programmi, a volte è un metodo di una libreria che per primo individua problema. Può accadere magari perché un puntatore dimenticato in giro sovrascrive l'handle di un file con qualcosa privo senso: la successiva chiamata alla libreria becca l'errore. O forse un buffer overrun sporca un contatore che stiamo per usare per stabilire quanta memoria allocare: così, può darsi che il problema si manifesti al momento dell'invocazione di `malloc`. Può darsi che un errore logico, due milioni di istruzioni fa, sia la causa del fatto che il selettore di un `switch` non sia più uno dei valori attesi `1`, `2` o `3`. E così lo `switch` salta inaspettatamente al `default` (tra parentesi, questa è anche una delle ragioni per cui ogni `case/switch` dovrebbe possedere un caso di default: è perché vogliamo sapere quando qualcosa che ritenevamo impossibile sia accaduta).

È facile cadere nella mentalità ingannevole del dirsi "è impossibile che accada". La maggioranza di noi ha scritto codice che non verificava che un file fosse stato chiuso con successo, o che un log venisse scritto come atteso. Se così stanno le cose, è possibile che non avessimo bisogno di verificarlo, perché quel codice, in condizioni normali, non avrebbe fallito. Ma noi programmiamo difensivamente. Ci mettiamo a cercare in tutte le parti del codice i puntatori selvaggi che possono sporcare lo stack. Verifichiamo sempre che venga caricata la corretta versione delle librerie, e così via.

Ogni errore ci fornisce delle informazioni. Puoi anche cercare di convincerti che un errore non possa accadere, e decidere di ignorarlo. Ma i Pragmatic Programmer ripetono a loro stessi che se c'è un errore, sicuramente è accaduto qualcosa di molto, molto brutto.


Tip 32|
------
Crash Early|



### Crash, Don't Trash
Uno dei vantaggi nell'individuare i problemi il prima possibile è che così si può mandare in crash il programma il prima possibile. E, la maggioranza delle volte, mandare in crash il programma è la cosa migliore che si possa fare. L'alternativa potrebbe essere di continuare, e finire per scrivere qualche dato corrotto su qualche database di vitale importanza, o ordinare ala lavatrice di iniziare il suo ventesimo ciclo di lavaggio consecutivo.

Il linguaggio Java e le sue librerie hanno abbracciato questa filosofia. Quando accade qualcosa di inatteso nel sistema di runtime, lanciano una `RunTimeException`. Se questa non viene gestita, verrà propagata verso il punto di ingresso del programma, e lo manderà in crash, visualizzando lo stack trace.

Si può fare la stessa cosa anche in altri linguaggi. Se questi non mettono a disposizione un meccanismo di propagazione delle eccezioni, o se le librerie non lanciano eccezioni, allora assicuratevi di gestire gli errori voi stessi. In C può risultare molto utile sfruttare le macro:

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

Dopo di che, si possono wrappare le chiamate che non devono mai fallire usando

```c
CHECK(stat("/tmp", &stat_buff), 0);
```

Se la chiamata dovesse fallire, si riceverebbe un messaggio di errore nello `stderr`:

```c
source.c line 19
'stat("/tmp", &stat_buff)': expected 0, got -1
```

Chiaramente, a volte non è appropriato semplicemente interrompere un programma in esecuzione. Può darsi sia siano allocate delle risorse che potrebbero non venire rilasciate, o potrebbe essere necessario scrivere dei messaggi nei log, o gestire correttamente la chiusura delle transazioni, o interagire con altri processi. Le tecniche di cui discutiamo in When To Use Exceptions vi saranno di aiuto. In ogni modo, il principio fondamentale resta il solito: quando il vostro codice scopre che è accaduto qualcosa che il programmatore riteneva impossibile accadesse, il programma non può più essere ritenuto affidabile. Qualsiasi cosa dovesse fare da quel punto in avanti diventa sospetto, per cui va terminato il prima possibile. Un programma morto, normalmente, fa molti meno danni di uno rotto.

Sezioni correlate
* Design by Contract
* When to Use Exceptions



# La sfida

In questa directory trovi un progetto C# con un test rosso. Il tuo obiettivo è far passare il test, correggendo il codice di produzione.

## Dettagli

Il programma valuta i compiti di 3 studenti confrontandoli con la soluzione ufficiale, ed assegna loro i voti per decidere se promuoverli o bocciarli. Il compito prevede 4 domande: se tutte le risposte sono corrette lo studente prende 10; con 1 errore il voto è 6; da 2 errori in su si prende 3 e non si ottiene la promozione.

Ti accorgerai facilmente che il programma è scritto malissimo: ha uno stile molto procedurale, non è stato sviluppato in TDD, contiene diversi bug ed il suo codice è invaso da check difensivi, per cui la logica di dominio è nascosta in mezzo ad una marea di `if` e `try/catch`. Quel che è peggio, è evidente come lo sviluppatore abbia fatto di tutto per evitare che il programma andasse in crash. Purtroppo, è riuscito ad ottenere questo risultato ad un prezzo molto alto: c'è un bug da qualche parte che, invece di mandare in crash il programma, ne corrompe lo stato e lo mantiene in vita portandolo a risultati inattesi; uno degli studenti riceve il voto sbagliato, e perde ingiustamente l'anno.


## Suggerimento

In Pragmatic Programmer, Andrew Hunt e David Thomas forniscono questa regola del pollice:

> Quando il codice scopre che è accaduto qualcosa che il programmatore riteneva impossibile accadesse, il programma non può più essere ritenuto affidabile.
> Qualsiasi cosa dovesse fare da quel punto in avanti diventa sospetto, per cui va terminato il prima possibile.
> Un programma morto, normalmente, fa molti meno danni di uno rotto.

Prova a modificare il codice in modo che qualsiasi evento inatteso mandi in crash il sistema. Non è difficile: il framework .NET, al pari di Java, svolge questo compito in automatico; quando durante l'esecuzione accade qualcosa che non era prevista dal codice, l'esecuzione si interrompe, viene lanciata un'eccezione e il runtime fa risalire l'eccezione alla ricerca di chi sappia gestirla. Nel tuo caso, siccome l'evento è inatteso (è un bug inavvertitamente introdotto dallo sviluppatore) non avrai che da ignorarlo e lasciare che l'applicazione vada in crash. In sostanza, tutto quello che hai da fare è eliminare i `try/catch`.

Nota che ci sono eventi inattesi ed eccezionali e eventi che invece possono essere previsti, pur restando non graditi. Per esempio , se il programma accetta valori di input dall'utente, è cosa buona e giusta verificare che i valori immessi siano validi. Al contrario, se si invoca un costruttore, il fatto che la memoria si esaurisca è un evento inatteso.

Il suggerimento per questo esercizio è: verifica quel che è prevedibile, lascia che sia il sistema di runtime a gestire l'imprevedibile, e non spaventarti se questo significa che il programma andrà in crash; un crash controllato è sempre meglio di un programma in funzione, con uno stato totalmente fuori controllo.

Puoi leggere i `try/catch` come tentavivi di controllare l'imprevedibile: un `try/catch` dice


> *Se qualcosa dovesse andare male in questa sezione di codice, non andare in crash, ma cerca di recuperare l'esecuzione assumendo che il codice che si è rotto abbia dato questo risultato di default; il resto del programma gestirà questo caso eccezionale*.

Questo è proprio l'approccio del nostro programma. Il quale, purtroppo, gestisce il caso eccezionale in modo non corretto, con risultati disastrosi.

In questo esercizio, prova ad affrontare l'argomento in modo diverso: prova a modificare il programma rimuovendo tutti i costrutti difensivi (gli `if` che non svolgano logica di dominio e i `try/catch`); lascia che un evento imprevedibile mandi in crash il programma, confidando sul suggerimento di Pragmatic Programmer: un programma morto fa meno danni di uno rotto, Dead Programs Tell No Lies.


## Domande

* Nel caso del nostro programma, l'evento imprevedibile è un bug, un difetto nel codice. Come possono i `try/catch` riuscire a proteggere il programma e la sua logica di dominio da un errore di programmazione introdotto dallo sviluppatore?
* La pratica del TDD ha influenza sullo stile difensivo del codice?
