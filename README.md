# WritersBlockAPI

This API allows you to store details about different worlds, locations and areas you may have in a story you're writing/planning. For example, a world would have locations such as a country or piece of land, and a location could have an area (like a sub-location) such as a village or building. Tables which store details about each of these respective categories are linked to each other as a result and so this API allows you to, essentially, create, update and delete indiviudual records with ease.

### Database

This project uses SQL Server and the schema can be found at `src\Data\Schema.sql`.

### Testing

There are automated tests that cover the different API endpoints that can be run using `dotnet test` in the test project folder, and there is a swagger page on startup that can be used to manually test and use the API.

### Development

This is in the middle of development so please expect some features will not be ready until further on when they are needed for my personal use.
