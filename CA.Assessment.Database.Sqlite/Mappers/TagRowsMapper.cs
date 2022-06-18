using CA.Assessment.Database.Sqlite.Rows;
using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Database.Sqlite.Mappers;

internal sealed class TagRowsMapper
{
    private Tag MapOne(TagRow tagRow)
    {
        if (tagRow is null)
        {
            throw new ArgumentNullException(nameof(tagRow));
        }

        return new Tag(Guid.Parse(tagRow.Id), tagRow.Name);
    }

    internal IEnumerable<Tag> MapMany(IEnumerable<TagRow> tagRows)
    {
        if (tagRows is null)
        {
            throw new ArgumentNullException(nameof(tagRows));
        }

        return tagRows.Select(MapOne).ToList();
    }
}