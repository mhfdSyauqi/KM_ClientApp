using KM_ClientApp.Commons.Shared;

namespace KM_ClientApp.Endpoint.Category;

public static class CategoryErrors
{
    public static readonly Error NotFound = new(
        "Category.NotFound",
        "There is no category found in database, please check your input");
}
