using CA.Assessment.Database.Sqlite.Rows;
using CA.Assessment.Model;

namespace CA.Assessment.Database.Sqlite.Mappers;

internal static class CategoryRowsMapper
{
    internal static Category MapOne(CategoryDbRow categoryDbRow)
    {
        if (categoryDbRow is null)
        {
            throw new ArgumentNullException(nameof(categoryDbRow));
        }

        return new Category(Guid.Parse(categoryDbRow.Id), categoryDbRow.Name);
    }
}
