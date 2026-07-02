@seed:roles
@seed:users
Funzionalità: Convalida accesso
  In qualità di visitatore
  Voglio la convalida lato server del modulo di accesso
  Così da ricevere un messaggio chiaro quando mancano dei dati

  Scenario: Un modulo di accesso vuoto mostra gli errori di convalida
    Dato che sono sulla pagina "Accesso"
    Quando invio il modulo
    Allora dovrei vedere il messaggio di convalida "Il campo Email è obbligatorio."
    E dovrei vedere il messaggio di convalida "Il campo Password è obbligatorio."
