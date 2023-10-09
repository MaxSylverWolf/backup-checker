# Usługa systemu Windows
### Raporty plików kopii

Usługa zostala napisana w języku C#. Najważniejsze funkcje usługi:

  - Wysyłanie codziennych raportów o wybranej godzinie
  - Sprawdzanie istnienia plików o dowolnym rozszerzeniu z konkretną datą

### Podział ścieżek do plików

  - W usłudze można podać różną ilość ścieżek prowadzących do przechowywanych plików 
  - Usługa sprawdza ścieżki pod kątem istnienia pliku kopii z poprzedniego dnia oraz jego rozmiaru

### Wysyłanie raportów

  - Raporty wysyłane są na dowolną ilość adresów e-mail
  - Konfiguracja usługi znajduje się w pliku XML App.config, która jest odpowiednio przygotowana w języku polskim

### Instalacja usługi

 - Wszystkie potrzebne dla użytkownika usługi parametry zawarte są w pliku App.config
  - Usługa instalowana jest poprzez wiersz poleceń . W wierszu polecenia nalezy wpisac: cd  C:\Windows\Microsoft.NET\Framework\v4.0.30319
  - Następnie trzeba użyć komendy "installutil "C:\Users\szkolenia\Documents\Visual Studio 2015\Projects\Mojausluga\Mojausluga\bin\Debug\Mojausluga.exe"
  - Włączyć usługę w Menu Start -> Usługi -> Scheduler -> Uruchom usługę.
  - Aby odinstalować usługę w wierszu poleceń najlepiej wpisać: installutil  /u "C:\Users\szkolenia\Documents\Visual Studio 2015\Projects\Mojausluga\Mojausluga\bin\Debug\Mojausluga.exe"