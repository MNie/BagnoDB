namespace BagnoDB
    open MongoDB.Driver

    module Query =
        let getAll<'TModel> token (fOpt: FindOptions<'TModel>) con =
            let connection = Connection.connect<'TModel> con
            let emptyFilter =
                FilterDefinitionBuilder<'TModel>().Empty
            async {
                let! result = connection.FindAsync (emptyFilter, fOpt, token) |> Async.AwaitTask
                return! result.ToListAsync() |> Async.AwaitTask
            }

        let filter<'TModel> token (fOpt: FindOptions<'TModel>) (fOp: Filter<'TModel>) con =
            let connection = Connection.connect<'TModel> con
            async {
                let! result = connection.FindAsync (fOp.definition, fOpt, token) |> Async.AwaitTask
                return! result.ToListAsync() |> Async.AwaitTask
            }

        let insertOne<'TModel> token iOpt doc con =
            let connection = Connection.connect<'TModel> con
            async {
                do! connection.InsertOneAsync (doc, iOpt, token) |> Async.AwaitTask
            }

        let insertMany<'TModel> token iOpt docs con =
            let connection = Connection.connect<'TModel> con
            async {
                do! connection.InsertManyAsync (docs, iOpt, token) |> Async.AwaitTask
            }

        let upsert<'TModel> token (uOpt: FindOneAndReplaceOptions<'TModel>) doc (fOp: Filter<'TModel>) con =
            let connection = Connection.connect<'TModel> con
            async {
                return! connection.FindOneAndReplaceAsync(fOp.definition, doc, uOpt, token) |> Async.AwaitTask
            }

        let deleteMany<'TModel> token dOpt (fOp: Filter<'TModel>) con =
            let connection = Connection.connect<'TModel> con
            async {
                return! connection.DeleteManyAsync(fOp.definition, dOpt, token) |> Async.AwaitTask
            }

        let delete<'TModel> token (dOpt: FindOneAndDeleteOptions<'TModel>) (fOp: Filter<'TModel>) con =
            let connection = Connection.connect<'TModel> con
            async {
                return! connection.FindOneAndDeleteAsync(fOp.definition, dOpt, token) |> Async.AwaitTask
            }
