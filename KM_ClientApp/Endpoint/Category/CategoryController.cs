using KM_ClientApp.Controllers;
using KM_ClientApp.Endpoint.Category.Command;
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


    [HttpPost]
    public async Task<IActionResult> GetCategories([FromBody] GetCategoriesRequest request, CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery(request);
        var result = await Sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.CreateResponseObject()) : NotFound(result.Error);
    }

    [HttpPost]
    [Route("search")]
    public async Task<IActionResult> SearchCategories([FromBody] SearchCategoriesRequest request, CancellationToken cancellationToken)
    {
        var query = new SearchCategoriesQuery(request);
        var result = await Sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.CreateResponseObject()) : NotFound(result.Error);
    }

    [HttpPost]
    [Route("suggest")]
    public async Task<IActionResult> SuggestionCategories([FromBody] SuggestionCategoriesRequest request, CancellationToken cancellationToken)
    {
        var query = new SuggestionCategoriesQuery(request);
        var result = await Sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.CreateResponseObject()) : NotFound(result.Error);
    }

    [HttpPost]
    [Route("heat")]
    public async Task<IActionResult> AddHeatCategories([FromBody] HeatCategoriesRequest request, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        request.User_Name = computerName.Split("\\")[1];

        var command = new HeatCategoriesCommand(request);
        var result = await Sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost]
    [Route("ref")]
    public async Task<IActionResult> GetReferenceCategories([FromBody] ReferenceCategoriesRequest request, CancellationToken cancellationToken)
    {
        var query = new ReferenceCategoriesQuery(request);
        var result = await Sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.CreateResponseObject()) : NotFound(result.Error);
    }

    [HttpPost]
    [Route("reasked")]
    public async Task<IActionResult> ReAskedCategories([FromBody] ReAskedRequest request, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        request.Create_By = computerName.Split("\\")[1];

        var query = new ReAskedCategoryQuery(request);
        var result = await Sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.CreateResponseObject()) : NotFound();
    }
}
