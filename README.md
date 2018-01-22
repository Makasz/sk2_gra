### TODO
- dodać opcje startu gry
- [x] klient
- [x] server
- obsłużyć sytuacje gdy jeden gracz nie odda głosu 
(dodać timer - po upływie czasu zaznaczenie pierwszej możliwej opcji)
- [x] klient
- [x] server
- dodać funkcję wysyłania obecnego stanu gry do gracza, który dołącza do rozgrywki po jej rozpoczęciu
- [ ] klient
- [ ] server
---------------------------------------------------------------------------------

# Projekt zaliczeniowy z przedmiotu sieci komputerowe

Komputerowa, drużynowa gra online - kółko i krzyżyk


## Powstało przy użyciu

* [C#] Windows Forms - klient 
* [C++] - serwer

## Uruchomienie

Do samodzielnej kompilacji projektu potrzebne są Visual Studio oraz kompilator g++ w wersji 5 lub wyższej.

* Klient - plik .exe znajduje się w ```\sk2_gra\tictactoe\WindowsFormsApp5\bin\Debug\klient.exe```
* Serwer - należy użyć skryptu ```makefile.sh``` ```(chmod +x makefile.sh)``` , następnie uruchomić plik poleceniem ```./s``` 

## Autorzy

* Mikołaj Leśny
* Filip Bugaj
