﻿using Bookie.Common;
using Bookie.Common.Model;
using Bookie.Domain.Interfaces;
using Bookie.Domain.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Windows.Storage;
using static System.String;

namespace Bookie.Domain
{
    public class Importer : IProgressPublisher
    {
        public event EventHandler<BookEventArgs> BookChanged;

        public readonly BackgroundWorker Worker;

        // private SourceDal sources = new SourceDal();
        private PdfCover pdfCover = new PdfCover();

        // private CoverDal covers = new CoverDal();
        private BookService _bookService;

        private SourceService _sourceService;
        private ISourceRepository _sourcerepo;

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
                            //Scrape for Isbn
                            PdfParser p = new PdfParser();
                            var s = p.Extract((storageFiles[i] as StorageFile), 1, 10).Result;
                            var isbn = PdfIsbnParser.FindIsbn(s);
                            if (!IsNullOrEmpty(isbn))
                            {
                                book.Isbn = isbn;
                                var scraper = new Scraper.Scraper();
                                var results = scraper.Scrape(book.Isbn).Result;
                                if (results.items != null && results.items.Count > 0)
                                {
                                    book.Title = results.items[0].volumeInfo.title;
                                    book.Abstract = results.items[0].volumeInfo.description;
                                    book.Pages = results.items[0].volumeInfo.pageCount;
                                    book.Scraped = true;

                                    if (results.items[0].volumeInfo.authors != null)
                                    {
                                        book.Author = Join(",", results.items[0].volumeInfo.authors.ToArray());
                                    }
                                    book.Publisher = results.items[0].volumeInfo.publisher;
                                }
                            }
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

        private Book XmlToBook(StorageFile xml, Book book)
        {
            var file = StorageFile.GetFileFromPathAsync(xml.Path).GetAwaiter().GetResult();
            var stream = file.OpenStreamForReadAsync().GetAwaiter().GetResult();
            var serializer = new XmlSerializer(typeof(Book));
            Book outBook;
            using (var reader = new StreamReader(stream))
            {
                outBook = (Book)serializer.Deserialize(reader);
            }
            outBook.Source = book.Source;
            outBook.FileName = book.FileName;
            outBook.FullPathAndFileName = book.FullPathAndFileName;
            outBook.Scraped = true;
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