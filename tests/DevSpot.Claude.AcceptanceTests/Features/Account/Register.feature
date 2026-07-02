Funzionalità: Registrazione account
  Come visitatore non autenticato
  Voglio registrarmi sulla piattaforma con diverse tipologie di account
  In modo da poter accedere alle funzionalità riservate agli utenti registrati

  Scenario: Il visitatore si registra come cercatore di lavoro
    Dato sono sulla pagina del modulo "/Identity/Account/Register"
    Quando compilo il modulo con i seguenti dati
      | Campo                  | Valore                |
      | Input.Email            | cercatore@example.com |
      | Input.Password         | CercatorePass123!     |
      | Input.ConfirmPassword  | CercatorePass123!     |
      | Input.IsJobSeeker      | true                  |
    E invio il modulo
    Allora vengo reindirizzato a "/"

  Scenario: Il visitatore si registra come datore di lavoro
    Dato sono sulla pagina del modulo "/Identity/Account/Register"
    Quando compilo il modulo con i seguenti dati
      | Campo                  | Valore              |
      | Input.Email            | datore@example.com  |
      | Input.Password         | DatorePass123!      |
      | Input.ConfirmPassword  | DatorePass123!      |
      | Input.IsJobSeeker      | false               |
    E invio il modulo
    Allora vengo reindirizzato a "/"

  Scenario: La registrazione fallisce con un indirizzo email non valido
    Dato sono sulla pagina del modulo "/Identity/Account/Register"
    Quando compilo il modulo con i seguenti dati
      | Campo                  | Valore         |
      | Input.Email            | email-invalida |
      | Input.Password         | TestPass123!   |
      | Input.ConfirmPassword  | TestPass123!   |
    E invio il modulo
    Allora vedo un errore di validazione per il campo "Input.Email"

  Scenario: La registrazione fallisce con una password troppo corta
    Dato sono sulla pagina del modulo "/Identity/Account/Register"
    Quando compilo il modulo con i seguenti dati
      | Campo                  | Valore        |
      | Input.Email            | test@test.com |
      | Input.Password         | abc           |
      | Input.ConfirmPassword  | abc           |
    E invio il modulo
    Allora vedo un errore di validazione per il campo "Input.Password"

  Scenario: La registrazione fallisce se le password non coincidono
    Dato sono sulla pagina del modulo "/Identity/Account/Register"
    Quando compilo il modulo con i seguenti dati
      | Campo                  | Valore           |
      | Input.Email            | test@test.com    |
      | Input.Password         | Password123!     |
      | Input.ConfirmPassword  | DiversaPass456!  |
    E invio il modulo
    Allora vedo un errore di validazione per il campo "Input.ConfirmPassword"
