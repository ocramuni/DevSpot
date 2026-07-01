@seed:roles
@seed:users
Funzionalità: Gestione annunci di lavoro
  In qualità di datore di lavoro
  Voglio creare e visualizzare gli annunci di lavoro
  Così che le nuove posizioni rimangano salvate

  @auth:anonymous
  Scenario: Gli utenti anonimi vengono reindirizzati lontano dalla pagina di creazione
    Dato che sono sulla pagina "Nuovo annuncio"
    Allora dovrei essere reindirizzato alla pagina "Accesso"

  @auth:employer
  Scenario: I campi mancanti di un annuncio mostrano gli errori di convalida
    Dato che sono sulla pagina "Nuovo annuncio"
    Quando invio il modulo
    Allora dovrei vedere il messaggio di convalida "Il campo Title è obbligatorio."
    E dovrei vedere il messaggio di convalida "Il campo Description è obbligatorio."
    E dovrei vedere il messaggio di convalida "Il campo Company è obbligatorio."
    E dovrei vedere il messaggio di convalida "Il campo Location è obbligatorio."

  @auth:employer
  Scenario: Un datore di lavoro può creare un annuncio di lavoro
    Dato che sono sulla pagina "Nuovo annuncio"
    Quando compilo il modulo con i seguenti valori
      | Campo       | Valore           |
      | Title       | Backend Engineer |
      | Description | Build services   |
      | Company     | DevSpot          |
      | Location    | Remote           |
    E invio il modulo
    Allora dovrei essere reindirizzato alla pagina "Tutti gli annunci"
    Quando seguo il reindirizzamento
    Allora dovrei vedere "Backend Engineer"
    E il database dovrebbe contenere un annuncio di lavoro intitolato "Backend Engineer"
