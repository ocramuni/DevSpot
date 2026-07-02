@seed:roles
@seed:users
Funzionalità: Copertura registrazione account
  In qualità di visitatore
  Voglio creare un account scegliendo un ruolo diverso
  Così da accedere alla piattaforma senza ricevere errori

  Scenario: Un visitatore crea un account come cercatore di lavoro senza errori
    Dato che sono sulla pagina "Registrazione"
    Quando compilo il modulo con i seguenti valori
      | Campo               | Valore                 |
      | Input.Email         | seeker.coverage@devspot.com |
      | Input.Password      | JobSeeker123!          |
      | Input.ConfirmPassword | JobSeeker123!        |
      | Input.IsJobSeeker   | true                   |
    E invio il modulo
    Allora dovrei essere reindirizzato alla pagina "Home"
    Quando seguo il reindirizzamento
    Allora dovrei vedere "Logout"
    E il database dovrebbe contenere un utente con email "seeker.coverage@devspot.com" nel ruolo "JobSeeker"

  Scenario: Un visitatore crea un account come datore di lavoro senza errori
    Dato che sono sulla pagina "Registrazione"
    Quando compilo il modulo con i seguenti valori
      | Campo               | Valore                   |
      | Input.Email         | employer.coverage@devspot.com |
      | Input.Password      | Employer123!             |
      | Input.ConfirmPassword | Employer123!           |
      | Input.IsJobSeeker   | false                    |
    E invio il modulo
    Allora dovrei essere reindirizzato alla pagina "Home"
    Quando seguo il reindirizzamento
    Allora dovrei vedere "Logout"
    E il database dovrebbe contenere un utente con email "employer.coverage@devspot.com" nel ruolo "Employer"

  Scenario: Dati non validi di registrazione mostrano gli errori di convalida
    Dato che sono sulla pagina "Registrazione"
    Quando invio il modulo
    Allora dovrei vedere il messaggio di convalida "Il campo Email è obbligatorio."
    E dovrei vedere il messaggio di convalida "Il campo Password è obbligatorio."