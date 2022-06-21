using FluentMigrator;

namespace CA.Assessment.Database.Migrations.Scripts;

[Migration(1)]
public sealed class CreateTables : Migration
{
    public override void Up()
    {
        Create.Table("blog_posts")
            .WithColumn("id").AsString().PrimaryKey()
            .WithColumn("author").AsString().NotNullable()
            .WithColumn("title").AsString().NotNullable()
            .WithColumn("content").AsString().NotNullable()
            .WithColumn("image_id").AsString().NotNullable()
            .WithColumn("category_id").AsString().NotNullable();

        Create.Table("blog_posts_to_tags")
            .WithColumn("blog_post_id").AsString().PrimaryKey()
            .WithColumn("tag_id").AsString().PrimaryKey();

        Create.Table("tags")
            .WithColumn("id").AsString().PrimaryKey()
            .WithColumn("name").AsString().NotNullable();

        Create.Table("categories")
            .WithColumn("id").AsString().PrimaryKey()
            .WithColumn("name").AsString().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("blog_posts_to_tags");
        Delete.Table("blog_posts");
        Delete.Table("tags");
        Delete.Table("categories");
    }
}
