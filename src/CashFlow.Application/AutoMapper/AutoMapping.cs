using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;

namespace CashFlow.Application.AutoMapper;

public class AutoMapping : Profile
{

    public AutoMapping()
    {
        RequestToEntity();
        EntityToResponse();

    }
    private void RequestToEntity()
    {
        CreateMap<RequestRegisterUserJson, User>()
            .ForMember(dest => dest.Password, config => config.Ignore());

        CreateMap<RequestExpenseJson, Expense>()
            .ForMember(dest => dest.Tags,
                config => 
                    config.MapFrom(source => source.Tags.Distinct())); //dessa forma, o distinct remove valores duplicados 
        
        CreateMap<CashFlow.Communication.Enums.Tag, Tag>()
            .ForMember(dest => dest.ValueTag, 
                config => config.MapFrom(source => source));
    }
    private void EntityToResponse()
    {
        CreateMap<Expense, ResponseExpenseByIdJson>()
            .ForMember(dest => dest.Tags, config =>
                config.MapFrom(source => source.Tags.Select(tag => tag.ValueTag)));
        
        CreateMap<Expense, ResponseRegisterExpense>();
        CreateMap<Expense, ResponseShortExpenseJson>();
        CreateMap<User, ResponseUserProfileJson>();
    }
}
