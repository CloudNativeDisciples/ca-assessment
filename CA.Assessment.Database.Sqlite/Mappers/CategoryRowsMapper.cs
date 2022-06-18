using CA.Assessment.Database.Sqlite.Rows;
using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Database.Sqlite.Mappers;

internal class CategoryRowsMapper
{
    internal Category MapOne(CategoryRow categoryRow)
    {
        if (categoryRow is null)
        {
            throw new ArgumentNullException(nameof(categoryRow));
        }

        return new Category(Guid.Parse(categoryRow.Id), categoryRow.Name);
    }
}