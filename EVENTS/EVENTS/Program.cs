using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVENTS
{
    class Program
    {
        class Book
        {
            public string bookname { get; set; }
            public string author { get; set; }
            public Book(string bookname, string author)
            {
                this.bookname = bookname;
                this.author = author;
                CheckNewLine();
            }
            private void CheckNewLine()
            {
                bool flag = false;
                for (int i = 0; i < bookname.Length; i++)
                {
                    if (bookname[i] == '\n')
                        flag = true;
                }
                if (!flag)
                    bookname += '\n';
            }
        }
        class User
        {
            public delegate void UserHandler();
            public event UserHandler UserAddedHandlerEventArgs;
            public event UserHandler UserSavedHandlerEventArgs;
            public event UserHandler UserLoadedHandlerEventArgs;

            public delegate void ListBookHandler(Book book);
            public event ListBookHandler ListBookHandlerEventArgs;

            public delegate void BookHandler();
            public event BookHandler BookHandlerEventArgs;
            public event BookHandler BookHandlerEventArgsI;


            public void AddBook(string bookname, string author, List<Book> books)
            {
                books.Add(new Book(bookname, author));
                UserAddedHandlerEventArgs();
            }
            public void WriteAllBooks(List<Book> books)
            {
                foreach (var elem in books)
                    ListBookHandlerEventArgs(elem);
            }
            public void SaveBooks(List<Book> books, string path)
            {
                if (CheckBooks(books))
                {
                    string tempsaving = null;
                    path += "\\books.txt";
                    File.Create(path).Dispose();

                    foreach (var elem in books)
                    {
                        tempsaving += $"{elem.author}:{elem.bookname}";
                    }

                    File.WriteAllText(path, tempsaving);
                    UserSavedHandlerEventArgs();
                } else
                    BookHandlerEventArgsI();
            }
            public void LoadBooks(List<Book> books, string path)
            {
                string tempAuthor = null;
                string tempBookName = null;

                try
                {
                    string tempBooks = File.ReadAllText(path);
                    bool nameWrited = false;

                    for (int i = 0; i < tempBooks.Length; i++)
                    {
                        if (tempBooks[i] != ':' && nameWrited.Equals(false))
                            tempAuthor += tempBooks[i];

                        if (tempBooks[i] == ':')
                            nameWrited = true;

                        if (nameWrited && tempBooks[i] != ':')
                            tempBookName += tempBooks[i];

                        if (tempBooks[i] == '\n' || i == tempBooks.Length - 1)
                        {
                            books.Add(new Book(tempBookName, tempAuthor));
                            tempAuthor = null;
                            tempBookName = null;
                            nameWrited = false;
                        }
                    }
                    UserLoadedHandlerEventArgs();
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Введите путь корректно");
                }
            }
            public void RemoveBook(string author, string bookname, List<Book> books)
            {
                bool isDeleted = false;
                bookname += '\n';
                if (CheckBooks(books))
                {
                    foreach (var elem in books)
                    {
                        if (elem.author.Equals(author) && elem.bookname.Equals(bookname))
                        {
                            books.Remove(elem);
                            isDeleted = true;
                            break;
                        }
                    }
                    if (isDeleted)
                        BookHandlerEventArgs();
                    else
                        BookHandlerEventArgsI();
                }
                else
                    BookHandlerEventArgsI();
            }
            private bool CheckBooks(List<Book> books)
            {
                return books.Count != 0 ? true : false;
            }
        }
        class Commands
        {
            public void BookAdded()
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nКнига была добавлена");
            }
            public void DeleteBook()
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nКнига была удалена");
            }
            public void WriteBook(Book book)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Имя автора: {book.author}. Название книги: {book.bookname}");
            }
            public void Error()
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Книг не найдено");
            }
            public void Save()
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Все данные сохранены");
            }
            public void Load()
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Все данные загружены");
            }
            public void Notify()
            {
                Console.WriteLine("Хотите вернутся назад? Нажмите любую клавишу\n");
                Console.ReadKey();
            }
        }
        static void Main(string[] args)
        {
            List<Book> books = new List<Book>();
            User user = new User();
            Commands commands = new Commands();
            bool leaving = false;

            user.ListBookHandlerEventArgs += commands.WriteBook;
            user.UserAddedHandlerEventArgs += commands.BookAdded;
            user.BookHandlerEventArgs += commands.DeleteBook;
            user.BookHandlerEventArgsI += commands.Error;
            user.UserSavedHandlerEventArgs += commands.Save;
            user.UserLoadedHandlerEventArgs += commands.Load;


            while (true)
            {
                if (leaving)
                    break;

                int selected = 0;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Выберите что хотите сделать:\n\t1.Добавить книгу" +
                       "\n\t2.Удалить книгу\n\t3.Показать все книги\n\t4.Сохранить все книги\n\t" +
                       "5.Загрузить книги из файла\n\t6.Отчистить консоль\n\t7.Выйти из приложения");

                try
                {
                    selected = int.Parse(Console.ReadLine());
                    switch (selected)
                    {
                        case 1:
                            Console.Write("Введите имя книги: ");
                            string bookname = Console.ReadLine();
                            Console.Write("Введите автора книги: ");
                            string author = Console.ReadLine();

                            user.AddBook(bookname, author, books);
                            commands.Notify();
                            break;
                        case 2:
                            Console.Write("Введите имя удаляемой книги: ");
                            bookname = Console.ReadLine();
                            Console.Write("Введите автора удаляемой книги: ");
                            author = Console.ReadLine();

                            user.RemoveBook(author, bookname, books);
                            commands.Notify();
                            break;
                        case 3:
                            if (books.Count == 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\nКниг пока еще нету");
                                commands.Notify();
                            }
                            else
                            {
                                user.WriteAllBooks(books);
                                commands.Notify();
                            }
                            break;
                        case 4:
                            Console.Write("Введите путь куда хотите сохранить книги: ");
                            user.SaveBooks(books, Console.ReadLine());
                            break;
                        case 5:
                            Console.Write("Введите путь txt файла books: ");
                            user.LoadBooks(books, Console.ReadLine());
                            break;
                        case 6: Console.Clear(); break;
                        case 7:
                            leaving = true;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Такого варианта не существует");
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine("Неверно введена команда..");
                }
            }

            Console.WriteLine("Нажмите любую клавишу чтобы продолжить..");
            Console.ReadKey();
        }
    }
}
