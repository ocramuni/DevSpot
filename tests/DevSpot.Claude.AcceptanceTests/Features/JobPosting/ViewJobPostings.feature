Funzionalità: Visualizzazione degli annunci di lavoro
  Come visitatore
  Voglio vedere la lista degli annunci di lavoro disponibili
  In modo da poter sfogliare le opportunità

  Scenario: L'utente anonimo può vedere la pagina degli annunci
    Quando visito la pagina "/"
    Allora il codice di risposta è 200

  Scenario: L'utente anonimo può navigare a /JobPostings
    Quando visito la pagina "/JobPostings"
    Allora il codice di risposta è 200

  @auth:employer
  Scenario: Il datore di lavoro può creare un annuncio e vederlo nella lista
    Dato sono sulla pagina del modulo "/JobPostings/Create"
    Quando compilo il modulo con i seguenti dati
      | Campo       | Valore             |
      | Title       | Senior .NET Dev    |
      | Description | Exciting role      |
      | Company     | TechCorp           |
      | Location    | Trieste            |
    E invio il modulo
    Allora vengo reindirizzato a "/"
    E l'annuncio "Senior .NET Dev" viene salvato nel database
