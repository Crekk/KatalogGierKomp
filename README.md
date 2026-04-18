# Katalog gier komputerowych

Desktopowa aplikacja do zarządzania własnym katalogiem gier komputerowych, przygotowana w technologii **WPF**, **C#** i **SQLite**.

## Opis projektu

Aplikacja pozwala prowadzić prywatną bazę gier i zapisywać dla każdej pozycji:

- tytuł,
- okładkę w formacie PNG,
- ocenę,
- krótką recenzję,
- status ukończenia.

Program umożliwia uporządkowanie listy gier, rozróżnienie tytułów planowanych, aktualnie ogrywanych, ukończonych i porzuconych, a także szybki powrót do zapisanych opinii.

Dane są przechowywane lokalnie w bazie **SQLite** w pliku `games.db`, dzięki czemu po ponownym uruchomieniu aplikacji wcześniej zapisane rekordy są nadal dostępne.

## Najważniejsze funkcje

Aplikacja obsługuje podstawowe operacje **CRUD**:

- dodawanie nowych gier,
- odczyt listy zapisanych gier,
- edycję istniejących wpisów,
- usuwanie wybranych rekordów.

Dodatkowo dostępne są:

- filtrowanie gier według statusu,
- sortowanie według nazwy,
- sortowanie według oceny,
- sortowanie według statusu,
- podgląd szczegółów wybranej gry.

## Technologie

Projekt wykorzystuje następujące technologie:

- **WPF** – warstwa graficzna aplikacji,
- **XAML** – definicja widoków i układu okien,
- **C#** – logika aplikacji,
- **SQLite** – lokalna baza danych,
- **Microsoft.Data.Sqlite** – obsługa połączenia z bazą danych.

## Struktura projektu

Najważniejsze pliki w projekcie:

- `MainWindow.xaml` / `MainWindow.xaml.cs` – główne okno aplikacji,
- `GameFormWindow.xaml` / `GameFormWindow.xaml.cs` – formularz dodawania i edycji gry,
- `GameDetailsWindow.xaml` / `GameDetailsWindow.xaml.cs` – widok szczegółów gry,
- `Game.cs` – model danych gry,
- `DbManager.cs` – obsługa bazy danych SQLite,
- `Utility.cs` – funkcje pomocnicze do konwersji obrazów,
- `ImageBytesConverter.cs` – konwerter `byte[]` na obraz wyświetlany w WPF.

## Model danych

Każda gra jest reprezentowana przez klasę `Game`, która przechowuje:

- `Id` – identyfikator rekordu,
- `Title` – tytuł gry,
- `Image` – okładkę w postaci `byte[]`,
- `Score` – opcjonalną ocenę,
- `Review` – tekst recenzji,
- `Completion` – status ukończenia zapisany jako liczba.

### Właściwości pomocnicze

- `ScoreText` – zwraca tekstową reprezentację oceny, np. `7/10` lub `No score`,
- `CompletionStatus` – zamienia wartość liczbową statusu na czytelny opis.

### Statusy gry

- `0` – `Plan to play`
- `1` – `Playing`
- `2` – `Completed`
- `3` – `Dropped`

## Baza danych

Aplikacja korzysta z lokalnej bazy danych SQLite z połączeniem:

```txt
Data Source=games.db
```

Przy uruchomieniu aplikacji metoda `Initialize()` tworzy tabelę `games`, jeśli jeszcze nie istnieje.

Tabela przechowuje:

- identyfikator,
- tytuł,
- obraz,
- ocenę,
- recenzję,
- status ukończenia.

Klasa `DbManager` odpowiada za operacje:

- `LoadGames()` – odczyt listy gier z bazy,
- `AddGame()` – dodanie nowej gry,
- `EditGame()` – edycję istniejącego wpisu,
- `DeleteGame()` – usunięcie gry.

W zapytaniach SQL użyto parametrów, co poprawia czytelność i bezpieczeństwo kodu.

## Obsługa obrazów PNG

Okładki gier są zapisywane w bazie jako **BLOB**.

### Zapis obrazu

1. Użytkownik wybiera plik PNG.
2. Plik jest odczytywany jako `FileStream`.
3. Metoda `ImageToByteArray()` zamienia zawartość pliku na `byte[]`.
4. Dane są zapisywane w kolumnie `image` tabeli `games`.

### Odczyt obrazu

1. `LoadGames()` pobiera dane binarne z kolumny `image`.
2. Bajty są przypisywane do właściwości `Image` obiektu `Game`.
3. `ByteArrayToBitmapImage()` konwertuje `byte[]` do `BitmapImage`.
4. `ImageBytesConverter` umożliwia wyświetlenie obrazu w interfejsie WPF.

## Główne okno aplikacji

Po uruchomieniu programu:

1. tworzony jest obiekt `DbManager`,
2. inicjalizowana jest baza danych,
3. wywoływana jest metoda `ShowGames()`.

Główne okno zawiera:

- nagłówek z liczbą wyświetlonych gier,
- przycisk dodawania,
- listę rozwijaną do filtrowania,
- listę rozwijaną do sortowania,
- listę kart gier.

Lista gier jest prezentowana przez `ItemsControl`, a karty są układane w `WrapPanel`.

Każda karta zawiera:

- okładkę,
- tytuł,
- ocenę,
- status,
- przyciski `View`, `Edit` i `Delete`.

Metoda `ShowGames()` odpowiada za:

- pobranie danych z bazy,
- filtrowanie według statusu,
- sortowanie według wybranego kryterium,
- aktualizację źródła danych `GamesList.ItemsSource`,
- aktualizację licznika gier,
- pokazanie komunikatu o braku rekordów, jeśli lista jest pusta.

## Dodawanie i edycja gier

Do dodawania i edycji służy to samo okno: `GameFormWindow`.

### Tryb dodawania

- pola formularza są puste,
- przycisk zapisuje nową pozycję.

### Tryb edycji

- formularz jest wypełniany danymi istniejącej gry,
- tytuł okna zmienia się na `Edit Game`,
- tekst przycisku zmienia się na `Update`.

### Formularz zawiera

- pole tytułu,
- opcjonalną ocenę w zakresie `0–10`,
- listę statusów,
- pole recenzji,
- przycisk wyboru pliku PNG.

### Walidacja danych

Metoda `SaveButton_Click`:

- usuwa zbędne spacje z tytułu i recenzji,
- sprawdza, czy tytuł nie jest pusty,
- waliduje ocenę, jeśli została wpisana,
- tworzy obiekt `GameResult`,
- zwraca `DialogResult = true` po poprawnej walidacji.

Sam formularz nie zapisuje danych do bazy. Po jego zamknięciu główne okno wywołuje:

- `dbManager.AddGame(window.GameResult)` – przy dodawaniu,
- `dbManager.EditGame(window.GameResult)` – przy edycji.

Po każdej operacji wykonywane jest ponowne `ShowGames()`, aby odświeżyć listę.

## Podgląd szczegółów gry

Okno `GameDetailsWindow` pokazuje jedną grę w bardziej czytelnym układzie.

Po otwarciu okna wybrany obiekt `Game` jest przypisywany do `DataContext`, dzięki czemu elementy XAML mogą korzystać z bindowania danych.

Widok szczegółów prezentuje:

- większą okładkę,
- tytuł gry,
- ocenę,
- status,
- pełną recenzję.

Dłuższa recenzja jest umieszczona w przewijanym obszarze, a przycisk `Close` zamyka okno.

## Przykładowy przepływ działania

1. Aplikacja uruchamia główne okno.
2. Inicjalizowana jest baza danych.
3. Gry są ładowane i wyświetlane.
4. Użytkownik może dodać nową grę przez formularz.
5. Po zapisaniu rekord trafia do bazy danych.
6. Lista gier jest odświeżana.
7. Użytkownik może także edytować, usuwać lub przeglądać szczegóły wybranej gry.

Usuwanie rekordu wymaga potwierdzenia w oknie `MessageBox`, co zabezpiecza przed przypadkowym usunięciem danych.

## Podsumowanie

Projekt przedstawia prostą aplikację desktopową do zarządzania katalogiem gier komputerowych. Łączy interfejs WPF, logikę w C# oraz lokalne przechowywanie danych w SQLite. Dzięki obsłudze CRUD, filtrowaniu, sortowaniu i wsparciu dla obrazów PNG aplikacja umożliwia wygodne zarządzanie własną biblioteką gier.
