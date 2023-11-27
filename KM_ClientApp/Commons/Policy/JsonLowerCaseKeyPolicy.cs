using System.Text.Json;

namespace KM_ClientApp.Commons.Policy;

public class JsonLowerCaseKeyPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));

        return name.ToLower();
    }
}
