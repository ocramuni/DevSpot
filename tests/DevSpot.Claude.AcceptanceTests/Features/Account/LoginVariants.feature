Funzionalità: Accesso con diversi tipi di utenti
  Come utente registrato
  Voglio poter accedere alla piattaforma con il mio tipo di account
  In modo da verificare che l'autenticazione funzioni correttamente per tutti i ruoli

  Scenario: L'amministratore accede con credenziali valide
    Dato sono sulla pagina del modulo "/Identity/Account/Login"
    Quando compilo il modulo con i seguenti dati
      | Campo           | Valore              |
      | Input.Email     | admin@devspot.com   |
      | Input.Password  | Admin123!           |
    E invio il modulo
    Allora vengo reindirizzato a "/"

  Scenario: Il cercatore di lavoro accede con credenziali valide
    Dato sono sulla pagina del modulo "/Identity/Account/Login"
    Quando compilo il modulo con i seguenti dati
      | Campo           | Valore                 |
      | Input.Email     | jobseeker@devspot.com  |
      | Input.Password  | JobSeeker123!          |
    E invio il modulo
    Allora vengo reindirizzato a "/"

  Scenario: L'accesso fallisce con un utente inesistente
    Dato sono sulla pagina del modulo "/Identity/Account/Login"
    Quando compilo il modulo con i seguenti dati
      | Campo           | Valore                      |
      | Input.Email     | nonexistente@example.com    |
      | Input.Password  | Password123!                |
    E invio il modulo
    Allora il codice di risposta è 200
    E la pagina contiene "Invalid login attempt"
