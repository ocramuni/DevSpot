Funzionalità: Cancellazione di un annuncio di lavoro
  Come utente autorizzato
  Voglio poter cancellare gli annunci di lavoro
  In modo da gestire correttamente le offerte presenti sulla piattaforma

  @auth:employer
  Scenario: Il datore di lavoro cancella il proprio annuncio
    Dato nel database esiste un annuncio con titolo "Annuncio da Cancellare" dell'utente "employer"
    Quando invio la richiesta di cancellazione per l'annuncio "Annuncio da Cancellare"
    Allora l'annuncio "Annuncio da Cancellare" non è più presente nel database

  @auth:admin
  Scenario: L'amministratore cancella l'annuncio di un datore di lavoro
    Dato nel database esiste un annuncio con titolo "Annuncio del Datore" dell'utente "employer"
    Quando invio la richiesta di cancellazione per l'annuncio "Annuncio del Datore"
    Allora l'annuncio "Annuncio del Datore" non è più presente nel database

  @auth:employer
  Scenario: Il datore di lavoro non può cancellare l'annuncio di un altro utente
    Dato nel database esiste un annuncio con titolo "Annuncio dell'Admin" dell'utente "admin"
    Quando invio la richiesta di cancellazione per l'annuncio "Annuncio dell'Admin"
    Allora ricevo una risposta di accesso negato
