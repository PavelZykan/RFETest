using AutoMapper;
using RFETest.Core.Diff;
using RFETest.WebContracts;

namespace RFETest.WebApi.Mapping
{
    public class WebProfile : Profile
    {
        public WebProfile()
        {
            CreateMap<Core.Diff.DiffLocation, Difference>();
            CreateMap<Core.Diff.EDiffResultType, WebContracts.EDiffResultType>();

            // TODO: if this grows too much, separate the mapping into a dedicated file
            CreateMap<DiffResult, DiffOutput>()
                .ForMember(x => x.Message, 
                    x => x.MapFrom(result => result.Result == Core.Diff.EDiffResultType.Match
                                ? "inputs were equal"
                                : result.Result == Core.Diff.EDiffResultType.ContentMismatch
                                ? "inputs have different content"
                                : "inputs are of different size"));
        }
    }
}
