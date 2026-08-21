using AutoMapper;
using GymManagment.Application.Interfaces;
using GymManagment.Application.Repositories;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Common;
using GymManagment.Domain.Models;
using Microsoft.Extensions.Caching.Memory;


namespace GymManagment.Application.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly ITrainerRepository _trainerRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private static int _cacheVersion = 1;

        public TrainerService(ITrainerRepository trainerRepository, IMapper mapper, IMemoryCache memoryCache)   
        {
            _trainerRepository = trainerRepository;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<PagedResult<TrainerDto>> GetAllTrainersAsync(int page)
        {
            string cacheKey = $"trainers_v{_cacheVersion}_page_{page}";
            

            var result = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var trainers = await _trainerRepository.GetAllAsync(page);
                return _mapper.Map<PagedResult<TrainerDto>>(trainers);
            });

            return result;
        
        }

        public async Task<TrainerDto?> GetTrainerByIdAsync(int id)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id);

            return  _mapper.Map<TrainerDto>(trainer);
        }

        public async Task<Result> CreateTrainerAsync(TrainerDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FullName))
                return Result.Fail("Переданы пустые значения", Result.ErrorTypes.ValidationError);

            if (dto.Age < 20 || dto.Age > 70)
                return Result.Fail("Возраст тренера указан неверно", Result.ErrorTypes.ValidationError);
            

            var trainer = _mapper.Map<Trainer>(dto);

               await _trainerRepository.AddAsync(trainer);
            var succes = await _trainerRepository.SaveChangesAsync();
            if (succes == false)
            {
                return Result.Fail("Не удалось создать тренера", Result.ErrorTypes.ServerError);
            }
            _cacheVersion++;
            return Result.Ok();
        }

        public async Task<Result> UpdateTrainerAsync(int id, TrainerDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.FullName))
                return Result.Fail("Переданы пустые значения", Result.ErrorTypes.ValidationError);
            if (dto.Age < 20 || dto.Age > 70)
                return Result.Fail("Возраст тренера указан неверно", Result.ErrorTypes.ValidationError);
            var trainer = await _trainerRepository.GetByIdTrackedAsync(id);
            if (trainer == null)
                return Result.Fail($"Тренер с ID:{id} не найден", Result.ErrorTypes.NotFound);

            _mapper.Map(dto, trainer);


            var succes = await _trainerRepository.SaveChangesAsync();
            if (succes == false)
            {
                return Result.Fail("Не удалось обновить  тренера", Result.ErrorTypes.ServerError);
            }
            _cacheVersion++;
            return Result.Ok();
        }

        public async Task<Result> DeleteTrainerAsync(int id)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id);
            if (trainer == null)
            {
                return Result.Fail($"Тренер с ID:{id} не найден", Result.ErrorTypes.NotFound);
            }
            var hasActiveMembers= await _trainerRepository.HasActiveMembersAsync(id);
            if (hasActiveMembers == true) { 
          
                return Result.Fail("Невозможно удалить тренера с активными клиентами", Result.ErrorTypes.Conflict); }
           await _trainerRepository.DeleteAsync(id);
            var succes=  await _trainerRepository.SaveChangesAsync();
            if (succes == false) {
                return Result.Fail("Не удалось удалить тренера", Result.ErrorTypes.ServerError);
            }
            _cacheVersion++;
            return Result.Ok();
        }
    }
}