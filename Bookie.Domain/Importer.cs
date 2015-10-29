using Bookie.Common;
using Bookie.Common.Model;
using Bookie.Domain.Interfaces;
using Bookie.Domain.Services;
using System;
using System.ComponentModel;
using System.IO;

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
            var storageFolders = _sourceService.GetAllAsStorageFolders().Result;

            foreach (var storageFolder in storageFolders)
            {
                var source = _sourceService.GetByUrl(storageFolder.Path);

                var storageFiles = storageFolder.GetFilesAsync().GetAwaiter().GetResult();
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
                        Title = storageFiles[i].DisplayName,
                        Source = source,
                        FileName = Path.GetFileName(storageFiles[i].Path),
                        FullPathAndFileName = storageFiles[i].Path
                    };

                    var existingBook = _bookService.Find(b => b.FullPathAndFileName == book.FullPathAndFileName);
                    if (existingBook.Count == 0) // Add Book
                    {
                        var cover = new Cover();
                        var coverPath = pdfCover.GenerateCoverImage(book, 0, _sourcerepo).Result;
                        cover.FileName = Path.GetFileName(coverPath);

                        book.Cover = cover;
                        book = _bookService.Add(book);

                        //if (book != null)
                        //{
                        //    sources.AddBookToSource(book);
                        //}
                        Worker.ReportProgress(progress, book);
                    }
                    else
                    {
                        Worker.ReportProgress(progress, null);
                    }
                }
            }
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