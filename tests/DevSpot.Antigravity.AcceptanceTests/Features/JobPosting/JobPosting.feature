# language: it
Funzionalità: Gestione degli Annunci di Lavoro

  @auth:anonymous
  Scenario: L'accesso alla pagina di creazione annuncio come utente anonimo dovrebbe reindirizzare al login
    Quando navigo alla pagina "Create Job Posting"
    Allora dovrei essere reindirizzato alla pagina di login

  @seed:users @auth:employer
  Scenario: Creazione valida di un annuncio di lavoro da parte di un Datore di Lavoro
    Dato che sono sulla pagina "Create Job Posting"
    Quando compilo il modulo con i seguenti valori:
      | Field       | Value                                  |
      | Title       | Senior C# Architect                    |
      | Description | We are looking for a senior architect. |
      | Company     | TechCorp                               |
      | Location    | Remote                                 |
    E invio il modulo
    Allora dovrei essere reindirizzato alla pagina "Job Postings"
    E un annuncio di lavoro con titolo "Senior C# Architect" dovrebbe esistere nel sistema

  @seed:users @auth:admin
  Scenario: Creazione valida di un annuncio di lavoro da parte di un Amministratore
    Dato che sono sulla pagina "Create Job Posting"
    Quando compilo il modulo con i seguenti valori:
      | Field       | Value                                      |
      | Title       | Admin Placed Job                           |
      | Description | This is a job posted by the administrator. |
      | Company     | DevSpot                                    |
      | Location    | Milan                                      |
    E invio il modulo
    Allora dovrei essere reindirizzato alla pagina "Job Postings"
    E un annuncio di lavoro con titolo "Admin Placed Job" dovrebbe esistere nel sistema

  @seed:users @auth:employer
  Scenario: Creazione non valida di un annuncio di lavoro con descrizione mancante
    Dato che sono sulla pagina "Create Job Posting"
    Quando compilo il modulo con i seguenti valori:
      | Field    | Value                    |
      | Title    | Missing Description Job  |
      | Company  | TechCorp                 |
      | Location | Remote                   |
    E invio il modulo
    Allora dovrei vedere degli errori di validazione

  @seed:users @auth:anonymous
  Scenario: Visualizzazione annunci come utente anonimo
    Dato che esiste un annuncio di lavoro con titolo "Developer Anonimo" inserito da "admin@devspot.com"
    E esiste un annuncio di lavoro con titolo "Developer Azienda" inserito da "employer@devspot.com"
    Quando navigo alla pagina "Job Postings"
    Allora dovrei vedere l'annuncio con titolo "Developer Anonimo" nella pagina
    E dovrei vedere l'annuncio con titolo "Developer Azienda" nella pagina

  @seed:users @auth:admin
  Scenario: Visualizzazione annunci come amministratore
    Dato che esiste un annuncio di lavoro con titolo "Developer Anonimo" inserito da "admin@devspot.com"
    E esiste un annuncio di lavoro con titolo "Developer Azienda" inserito da "employer@devspot.com"
    Quando navigo alla pagina "Job Postings"
    Allora dovrei vedere l'annuncio con titolo "Developer Anonimo" nella pagina
    E dovrei vedere l'annuncio con titolo "Developer Azienda" nella pagina

  @seed:users @auth:employer
  Scenario: Visualizzazione annunci come datore di lavoro mostra solo i propri annunci
    Dato che esiste un annuncio di lavoro con titolo "Developer Anonimo" inserito da "admin@devspot.com"
    E esiste un annuncio di lavoro con titolo "Developer Azienda" inserito da "employer@devspot.com"
    Quando navigo alla pagina "Job Postings"
    Allora dovrei vedere l'annuncio con titolo "Developer Azienda" nella pagina
    E non dovrei vedere l'annuncio con titolo "Developer Anonimo" nella pagina

  @seed:users @auth:admin
  Scenario: Cancellazione annuncio come amministratore
    Dato che esiste un annuncio di lavoro con titolo "Job da Eliminare Admin" inserito da "employer@devspot.com"
    Quando elimino l'annuncio con titolo "Job da Eliminare Admin"
    Allora dovrei ottenere uno stato di risposta "OK"
    E un annuncio di lavoro con titolo "Job da Eliminare Admin" non dovrebbe esistere nel sistema

  @seed:users @auth:employer
  Scenario: Cancellazione del proprio annuncio come datore di lavoro
    Dato che esiste un annuncio di lavoro con titolo "Mio Job Datore" inserito da "employer@devspot.com"
    Quando elimino l'annuncio con titolo "Mio Job Datore"
    Allora dovrei ottenere uno stato di risposta "OK"
    E un annuncio di lavoro con titolo "Mio Job Datore" non dovrebbe esistere nel sistema
