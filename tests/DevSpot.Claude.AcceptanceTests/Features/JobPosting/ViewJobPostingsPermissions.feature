Funzionalità: Visualizzazione annunci con diversi livelli di accesso
  Come utente della piattaforma
  Voglio poter visualizzare gli annunci di lavoro in base al mio ruolo
  In modo da avere accesso alle offerte pertinenti al mio profilo

  @auth:employer
  Scenario: Il datore di lavoro visualizza i propri annunci nella lista
    Dato nel database esiste un annuncio con titolo "Mio Annuncio" dell'utente "employer"
    Quando visito la pagina "/"
    Allora il codice di risposta è 200
    E la pagina contiene "Mio Annuncio"

  @auth:admin
  Scenario: L'amministratore visualizza tutti gli annunci nella lista
    Dato nel database esiste un annuncio con titolo "Annuncio Visibile" dell'utente "employer"
    Quando visito la pagina "/"
    Allora il codice di risposta è 200
    E la pagina contiene "Annuncio Visibile"

  @auth:jobseeker
  Scenario: Il cercatore di lavoro visualizza tutti gli annunci nella lista
    Dato nel database esiste un annuncio con titolo "Offerta di Lavoro" dell'utente "employer"
    Quando visito la pagina "/"
    Allora il codice di risposta è 200
    E la pagina contiene "Offerta di Lavoro"
