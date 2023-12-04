using KM_ClientApp.Controllers;
using KM_ClientApp.Endpoint.Category.Query;
using KM_ClientApp.Models.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KM_ClientApp.Endpoint.Category;

public class CategoryController : MyAPIController
{
    public CategoryController(ISender sender) : base(sender)
    {
    }


    [HttpGet]
    public async Task<IActionResult> GetCategories([FromBody] GetCategoriesRequest request, CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery(request);
        var result = await Sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.CreateResponseObject()) : NotFound(result.Error);
    }

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> SearchCategories([FromBody] SearchCategoriesRequest request, CancellationToken cancellationToken)
    {
        var query = new SearchCategoriesQuery(request);
        var result = await Sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.CreateResponseObject()) : NotFound(result.Error);
    }

    [HttpGet]
    [Route("suggest")]
    public async Task<IActionResult> SugestionCategories([FromBody] SuggestionCategoriesRequest request, CancellationToken cancellationToken)
    {
        var query = new SuggestionCategoriesQuery(request);
        var result = await Sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.CreateResponseObject()) : NotFound(result.Error);
    }
}
