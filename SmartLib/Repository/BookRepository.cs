using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;
using SmartLib.Models.Entities;

namespace SmartLib.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BookRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<IEnumerable<BookResponseDto>> GetAllBooksAsync()
        {
            var entities = await _context.Books.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<BookResponseDto>>(entities);
            return dtos;
        }


        public async Task<BookResponseDto> GetBookByIdAsync(int id)
        {
            var entity = await _context.Books.FindAsync(id);
            if (entity == null)
            {
                throw new InvalidOperationException($"Book with ID {id} not found.");
            }

            var dto = _mapper.Map<BookResponseDto>(entity);
            return dto;
        }

        public async Task<BookResponseDto> CreateBookAsync(CreateBookDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Book data cannot be null.");
            }

            var result = await _context.Books.FirstOrDefaultAsync(b => b.Isbn == request.Isbn);

            if (result != null)
            {
                throw new InvalidOperationException($"A book with ISBN {request.Isbn} already exists.");
            }

            var entity = _mapper.Map<Book>(request);

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<BookResponseDto>(entity);
            return dto;
        }

        public async Task<BookResponseDto> UpdateBookAsync(int id, CreateBookDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Book data cannot be null.");
            }

            var entity = await _context.Books.FindAsync(id);

            if (entity == null)
            {
                throw new InvalidOperationException($"Book with ID {id} not found.");
            }

            _mapper.Map(request, entity);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<BookResponseDto>(entity);
            return dto;
        }

        public async Task DeleteBookAsync(int id)
        {
            var entity = await _context.Books.FindAsync(id);

            if (entity == null)
            {
                throw new InvalidOperationException($"Book with ID {id} not found.");
            }

            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return;
        }
        public async Task<BookResponseDto> GetBooksByNameAsync(string name)
        {
            var entities = await _context.Books.FirstOrDefaultAsync(b => b.Title == name);

            if (entities == null)
            {
                throw new InvalidOperationException($"Book with name {name} not found.");
            }

            var dto = _mapper.Map<BookResponseDto>(entities);
            return dto;
        }


    }
}
