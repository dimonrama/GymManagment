using AutoMapper;
using GymManagment.Application.Interfaces;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;
using GymManagment.Infrastructure.Data;
using System.Collections.Generic;

namespace GymManagment.Application.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly GymDbContext _context;
        private readonly IMapper _mapper;           

        public TrainerService(GymDbContext context, IMapper mapper)   
        {
            _context = context;
            _mapper = mapper;
        }

        public List<TrainerDto> GetAllTrainers()
        {
            var allTrainers = _context.Trainers.ToList();

            return _mapper.Map<List<TrainerDto>>(allTrainers);
        }

        public TrainerDto? GetTrainerById(int id)
        {
            var trainer = _context.Trainers.Find(id);
            if (trainer == null)
                return null;

            return _mapper.Map<TrainerDto>(trainer);
        }

        public bool CreateTrainer(TrainerDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FullName))
                return false;

            if (dto.Age < 20 || dto.Age > 70)
                return false;

            var trainer = _mapper.Map<Trainer>(dto);

            _context.Trainers.Add(trainer);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateTrainer(int id, TrainerDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FullName))
                return false;

            var trainer = _context.Trainers.Find(id);
            if (trainer == null)
                return false;

            if (dto.Age < 20 || dto.Age > 70)
                return false;

            _mapper.Map(dto, trainer);

            _context.SaveChanges();
            return true;
        }

        public bool DeleteTrainer(int id)
        {
            var trainer = _context.Trainers.Find(id);
            if (trainer == null)
                return false;

            _context.Trainers.Remove(trainer);
            _context.SaveChanges();
            return true;
        }
    }
}