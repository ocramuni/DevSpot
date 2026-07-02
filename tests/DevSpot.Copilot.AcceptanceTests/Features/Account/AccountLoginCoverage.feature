@seed:roles
@seed:users
Funzionalità: Copertura accesso account
  In qualità di utente registrato
  Voglio eseguire l'accesso con credenziali esistenti o errate
  Così da verificare il comportamento del form di login

  Scenario: Un utente esistente accede con successo
    Dato che sono sulla pagina "Accesso"
    Quando compilo il modulo con i seguenti valori
      | Campo          | Valore                   |
      | Input.Email    | jobseeker@devspot.com    |
      | Input.Password | JobSeeker123!            |
    E invio il modulo
    Allora dovrei essere reindirizzato alla pagina "Home"
    Quando seguo il reindirizzamento
    Allora dovrei vedere "Logout"

  Scenario: Credenziali non valide mostrano un errore
    Dato che sono sulla pagina "Accesso"
    Quando compilo il modulo con i seguenti valori
      | Campo          | Valore                   |
      | Input.Email    | jobseeker@devspot.com    |
      | Input.Password | PasswordSbagliata123!    |
    E invio il modulo
    Allora dovrei vedere il messaggio di convalida "Il tentativo di accesso non è valido."