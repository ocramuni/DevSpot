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
