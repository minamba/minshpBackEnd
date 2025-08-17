using AutoMapper;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class TaxeService : ITaxeService
    {

        private IMapper _mapper;
        private ITaxeRepository _repository;

        public TaxeService(ITaxeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<Taxe> AddTaxeAsync(Taxe model)
        {
           return await _repository.AddTaxeAsync(model);
        }

        public async Task<bool> DeleteTaxeAsync(int idTaxe)
        {
           return await _repository.DeleteTaxeAsync(idTaxe);
        }

        public async Task<IEnumerable<Taxe>> GetTaxesAsync()
        {
           return await _repository.GetTaxesAsync();
        }

        public async Task<Taxe> UpdateTaxeAsync(Taxe model)
        {
            return await _repository.UpdateTaxeAsync(model);
        }
    }
}
