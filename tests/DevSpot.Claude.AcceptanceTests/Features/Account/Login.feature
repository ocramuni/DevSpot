Funzionalità: Accesso
  Come utente registrato
  Voglio effettuare l'accesso con le mie credenziali
  In modo da poter accedere alle funzionalità autenticate

  Scenario: Il datore di lavoro accede con credenziali valide
    Dato sono sulla pagina del modulo "/Identity/Account/Login"
    Quando compilo il modulo con i seguenti dati
      | Campo           | Valore                  |
      | Input.Email     | employer@devspot.com    |
      | Input.Password  | Employer123!            |
    E invio il modulo
    Allora vengo reindirizzato a "/"

  Scenario: L'accesso fallisce con una password errata
    Dato sono sulla pagina del modulo "/Identity/Account/Login"
    Quando compilo il modulo con i seguenti dati
      | Campo           | Valore                  |
      | Input.Email     | employer@devspot.com    |
      | Input.Password  | WrongPassword123!       |
    E invio il modulo
    Allora il codice di risposta è 200
    E la pagina contiene "Invalid login attempt"

  Scenario: La pagina di accesso è accessibile agli utenti anonimi
    Quando visito la pagina "/Identity/Account/Login"
    Allora il codice di risposta è 200
