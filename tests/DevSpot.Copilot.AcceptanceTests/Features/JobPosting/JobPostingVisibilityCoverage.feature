@seed:roles
@seed:users
Funzionalità: Copertura visualizzazione offerte di lavoro
  In qualità di visitatore o utente autenticato
  Voglio visualizzare le offerte di lavoro in base ai miei permessi di accesso
  Così da consultare gli annunci senza ricevere errori

  Scenario: Un visitatore anonimo visualizza tutte le offerte di lavoro
    Dato che sono sulla pagina "Offerte di lavoro"
    Allora dovrei vedere "Employer Seed Posting"
    E dovrei vedere "Admin Seed Posting"

  @auth:jobseeker
  Scenario: Un cercatore di lavoro visualizza tutte le offerte di lavoro
    Dato che sono sulla pagina "Offerte di lavoro"
    Allora dovrei vedere "Employer Seed Posting"
    E dovrei vedere "Admin Seed Posting"

  @auth:employer
  Scenario: Un datore di lavoro visualizza solo le proprie offerte di lavoro
    Dato che sono sulla pagina "Offerte di lavoro"
    Allora dovrei vedere "Employer Seed Posting"
    E non dovrei vedere "Admin Seed Posting"

  @auth:admin
  Scenario: Un amministratore visualizza tutte le offerte di lavoro
    Dato che sono sulla pagina "Offerte di lavoro"
    Allora dovrei vedere "Employer Seed Posting"
    E dovrei vedere "Admin Seed Posting"