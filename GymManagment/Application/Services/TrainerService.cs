using AutoMapper;
using GymManagment.Application.Interfaces;
using GymManagment.Application.Repositories;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;
using GymManagment.Infrastructure.Data;
using System.Collections.Generic;

namespace GymManagment.Application.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly ITrainerRepository _trainerRepository;
        private readonly IMapper _mapper;           

        public TrainerService(ITrainerRepository trainerRepository, IMapper mapper)   
        {
            _trainerRepository = trainerRepository;
            _mapper = mapper;
        }

        public List<TrainerDto> GetAllTrainers()
        {
            var allTrainers = _trainerRepository.GetAllAsync().Result;

            return _mapper.Map<List<TrainerDto>>(allTrainers);
        }

        public TrainerDto? GetTrainerById(int id)
        {
            var trainer = _trainerRepository.GetByIdAsync(id).Result;

            return _mapper.Map<TrainerDto>(trainer);
        }

        public bool CreateTrainer(TrainerDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FullName))
                return false;

            if (dto.Age < 20 || dto.Age > 70)
                return false;

            var trainer = _mapper.Map<Trainer>(dto);

       _trainerRepository.AddAsync(trainer);
            return _trainerRepository.SaveChangesAsync().Result;
        }

        public bool UpdateTrainer(int id, TrainerDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FullName))
                return false;
            if (dto.Age < 20 || dto.Age > 70)
                return false;
            var trainer =_trainerRepository.GetByIdAsync(id).Result;
            if (trainer == null)
                return false;

            _mapper.Map(dto, trainer);

            _trainerRepository.UpdateAsync(trainer).Wait();
            return _trainerRepository.SaveChangesAsync().Result;
        }

        public bool DeleteTrainer(int id)
        {
           _trainerRepository.DeleteAsync(id);
            return _trainerRepository.SaveChangesAsync().Result;
        }
    }
}