using FluentMigrator;

namespace CA.Assessment.Database.Migrations.Scripts;

[Migration(2)]
public sealed class _02_CreateImagesTable : Migration
{
    public override void Up()
    {
        Create.Table("images")
            .WithColumn("id").AsString().PrimaryKey()
            .WithColumn("mime").AsString().NotNullable()
            .WithColumn("Name").AsString().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("images");
    }
}
