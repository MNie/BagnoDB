# BagnoDB

F# wrapper over MongoDB.Driver.

## How to build configuration.

```fsharp
async {
  let collection = "bagno"
  let database = "bagnoDBTests"

  let config = {
    host = "0.0.0.0"
    port = 27017
    user = Some "admin"
    password = Some "123"
  }
  
  let! configuration =
    Connection.host config
    |> Connection.database database
    |> Connection.collection collection
}
```

## How to build filters.

Available options:
- `eq` - equal to $value,
- `gte` - greater than or equal to $value,
- `gt` - greater than $value,
- `lt` - less than $value,
- `lte` - less than or equal $value,
- `not` - negation of filter,
- `empty` - empty filter.

Additionally filters could be combine via:
- `&&&` or `Filter.and` - equivalent of `and` operator between filters,
- `|||` or `Filter.or` - equivalent of `or` operator between filters.

```fsharp
let filter =
  Filter.eq (ExpressionHelper.AsExpression (fun (o: BagnoTest) -> o.data)) "Bagno"
  |> (|||) (Filter.lt (ExpressionHelper.AsExpression (fun (o: BagnoTest) -> o.value)) 2137)
```

## How to run query against MongoDB

Available options:
- `filter` - get `n` results based on passed filter,
- `delete` - delete record based on a filter,
- `deleteMany` - delete records based on a filter,
- `upsert` - update a record based on a filter,
- `insert` - insert record,
- `insertMany` - insert records,
- `getAll` - get all results based on passed filter options.

```fsharp
let filter = Filter.eq (ExpressionHelper.AsExpression (fun (o: BagnoTest) -> o.data)) "mango"
let filterOpt = FindOptions<BagnoTest>()
async {
  let! result =
    Connection.host config
    |> Connection.database database
    |> Connection.collection collection
    |> Query.filter CancellationToken.None filterOpt filter

  return result
} |> Async.StartAsTask
```
