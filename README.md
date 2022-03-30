# Cloud Academy assessment

Cloud Academy assessment for a Senior Backend Engineer role

## Overview

The application
follows [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html). Business
logic is implemented in `CA.Assessment.Application` and all the adapters and interfaces to external services are
implemented in `CA.Assessment.Infrastructure`.

There is a very thin layer of database management and interaction which is used in place of a fully fledged ORM
solution.

The application does not have a rich domain model, instead all the domain entities are
simple [POCOs](https://en.wikipedia.org/wiki/Plain_old_CLR_object).  
Business logic is modelled in entity services that expose methods that function
as [Transaction Scripts](https://martinfowler.com/eaaCatalog/transactionScript.html).

The database of choice is SQLite to keep the application self contained and easy to try out.

There are only integration tests to keep development fast and ensure high code coverage quickly.

By design the Blog Post cannot be created immediately with an image, instead the image can be attached at a later time
with a different `POST` request. This choice was made to have the Blog Post creation use an `application/json` payload
instead of `application/x-www-form-urlencoded`.

## Running the application

The application uses .NET and ASP .NET therefore you
need [to follow this guide to install the .NET Core SDK](https://docs.microsoft.com/en-us/dotnet/core/install/linux).

### Server

Once the repository is cloned `cd` into the repository root folder and run `dotnet run --project CA.Assessment.WebAPI`  
By default the application will listen on `http://localhost:8090`.  
The application data will be written to `/tmp` by default.

### Tests

Once the repository is cloned `cd` into the repository root folder and run `dotnet test`.  
Unfortunately, in the .NET ecosystem there is no easy and free way to show code coverage, that's why I setup Codecov on
the repository.  
Code coverage will not be available when running tests locally.

## Coverage

Code Coverage is automatically calculated an reported on every commit to Codecov using GitHub Actions.  
You should be able to view the coverage on Codecov by logging in to Codecov with your GitHub account.  
Codecov currently reported coverage is 72.65%.

## API

The API is available under: `api/v1/blog-posts` and it exposes the endpoints detailed below.  
All the endpoints except `POST api/v1/blog-posts/<blog_post_id>/images` expect an `application/json` payload.

`POST   api/v1/blog-posts`
Creates a new Blog Post and returns the Blog Post id just created.  
It expects a JSON payload similar to:

```json
{
  "title": "title_1",
  "author": "author_1",
  "content": "content_1",
  "category": "the_category",
  "tags": [
    "tag_1",
    "tag_2"
  ]
}
```

All the properties are required.  
Any tag or category that does not exist is created on the fly. Tag and category names are case-sensitive.

`GET    api/v1/blog-posts/<blog_post_id>`
Gets the Blog Post with the id specified, if it exists.

`DELETE api/v1/blog-posts/<blog_post_id>`
Deletes the Blog Post with the id specified, if it exists and the user has specified `X-User`: `Admin` in the request
headers.

`PATCH  api/v1/blog-posts/<blog_post_id>`
Partially updates the Blog Post with the id specified, if it exists.

It expects a JSON payload similar to:

```json
{
  "title": "title_1",
  "author": "author_1",
  "content": "content_1",
  "category": "the_category",
  "tags": [
    "tag_1",
    "tag_2"
  ]
}
```

None of the properties is required.  
Any tag or category that does not exist is created on the fly. Tag and category names are case-sensitive.

`POST   api/v1/blog-posts/search`
Searches Blog Posts that match the filters specified.

It expects a JSON payload similar to:

```json
{
  "title": "title_1",
  "category": "the_category",
  "tags": [
    "tag_1",
    "tag_2"
  ]
}
```

None of the properties is required.  
Tag and category names are case-sensitive.

`DELETE api/v1/blog-posts/<blog_post_id>/tags`
Removes the tags specified from the blog post specified, if it exists.

It expects a JSON payload similar to:

```json
[
  "tag_1",
  "tag_2"
]
```

The array is required.  
Tag names are case-sensitive.

`PUT    api/v1/blog-posts/<blog_post_id>/tags`
Add the tags specified from the blog post specified, if it exists.

It expects a JSON payload similar to:

```json
[
  "tag_1",
  "tag_2"
]
```

The array is required.  
Tag names are case-sensitive.

`POST    api/v1/blog-posts/<blog_post_id>/images`
Add the image specified to the blog post specified, if it exists.   
It expects an `application/x-www-form-urlencoded` with one file property called `image`.  
The `image` file property is required.

`GET    api/v1/blog-posts/<blog_post_id>/images/<image_id>`
Returns the image specified of the blog post specified, if it exists.  
This call returns an image file.