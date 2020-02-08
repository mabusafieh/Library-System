# Library System

Libray System is a .Net core console application that is responsible for make auditing on a log of transactions file.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

Run Visual Studio as Administration.

## Running the tests

When the app is running, do the following:

1. enter full file path. example:

```
Enter input file path:C:/2017-01.xml
```

2. type 'J', if you need the output file as JSON file. click enter if you need it as text file:

```
If you need the output file in JSON type 'J', otherwise the output file will be exported as a text file (Optional):
```

3. the output file will be placed in the current directory.

Sample JSON file:
```
{ person_with_most_checkouts = 2, most_checked_out_book = 99-9263-544-4, current_checked_out_book_count = 1, person_who_has_currently_most_books = 3 }
```

Sample text file:
```
Person with most checkouts = 2
Most checked out book = 99-9263-544-4
Current checked out book count = 1
Person who has currently most books = 3
```

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc

