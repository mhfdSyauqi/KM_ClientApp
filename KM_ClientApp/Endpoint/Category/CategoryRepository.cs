﻿using Dapper;
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

    public async Task<int> AddHeatCategoryAsync(HeatCategoriesRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string storedProcedureName = "[dbo].[Add_Categories_Heat]";

        var parameter = new HeatCategories()
        {
            Session_Id = Guid.Parse(request.Session_Id),
            User_Name = request.User_Name,
            Heat_Name = request.Heat_Name,
            Heat_Id = string.IsNullOrEmpty(request.Heat_Id) ? null : Guid.Parse(request.Heat_Id)
        };

        var command = new CommandDefinition(
            storedProcedureName,
            parameter,
            commandType: System.Data.CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var result = await connection.ExecuteAsync(command);

        return result;
    }

    public async Task<int> GetReAskedCategoryAsync(ReAskedRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string query = @"
            SELECT 
	            [total_asked]
            FROM 
	            [dbo].[View_User_Session_History]
            WHERE
                [time] = CAST(@Create_At as date) AND
                [create_by] = @Create_By AND
                [uid_bot_category] = @Category_Id                
        ";

        var param = new
        {
            request.Create_At,
            request.Create_By,
            Category_Id = Guid.Parse(request.Category_Id)
        };

        var command = new CommandDefinition(query, param, cancellationToken: cancellationToken);
        var result = await connection.QueryFirstOrDefaultAsync<int>(command);

        return result;
    }

    public async Task<IEnumerable<Categories>> GetCategoryByIdentityAsync(GetCategoriesRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string storedProcedureName = "[dbo].[Get_Categories_By_Identity]";

        var param = new GetCategory()
        {
            searchedIdentity = string.IsNullOrEmpty(request.Searched_Identity) ? null : Guid.Parse(request.Searched_Identity),
            currentPage = request.Current_Page
        };

        var command = new CommandDefinition(
            storedProcedureName,
            param,
            commandType: System.Data.CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<Categories>(command);

        return result;
    }

    public async Task<Guid?> GetReferenceCategoryAsync(ReferenceCategoriesRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string query = @"
               SELECT 
	                [uid_reference]
                FROM 
	                [dbo].[View_Active_Categories]
                WHERE [uid] = @Id
        ";

        var command = new CommandDefinition(query, new { Id = Guid.Parse(request.Id) }, cancellationToken: cancellationToken);
        var result = await connection.QueryFirstOrDefaultAsync<Guid?>(command);

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
    Task<Guid?> GetReferenceCategoryAsync(ReferenceCategoriesRequest request, CancellationToken cancellationToken);

    Task<IEnumerable<Categories>> GetCategoryByIdentityAsync(GetCategoriesRequest request, CancellationToken cancellationToken);

    Task<IEnumerable<Categories>> SearchCategoryByKeywordAsync(SearchCategoriesRequest request, CancellationToken cancellationToken);

    Task<IEnumerable<Categories>> GetSuggestionCategoryAsync(SuggestionCategoriesRequest request, CancellationToken cancellationToken);

    Task<int> AddHeatCategoryAsync(HeatCategoriesRequest request, CancellationToken cancellationToken);

    Task<int> GetReAskedCategoryAsync(ReAskedRequest request, CancellationToken cancellationToken);
}
