using AutoMapper;
using GymManagment.Domain.Models;
using GymManagment.Domain.DTO;

namespace GymManagment.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Member <-> MemberDto
            CreateMap<Member, MemberDto>();
            CreateMap<MemberDto, Member>();

            // Trainer <-> TrainerDto
            CreateMap<Trainer, TrainerDto>();
            CreateMap<TrainerDto, Trainer>();
        }
    }
}