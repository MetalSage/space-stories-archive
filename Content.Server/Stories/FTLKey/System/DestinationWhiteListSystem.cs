using Content.Shared.Whitelist;
using Content.Shared.Tag;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Linq;

namespace Content.Server.Stories.Systems;

public sealed partial class DestinationWL
{
    public static EntityWhitelist CreateList(string ftlTags)
    {
        return CreateList(new string[] { ftlTags });
    }

    public static EntityWhitelist CreateList(string[] ftlTags)
    {
        var ftlTagList = new List<string>(ftlTags);
        var whitelist = new EntityWhitelist()
        {
            Tags = ftlTagList
        };
        return whitelist;
    }
}
