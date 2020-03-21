module BagnoDB.Connecting
    open MongoDB.Driver
    open BagnoDB.Configuring

    type Connection =
        {
          Config: Config
          Database: string option
          Collection: string option
        }

    module Connection =
        let host config =
            {
              Config = config
              Database = None
              Collection = None }
        let database db op = { op with Database = Some db }
        let collection col op = { op with Collection = Some col }
        let create config db col =
             {
              Config = config
              Database = Some db
              Collection = Some col }

        let internal connect<'TModel> op =
            let client = MongoClient (op.Config.GetConnectionString())
            match op.Database with
            | Some dbName ->
                match op.Collection with
                | Some col ->
                    let db = client.GetDatabase dbName
                    let collection = db.GetCollection<'TModel>(col)
                    collection
                | None -> failwith "Collection name was not provided!"
            | None -> failwith "Database name was not provided!"
