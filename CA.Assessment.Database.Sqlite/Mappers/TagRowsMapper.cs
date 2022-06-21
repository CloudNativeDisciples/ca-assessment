using CA.Assessment.Database.Sqlite.Rows;
using CA.Assessment.Model;

namespace CA.Assessment.Database.Sqlite.Mappers;

internal static class TagRowsMapper
{
    private static Tag MapOne(TagDbRow tagDbRow)
    {
        if (tagDbRow is null)
        {
            throw new ArgumentNullException(nameof(tagDbRow));
        }

        return new Tag(Guid.Parse(tagDbRow.Id), tagDbRow.Name);
    }

    internal static IEnumerable<Tag> MapMany(IEnumerable<TagDbRow> tagRows)
    {
        if (tagRows is null)
        {
            throw new ArgumentNullException(nameof(tagRows));
        }

        return tagRows.Select(MapOne).ToList();
    }
}
