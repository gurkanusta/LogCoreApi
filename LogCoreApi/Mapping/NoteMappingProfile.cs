using AutoMapper;
using LogCoreApi.DTOs.Notes;
using LogCoreApi.Entities;

namespace LogCoreApi.Mapping
{
    public class NoteMappingProfile : Profile
    {
        public NoteMappingProfile()
        {
            CreateMap<NoteCreateDto, Note>();
            CreateMap<NoteUpdateDto, Note>();
            CreateMap<Note, NoteResponseDto>();
        }
    }
}