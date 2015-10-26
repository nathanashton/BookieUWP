using Bookie.Common;
using Bookie.Common.Model;
using System;
using System.ComponentModel;
using System.IO;
using Bookie.Data;
using Bookie.Repository;

namespace Bookie.Core
{
    public class Importer : IProgressPublisher
    {
        public event EventHandler<BookEventArgs> BookChanged;

        public readonly BackgroundWorker Worker;
        private SourceDal sources = new SourceDal();
        private PdfCover pdfCover = new PdfCover();
        private CoverDal covers = new CoverDal();
        private BookRepository books = new BookRepository();
        private ProgressWindowEventArgs ProgressArgs = new ProgressWindowEventArgs();

        public Importer()
        {
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
                ProgressArgs.OperationName = "Importing Books";
                ProgressArgs.ProgressPercentage = Convert.ToInt32(e.ProgressPercentage);
                ProgressArgs.ProgressText = book.Title;
                OnProgressChange(ProgressArgs);
            }
            else
            {
                ProgressArgs.OperationName = "Importing Books";
                ProgressArgs.ProgressPercentage = Convert.ToInt32(e.ProgressPercentage);
                ProgressArgs.ProgressText = "Book exists";
                OnProgressChange(ProgressArgs);
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnProgressComplete();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var storageFolders = sources.GetAllAsStorageFolders().Result;

            foreach (var storageFolder in storageFolders)
            {
                var source = sources.GetSourceFromStorageFolderPath(storageFolder.Path);
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

                    // var exists = books.Exists(book);
                    var exists = false;
                    if (!exists)
                    {
                        var cover = new Cover();
                        var coverPath = pdfCover.GenerateCoverImage(book, 0).Result;
                        cover.FileName = Path.GetFileName(coverPath);

                        book.Cover = cover;
                        book = books.Add(book);

                        if (book != null)
                        {
                            sources.AddBookToSource(book);
                        }
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