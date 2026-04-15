using AutoMapper;
using CMS.CustomerService.Application.DTOs;
using CMS.CustomerService.Application.Interfaces;
using CMS.CustomerService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Queries.GetCustomer;

public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, ApiResponse<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public GetCustomerQueryHandler(
        ICustomerRepository customerRepository,
        ICacheService cacheService,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CustomerDto>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"customer:{request.CustomerId}";
        var cached = await _cacheService.GetAsync<CustomerDto>(cacheKey);
        if (cached is not null)
            return ApiResponse<CustomerDto>.Ok(cached);

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId)
            ?? throw new NotFoundException("Customer", request.CustomerId);

        var dto = _mapper.Map<CustomerDto>(customer);
        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));

        return ApiResponse<CustomerDto>.Ok(dto);
    }
}
