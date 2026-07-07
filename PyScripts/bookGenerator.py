import json
import random


def read_lines(filename):
    with open(filename, "r", encoding="utf-8") as f:
        return [line.strip() for line in f if line.strip()]


def main():
    first_names = read_lines("FirstNames.txt")
    last_names = read_lines("LastNames.txt")
    genres = read_lines("Genre.txt")
    names = read_lines("Names.txt")
    adjs = read_lines("Adjs.txt")

    books = []
    for book_id in range(1, 101):
        title = f"{random.choice(adjs).capitalize()} {random.choice(names).capitalize()}"
        author = f"{random.choice(first_names)} {random.choice(last_names)}"

        book = {
            "Title": title,
            "Author": author,
            "CopiesAvailable": random.randint(0, 10),
            "Genre": random.choice(genres),
            "PublicationYear": random.randint(1500, 2026),
            "LostChargePrice": random.randint(10000, 1000000),
            "Id": book_id,
        }
        books.append(book)

    with open("books.json", "w", encoding="utf-8") as f:
        json.dump(books, f, indent=2, ensure_ascii=False)

    print(f"Created books.json with {len(books)} books.")


if __name__ == "__main__":
    main()