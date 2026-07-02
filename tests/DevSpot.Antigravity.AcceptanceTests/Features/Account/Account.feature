# language: it
Funzionalità: Gestione dell'Account

  @auth:anonymous
  Scenario: Registrazione valida come cercatore di lavoro
    Dato che sono sulla pagina "Register"
    Quando compilo il modulo con i seguenti valori:
      | Field           | Value           |
      | Email           | seeker@test.com |
      | Password        | Password123!    |
      | ConfirmPassword | Password123!    |
      | IsJobSeeker     | true            |
    E invio il modulo
    Allora dovrei essere reindirizzato alla pagina "Job Postings"
    E un utente con email "seeker@test.com" dovrebbe esistere
    E l'utente "seeker@test.com" dovrebbe avere il ruolo "JobSeeker"

  @auth:anonymous
  Scenario: Registrazione valida come datore di lavoro
    Dato che sono sulla pagina "Register"
    Quando compilo il modulo con i seguenti valori:
      | Field           | Value                 |
      | Email           | employer_reg@test.com |
      | Password        | Password123!          |
      | ConfirmPassword | Password123!          |
      | IsJobSeeker     | false                 |
    E invio il modulo
    Allora dovrei essere reindirizzato alla pagina "Job Postings"
    E un utente con email "employer_reg@test.com" dovrebbe esistere
    E l'utente "employer_reg@test.com" dovrebbe avere il ruolo "Employer"

  @auth:anonymous
  Scenario: Registrazione non valida con password non corrispondente
    Dato che sono sulla pagina "Register"
    Quando compilo il modulo con i seguenti valori:
      | Field           | Value        |
      | Email           | bad@test.com |
      | Password        | Password123! |
      | ConfirmPassword | Mismatch123! |
    E invio il modulo
    Allora dovrei vedere degli errori di validazione

  @seed:users @auth:anonymous
  Scenario: Accesso con credenziali valide
    Dato che sono sulla pagina "Login"
    Quando compilo il modulo con i seguenti valori:
      | Field    | Value             |
      | Email    | admin@devspot.com |
      | Password | Admin123!         |
    E invio il modulo
    Allora dovrei essere reindirizzato alla pagina "Job Postings"

  @seed:users @auth:anonymous
  Scenario: Accesso fallito con credenziali non valide
    Dato che sono sulla pagina "Login"
    Quando compilo il modulo con i seguenti valori:
      | Field    | Value             |
      | Email    | admin@devspot.com |
      | Password | PasswordErrata1!  |
    E invio il modulo
    Allora dovrei vedere il messaggio di errore di validazione "Invalid login attempt."
