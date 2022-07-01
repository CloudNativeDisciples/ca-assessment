# Senior Backend Engineer assessment

Assessment for a Senior Backend Engineer role

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=federico-paolillo_ca-assessment&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=federico-paolillo_ca-assessment)
[![codecov](https://codecov.io/gh/federico-paolillo/ca-assessment/branch/main/graph/badge.svg?token=0QOH0NVJAO)](https://codecov.io/gh/federico-paolillo/ca-assessment)

## Assignment

Your task is to architect and develop an application that provides a set of REST APIs to manage a single blog website.  

A blog post must have at least:

- A title
- A content
- An author
- An image
- Tags

The APIs have to support the following actions:

- CRUD on blog posts (allowing full and partial updates)
- Searching for a blog post by its title, category, tag(s) or all of the former
- Assigning a blog post to a category
- Adding/Removing tags from a blog post

Constraints:

- A blog post can only be related to one category
- A blog post content cannot exceed 1024 characters
- The deletion of a blog post can be performed by admins only
- The server listens to port 8090
- Test coverage must be at least 60%
- Produce a Dockerfile and a docker-compose file

To avoid implementing a full authn/authz to choose if an user is admin or not, simply use custom HTTP header `X-User` with values `user` for simple users and `admin` for admin users.

The whole assessment has to be done in 5 days.  