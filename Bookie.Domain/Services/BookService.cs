using Bookie.Common.Model;
using Bookie.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookie.Domain.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;

        public BookService(IBookRepository repository)
        {
            _repository = repository;
        }

        public List<Book> Find(Func<Book, bool> @where)
        {
            return _repository.Find(where).ToList();
        }

        public List<Book> GetByTitle(string title)
        {
            return _repository.Find(book => book.Title == title).ToList();
        }

        public Book GetById(int id)
        {
            return _repository.Find(book => book.Id == id).FirstOrDefault();
        }

        public async Task<List<Book>> GetAll()
        {
            return await _repository.GetAll();
       
        }

        public Book Add(Book book)
        {
            return _repository.Add(book).Result;
        }

        public void Update(Book book)
        {
            _repository.Update(book);
        }
    }
}