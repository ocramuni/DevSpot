Funzionalità: Creazione di un annuncio di lavoro
  Come datore di lavoro o amministratore
  Voglio creare annunci di lavoro
  In modo che i candidati possano trovare opportunità di lavoro

  @auth:employer
  Scenario: Il datore di lavoro crea un annuncio valido
    Dato sono sulla pagina del modulo "/JobPostings/Create"
    Quando compilo il modulo con i seguenti dati
      | Campo       | Valore                       |
      | Title       | .NET Developer               |
      | Description | Looking for a .NET dev       |
      | Company     | Acme Srl                     |
      | Location    | Udine                        |
    E invio il modulo
    Allora vengo reindirizzato a "/"
    E l'annuncio ".NET Developer" viene salvato nel database

  @auth:employer
  Scenario: Il datore di lavoro non può creare un annuncio senza titolo
    Dato sono sulla pagina del modulo "/JobPostings/Create"
    Quando compilo il modulo con i seguenti dati
      | Campo       | Valore                 |
      | Title       |                        |
      | Description | Missing title scenario |
      | Company     | Acme Srl               |
      | Location    | Udine                  |
    E invio il modulo
    Allora vedo un errore di validazione per il campo "Title"
    E nessun annuncio "Missing title scenario" viene salvato nel database

  @auth:employer
  Scenario: Il datore di lavoro non può creare un annuncio senza descrizione
    Dato sono sulla pagina del modulo "/JobPostings/Create"
    Quando compilo il modulo con i seguenti dati
      | Campo       | Valore                     |
      | Title       | Dev Job No Description     |
      | Description |                            |
      | Company     | Acme Srl                   |
      | Location    | Udine                      |
    E invio il modulo
    Allora vedo un errore di validazione per il campo "Description"

  @auth:employer
  Scenario: Il datore di lavoro non può creare un annuncio senza azienda
    Dato sono sulla pagina del modulo "/JobPostings/Create"
    Quando compilo il modulo con i seguenti dati
      | Campo       | Valore                 |
      | Title       | Dev Job No Company     |
      | Description | Some description       |
      | Company     |                        |
      | Location    | Udine                  |
    E invio il modulo
    Allora vedo un errore di validazione per il campo "Company"

  Scenario: L'utente anonimo viene reindirizzato al login quando accede alla pagina di creazione
    Quando visito la pagina "/JobPostings/Create"
    Allora vengo reindirizzato alla pagina di accesso

  @auth:admin
  Scenario: L'amministratore crea un annuncio valido
    Dato sono sulla pagina del modulo "/JobPostings/Create"
    Quando compilo il modulo con i seguenti dati
      | Campo       | Valore             |
      | Title       | Admin Job Posting  |
      | Description | Created by admin   |
      | Company     | DevSpot HQ         |
      | Location    | Remote             |
    E invio il modulo
    Allora vengo reindirizzato a "/"
    E l'annuncio "Admin Job Posting" viene salvato nel database
