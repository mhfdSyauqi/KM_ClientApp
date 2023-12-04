﻿using FluentValidation;
using KM_ClientApp.Commons.Entity;
using KM_ClientApp.Commons.Mediator;
using KM_ClientApp.Commons.Shared;
using KM_ClientApp.Models.Request;
using KM_ClientApp.Models.Response;

namespace KM_ClientApp.Endpoint.Category.Query;

public record GetCategoriesQuery(GetCategoriesRequest CategoriesRequest) : IQuery<GetCategoriesResponse>;

public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, GetCategoriesResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<GetCategoriesQuery> _validator;
    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository, IValidator<GetCategoriesQuery> validator)
    {
        _categoryRepository = categoryRepository;
        _validator = validator;
    }

    public async Task<Result<GetCategoriesResponse>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var validator = await _validator.ValidateAsync(request, cancellationToken);

        if (!validator.IsValid)
        {
            return Result.Failure<GetCategoriesResponse>(new(
                    "Category.BadRequest",
                    "Invalid id type, please check ur input"
                    ));
        }

        var categories = await _categoryRepository.GetCategoryByIdentityAsync(request.CategoriesRequest, cancellationToken);

        var response = categories.Select(row => new GetCategoriesResponse()
        {
            Searched_Identity = row.Uid_Reference?.ToString("N"),
            Layer = row.Layer,
            Items = categories.Select(item => new ItemCategoryResponse { Id = item.Uid.ToString("N"), Name = item.Name }).ToList(),
            Paginations = new()
            {
                Current = row.Current,
                Next = row.Next,
                Previous = row.Previous,
            }
        }).FirstOrDefault();

        if (categories == null || response == null)
        {
            return Result.Failure<GetCategoriesResponse>(new(
                "Category.NotFound",
                "There is no category found in database, please check your input"
                ));
        }

        return Result.Success(response);
    }
}

public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesQueryValidator()
    {
        RuleFor(key => key.CategoriesRequest.Searched_Identity ?? "").BeValidGuid();
    }

}
