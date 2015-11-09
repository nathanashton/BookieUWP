using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using static System.String;

namespace Bookie.Common.Model
{
    public class Book
    {
        public int Id { get; set; }

        [XmlElement("Title")]
        public string Title { get; set; }

        [XmlElement("Description")]
        public string Abstract { get; set; }

        [XmlElement("Year")]
        public DateTime? DatePublished { get; set; }

        [XmlElement("Pages")]
        public int? Pages { get; set; }

        public int Rating { get; set; }
        public string Isbn { get; set; }
        public bool Favourite { get; set; }
        public int? CurrentPage { get; set; }
        public bool Scraped { get; set; }
        public string Publisher { get; set; }
        public string Author { get; set; }

        public string FullPathAndFileName { get; set; }
        public string FileName { get; set; }

        public virtual List<BookMark> BookMarks { get; set; }
        public virtual Cover Cover { get; set; }
        public virtual Source Source { get; set; }
        public int SourceId { get; set; }
        public bool Shelf { get; set; }

        [NotMapped]
        public string DatePublishedString
        {
            get
            {
                var s = DatePublished;
                return s?.ToString("yyyy");
            }
        }

        [NotMapped]
        public ImageSource Image => !IsNullOrEmpty(Cover?.FileName) ? new BitmapImage(new Uri("ms-appdata:///local/Covers/" + Cover.FileName)) : new BitmapImage(new Uri("ms-appx:///Assets/nocover.png"));

        public override string ToString()
        {
            return $"Id: {Id}, Title: {Title}, Abstract: {Abstract}, DatePublished: {DatePublished}, Pages: {Pages}, FullPathAndFileName: {FullPathAndFileName}, FileName: {FileName}, BookMarks: {BookMarks}, Cover: {Cover}, Source: {Source}";
        }
    }
}