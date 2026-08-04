# GenericParser

ASP.NET Core Web API for parsing and processing different content formats.

The API accepts Base64-encoded content, parses it using the appropriate parser and returns structured data.

Currently supported formats:
- CSV
- Internal JSON

## Running the application

Run the application:

```bash
dotnet run --project GenericParser
```

After startup, the console will show the application URL. Open:

```
https://localhost:<port>/swagger
```

in your browser.

## Running tests

The project contains 23 automated tests covering parsers, services and API endpoints.

Run all tests with:

```bash
dotnet test
```

## Example

### CSV request

Endpoint:

```
POST /api/v1/parse-content
```

Request body:

```json
{
  "type": "CSV",
  "content": "bmFtZSxhZ2UKSm9obiwzMApBbm5hLDI1"
}
```

Decoded content:

```csv
name,age
John,30
Anna,25
```

Response:

```json
{
  "status": "Success",
  "processedCount": 2,
  "data": [
    {
      "name": "John",
      "age": "30"
    },
    {
      "name": "Anna",
      "age": "25"
    }
  ]
}
```

### Internal JSON request

Request body:

```json
{
  "type": "INTERNAL_JSON",
  "content": "W3siaWQiOjEsIm5hbWUiOiJKb2huIn1d"
}
```

Decoded content:

```json
[
  {
    "id": 1,
    "name": "John"
  }
]
```

Response:

```json
{
  "status": "Success",
  "processedCount": 1,
  "data": [
    {
      "id": 1,
      "name": "John"
    }
  ]
}
```
