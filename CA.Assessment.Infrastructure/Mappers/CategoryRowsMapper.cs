using CA.Assessment.Domain.Anemic;
using CA.Assessment.Infrastructure.Rows;

namespace CA.Assessment.Infrastructure.Mappers;

internal class CategoryRowsMapper
{
    internal Category MapOne(CategoryRow categoryRow)
    {
        if (categoryRow is null) throw new ArgumentNullException(nameof(categoryRow));

        return new Category(Guid.Parse(categoryRow.Id), categoryRow.Name);
    }
}
