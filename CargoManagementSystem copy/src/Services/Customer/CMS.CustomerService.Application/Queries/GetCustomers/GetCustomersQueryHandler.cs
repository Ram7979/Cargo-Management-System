using AutoMapper;
using CMS.CustomerService.Application.DTOs;
using CMS.CustomerService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Queries.GetCustomers;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResponse<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public GetCustomersQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _customerRepository.GetPagedAsync(
            request.Page, request.PageSize, request.Search, request.Type, request.IsActive);

        var dtos = _mapper.Map<IEnumerable<CustomerDto>>(items);
        return PagedResponse<CustomerDto>.Ok(dtos, request.Page, request.PageSize, totalCount);
    }
}
