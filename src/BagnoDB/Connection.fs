namespace BagnoDB
    open MongoDB.Driver

    type Connection =
        {
          config: Config
          database: string option
          collection: string option
        }

    module Connection =
        let host config =
            {
              config = config
              database = None
              collection = None }
        let database db op = { op with database = Some db }
        let collection col op = { op with collection = Some col }
        let create config db col =
             {
              config = config
              database = Some db
              collection = Some col }

        let internal connect<'TModel> op =
            let client = MongoClient (op.config.GetConnectionString())
            match op.database with
            | Some dbName ->
                match op.collection with
                | Some col ->
                    let db = client.GetDatabase dbName
                    let collection = db.GetCollection<'TModel>(col)
                    collection
                | None -> failwith "Collection name was not provided!"
            | None -> failwith "Database name was not provided!"
