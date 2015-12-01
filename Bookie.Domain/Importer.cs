using Bookie.Common;
using Bookie.Common.EventArgs;
using Bookie.Common.Interfaces;
using Bookie.Common.Model;
using Bookie.Domain.Interfaces;
using Bookie.Domain.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using Windows.Storage;
using Windows.Storage.Search;
using Syncfusion.Pdf.Parsing;
using static System.String;

namespace Bookie.Domain
{
    public class Importer : IProgressPublisher
    {
        public static List<Tuple<string, long>> Times = new List<Tuple<string, long>>();

        // private CoverDal covers = new CoverDal();
        private readonly BookService _bookService;

        private readonly ProgressWindowEventArgs _progressArgs = new ProgressWindowEventArgs();
        private readonly ISourceRepository _sourcerepo;

        private readonly SourceService _sourceService;

        // private SourceDal sources = new SourceDal();
        private readonly PdfCover _pdfCover = new PdfCover();

        public readonly BackgroundWorker Worker;
        public readonly BackgroundWorker WorkerCleanup;

        public Importer(IBookRepository bookRepository, ISourceRepository sourceRepository)
        {
            _sourcerepo = sourceRepository;
            _bookService = new BookService(bookRepository);
            _sourceService = new SourceService(sourceRepository);
            ProgressService.RegisterPublisher(this);
            Worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            Worker.DoWork += Worker_DoWork;
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            Worker.ProgressChanged += Worker_ProgressChanged;

            WorkerCleanup = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            WorkerCleanup.DoWork += WorkerCleanup_DoWork;
            WorkerCleanup.RunWorkerCompleted += WorkerCleanup_RunWorkerCompleted;
            WorkerCleanup.ProgressChanged += WorkerCleanup_ProgressChanged;
        }

        private void WorkerCleanup_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var book = (Book)e.UserState;
            if (book != null)
            {
                _progressArgs.OperationName = "Cleaning Up";
                _progressArgs.ProgressPercentage = Convert.ToInt32(e.ProgressPercentage);
                _progressArgs.ProgressText = "Removed book";
                OnProgressChange(_progressArgs);
            }
            else
            {
                _progressArgs.OperationName = "Cleaning Up";
                _progressArgs.ProgressPercentage = Convert.ToInt32(e.ProgressPercentage);
                _progressArgs.ProgressText = "No Change";
                OnProgressChange(_progressArgs);
            }
        }

        private void WorkerCleanup_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnProgressComplete();
        }

        private void WorkerCleanup_DoWork(object sender, DoWorkEventArgs e)
        {
            var books = _bookService.GetAllAsync().GetAwaiter().GetResult();

            //foreach book check if it physicall exist else delete from db.
            for (int index = 0; index < books.Count; index++)
            {
                var book = books[index];
                var progress = Utils.CalculatePercentage(index, 0, books.Count);
                var storageFolder = StorageFolder.GetFolderFromPathAsync(book.Source.Path).GetAwaiter().GetResult();
                var exists = storageFolder.TryGetItemAsync(book.FileName).GetAwaiter().GetResult(); // null not found
                if (exists == null)
                {
                    _bookService.Remove(book);
                    WorkerCleanup.ReportProgress(progress, book);
                }
                else
                {
                    WorkerCleanup.ReportProgress(progress, null);
                }
            }
        }

        public event EventHandler<ProgressWindowEventArgs> ProgressChanged;

        public event EventHandler<EventArgs> ProgressComplete;

        public event EventHandler<EventArgs> ProgressStarted;

        public void ProgressCancel()
        {
            if (Worker.IsBusy)
            {
                Worker.CancelAsync();
            }
        }

        public event EventHandler<BookEventArgs> BookChanged;

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                var book = (Book)e.UserState;
                _progressArgs.OperationName = "Importing Books";
                _progressArgs.ProgressPercentage = Convert.ToInt32(e.ProgressPercentage);
                _progressArgs.ProgressText = book.Title;
                _progressArgs.OperationSubText = "XML or Scarped";
                OnProgressChange(_progressArgs);
            }
            catch (Exception)
            {
                var b = (Tuple<Book, string>)e.UserState;

                _progressArgs.OperationName = "Importing Books";
                _progressArgs.ProgressPercentage = Convert.ToInt32(e.ProgressPercentage);
                _progressArgs.ProgressText = "Exists " + b.Item1.Title;
                _progressArgs.OperationSubText = "XML or Scarped";

                OnProgressChange(_progressArgs);
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnProgressComplete();
            Cleanup();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var filter = new List<string> { ".pdf" };

            var storageFolders = _sourceService.GetAllAsStorageFoldersAsync().Result;

            foreach (var storageFolder in storageFolders)
            {
                var source = _sourceService.GetByUrl(storageFolder.Path);
                var options = new QueryOptions(CommonFileQuery.OrderByName, filter);
                var ss = storageFolder.CreateItemQueryWithOptions(options);

                //Get all PDF files for storagefolder
                var pdfFiles = ss.GetItemsAsync().GetAwaiter().GetResult();
                for (var i = 0; i < pdfFiles.Count; i++)
                {
                    var progress = Utils.CalculatePercentage(i, 0, pdfFiles.Count);
                    if (Worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    var book = new Book
                    {
                        Title = Path.GetFileNameWithoutExtension(pdfFiles[i].Name),
                        Source = source,
                        FileName = Path.GetFileName(pdfFiles[i].Path),
                        FullPathAndFileName = pdfFiles[i].Path,
                        Rating = 0
                    };

                    var book1 = book;
                    var existingBook = _bookService.Exists(book1);

                    if (existingBook == false) // Add Book
                    {
                        var xml = storageFolder.TryGetItemAsync(book.Title + ".xml").GetAwaiter().GetResult();
                        if (xml != null)
                        {
                            book = XmlToBook(xml as StorageFile, book);
                        }
                        else
                        {
                            book = UseIsbn(pdfFiles[i] as StorageFile, book);
                        }



                        var cover = new Cover();
                        var coverPath = _pdfCover.GenerateCoverImage(book, 0, _sourcerepo, storageFolder, pdfFiles[i] as StorageFile).Result;
                        cover.FileName = Path.GetFileName(coverPath);

                        
                        book.Cover = cover;
                        book = _bookService.Add(book);
                        Worker.ReportProgress(progress, book);
                    }
                    else
                    {
                        Tuple<Book, string> exists = new Tuple<Book, string>(book, "Exists");
                        Worker.ReportProgress(progress, exists);
                    }
                }
            }
        }

        private Book UseIsbn(StorageFile storageFile, Book book)
        {
            var outBook = new Book
            {
                Title = Path.GetFileNameWithoutExtension(storageFile.Name),
                Source = book.Source,
                FileName = book.FileName,
                FullPathAndFileName = book.FullPathAndFileName
            };
            var parser = new PdfParser();
            var s = parser.Extract(storageFile, 1, 10).Result;
            var isbn = PdfIsbnParser.FindIsbn(s);
            if (IsNullOrEmpty(isbn)) return outBook;
            outBook.Isbn = isbn;
            var scraper = new Scraper.Scraper();
            var results = scraper.Scrape(outBook.Isbn).Result;
            if (results.items == null || results.items.Count <= 0) return outBook;
            outBook.Title = results.items[0].volumeInfo.title;
            outBook.Abstract = results.items[0].volumeInfo.description;
            outBook.Pages = results.items[0].volumeInfo.pageCount;
            outBook.Scraped = true;
            if (results.items[0].volumeInfo.authors != null)
            {
                outBook.Author = Join(",", results.items[0].volumeInfo.authors.ToArray());
            }
            outBook.Publisher = results.items[0].volumeInfo.publisher;
            return outBook;
        }

        private Book XmlToBook(StorageFile xml, Book book)
        {
            Stopwatch stop = new Stopwatch();
            stop.Start();
            var file = xml;

            var stream = file.OpenStreamForReadAsync().GetAwaiter().GetResult();

            var outBook = new Book();
            using (var reader = XmlReader.Create(stream))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "Title":
                                outBook.Title = reader.ReadElementContentAsString();
                                break;

                            case "Description":
                                outBook.Abstract = reader.ReadElementContentAsString();
                                break;

                            case "Year":
                                DateTime datePublished;
                                var successDate = DateTime.TryParseExact(reader.ReadElementContentAsString(), "yyyy",
                                    CultureInfo.InvariantCulture, DateTimeStyles.None, out datePublished);
                                if (successDate)
                                {
                                    outBook.DatePublished = datePublished;
                                }
                                break;

                            case "Pages":
                                int pages;
                                var successPages = int.TryParse(reader.ReadElementContentAsString(), out pages);
                                if (successPages)
                                {
                                    outBook.Pages = pages;
                                }
                                break;

                            case "Isbn":
                                outBook.Isbn = reader.ReadElementContentAsString();
                                break;

                            case "Publisher":
                                var publisherReader = reader.ReadSubtree();
                                publisherReader.Read();
                                while (publisherReader.Read())
                                {
                                    if (reader.Name == "Name")
                                    {
                                        outBook.Publisher += reader.ReadElementContentAsString();
                                    }
                                }
                                break;

                            case "Author":
                                var authorReader = reader.ReadSubtree();
                                authorReader.Read();
                                while (authorReader.Read())
                                {
                                    if (reader.Name == "Name")
                                    {
                                        outBook.Author += reader.ReadElementContentAsString() + ",";
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            outBook.Source = book.Source;
            outBook.FileName = book.FileName;
            outBook.FullPathAndFileName = book.FullPathAndFileName;
            outBook.Scraped = true;
            stream.Dispose();
            stop.Stop();
            var t = stop.ElapsedMilliseconds;
            return outBook;
        }

        public void OnBookChanged(Book book, BookEventArgs.BookState bookState, int? progress)
        {
            BookChanged?.Invoke(this, new BookEventArgs { Book = book, State = bookState, Progress = progress });
        }

        public void UpdateBooksFromSources()
        {
            OnProgressStarted();

            Worker.RunWorkerAsync();
        }

        private void Cleanup()
        {
            OnProgressStarted();
            WorkerCleanup.RunWorkerAsync();
        }

        private void OnProgressComplete()
        {
            ProgressComplete?.Invoke(this, null);
        }

        private void OnProgressStarted()
        {
            ProgressStarted?.Invoke(this, null);
        }

        private void OnProgressChange(ProgressWindowEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }
}