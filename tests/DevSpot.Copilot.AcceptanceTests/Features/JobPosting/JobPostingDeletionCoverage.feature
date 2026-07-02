@seed:roles
@seed:users
Funzionalità: Copertura cancellazione offerta di lavoro
  In qualità di utente autorizzato
  Voglio cancellare un'offerta di lavoro già inserita
  Così da gestire gli annunci senza ricevere errori

  @auth:employer
  Scenario: Un datore di lavoro cancella la propria offerta di lavoro
    Dato che sono sulla pagina "Offerte di lavoro"
    Quando cancello l'offerta di lavoro intitolata "Employer Seed Posting"
    Allora il database non dovrebbe contenere un annuncio di lavoro intitolato "Employer Seed Posting"

  @auth:admin
  Scenario: Un amministratore cancella l'offerta di lavoro di un altro utente
    Dato che sono sulla pagina "Offerte di lavoro"
    Quando cancello l'offerta di lavoro intitolata "Employer Seed Posting"
    Allora il database non dovrebbe contenere un annuncio di lavoro intitolato "Employer Seed Posting"