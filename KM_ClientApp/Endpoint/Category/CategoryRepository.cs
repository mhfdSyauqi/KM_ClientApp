using Dapper;
using KM_ClientApp.Commons.Connection;
using KM_ClientApp.Models.Entity;
using KM_ClientApp.Models.Request;

namespace KM_ClientApp.Endpoint.Category;

public class CategoryRepository : ICategoryRepository
{
    private readonly ISQLConnectionFactory _connection;

    public CategoryRepository(ISQLConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Categories>> GetCategoryByIdentityAsync(GetCategoriesRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string storedProcedureName = "[dbo].[Get_Categories_By_Identity]";

        var command = new CommandDefinition(
            storedProcedureName,
            new
            {
                searchedIdentity = request.Searched_Identity == null ? Guid.Empty : Guid.Parse(request.Searched_Identity),
                currentPage = request.Current_Page,
            },
            commandType: System.Data.CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<Categories>(command);

        return result;
    }

    public async Task<IEnumerable<Categories>> GetSuggestionCategoryAsync(SuggestionCategoriesRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string storedProcedureName = "[dbo].[Search_Categories_By_Keyword]";

        var command = new CommandDefinition(
            storedProcedureName,
            request,
            commandType: System.Data.CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<Categories>(command);

        return result;
    }

    public async Task<IEnumerable<Categories>> SearchCategoryByKeywordAsync(SearchCategoriesRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string storedProcedureName = "[dbo].[Search_Categories_By_Keyword]";

        var command = new CommandDefinition(
            storedProcedureName,
            request,
            commandType: System.Data.CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<Categories>(command);

        return result;
    }
}

public interface ICategoryRepository
{
    Task<IEnumerable<Categories>> GetCategoryByIdentityAsync(GetCategoriesRequest request, CancellationToken cancellationToken);

    Task<IEnumerable<Categories>> SearchCategoryByKeywordAsync(SearchCategoriesRequest request, CancellationToken cancellationToken);

    Task<IEnumerable<Categories>> GetSuggestionCategoryAsync(SuggestionCategoriesRequest request, CancellationToken cancellationToken);
}
