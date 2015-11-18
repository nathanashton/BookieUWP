using Bookie.Common;
using Bookie.Common.Model;
using Bookie.Domain.Interfaces;
using Bookie.Domain.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml;
using Windows.Storage;
using static System.String;

namespace Bookie.Domain
{
    public class Importer : IProgressPublisher
    {
        public event EventHandler<BookEventArgs> BookChanged;

        public readonly BackgroundWorker Worker;

        // private SourceDal sources = new SourceDal();
        private readonly PdfCover pdfCover = new PdfCover();

        // private CoverDal covers = new CoverDal();
        private readonly BookService _bookService;

        private readonly SourceService _sourceService;
        private readonly ISourceRepository _sourcerepo;

        private readonly ProgressWindowEventArgs _progressArgs = new ProgressWindowEventArgs();

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
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var book = (Book)e.UserState;
            if (book != null)
            {
                _progressArgs.OperationName = "Importing Books";
                _progressArgs.ProgressPercentage = Convert.ToInt32(e.ProgressPercentage);
                _progressArgs.ProgressText = book.Title;
                OnProgressChange(_progressArgs);
            }
            else
            {
                _progressArgs.OperationName = "Importing Books";
                _progressArgs.ProgressPercentage = Convert.ToInt32(e.ProgressPercentage);
                _progressArgs.ProgressText = "Book exists";
                OnProgressChange(_progressArgs);
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnProgressComplete();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> filter = new List<string>();
            filter.Add(".pdf");

            var storageFolders = _sourceService.GetAllAsStorageFolders().Result;

            foreach (var storageFolder in storageFolders)
            {
                var source = _sourceService.GetByUrl(storageFolder.Path);
                var options = new Windows.Storage.Search.QueryOptions(Windows.Storage.Search.CommonFileQuery.OrderByName, filter);
                var ss = storageFolder.CreateItemQueryWithOptions(options);
                var storageFiles = ss.GetItemsAsync().GetAwaiter().GetResult();
                for (var i = 0; i < storageFiles.Count; i++)
                {
                    var progress = Utils.CalculatePercentage(i, 0, storageFiles.Count);
                    if (Worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    var book = new Book
                    {
                        Title = Path.GetFileNameWithoutExtension(storageFiles[i].Name),
                        Source = source,
                        FileName = Path.GetFileName(storageFiles[i].Path),
                        FullPathAndFileName = storageFiles[i].Path,
                        Rating = 0
                    };

                    var existingBook = _bookService.Find(b => b.FullPathAndFileName == book.FullPathAndFileName);
                    if (existingBook.Count == 0) // Add Book
                    {
                        //If there is an XML for pdf then uses its data, otherwise find ISBN and scrape
                        var xml =
                        _sourceService.GetStorageFolderFromSource(source)
                            .GetAwaiter()
                            .GetResult()
                            .TryGetItemAsync(book.Title + ".xml")
                            .GetAwaiter()
                            .GetResult();
                        if (xml != null)
                        {
                            book = XmlToBook(xml as StorageFile, book);
                        }
                        else
                        {
                            book = UseIsbn(storageFiles[i] as StorageFile, book);
                        }

                        var cover = new Cover();
                        var coverPath = pdfCover.GenerateCoverImage(book, 0, _sourcerepo).Result;
                        cover.FileName = Path.GetFileName(coverPath);

                        book.Cover = cover;
                        book = _bookService.Add(book);
                        Worker.ReportProgress(progress, book);
                    }
                    else
                    {
                        Worker.ReportProgress(progress, null);
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
            var file = StorageFile.GetFileFromPathAsync(xml.Path).GetAwaiter().GetResult();
            var stream = file.OpenStreamForReadAsync().GetAwaiter().GetResult();

            var outBook = new Book();
            using (var reader = XmlReader.Create(stream))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        var s = reader.Name;
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
                                var successDate = DateTime.TryParseExact(reader.ReadElementContentAsString(), "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out datePublished);
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