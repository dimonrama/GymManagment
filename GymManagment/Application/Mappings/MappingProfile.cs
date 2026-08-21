using AutoMapper;
using GymManagment.Domain.Common;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;

namespace GymManagment.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MemberDto, Member>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Member, MemberDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));


            CreateMap<Trainer, TrainerDto>();
            CreateMap<TrainerDto, Trainer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

          
        }
    }
}