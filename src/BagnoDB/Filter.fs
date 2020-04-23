namespace BagnoDB

    open System
    open MongoDB.Driver
    open System.Linq.Expressions

    type internal Operation =
        | Eq
        | Greater
        | GreaterOrEqual
        | Less
        | LessOrEqual
        | Not

    type internal InternalFilter<'TModel, 'TField> =
        {
            operation: Operation
            field: Expression<Func<'TModel, 'TField>>
            value: 'TField
        }

    type Filter<'TModel> =
        {
            definition: FilterDefinition<'TModel>
        }
        static member (&&&) (first, second) =
            { definition = first.definition &&& second.definition }

        static member (|||) (first, second) =
            { definition = first.definition ||| second.definition }

    type Filter () =
        static member private create<'TModel, 'TField> fOp =
            let filter = FilterDefinitionBuilder<'TModel>()

            let translated =
                match fOp.operation with
                | Eq -> filter.Eq(fOp.field, fOp.value)
                | Greater -> filter.Gt<'TField>(fOp.field, fOp.value)
                | GreaterOrEqual -> filter.Gte(fOp.field, fOp.value)
                | Less -> filter.Lt(fOp.field, fOp.value)
                | LessOrEqual -> filter.Lte(fOp.field, fOp.value)
                | Not -> filter.Not(filter.Eq(fOp.field, fOp.value))

            { definition = translated }
        static member eq (f: Expression<Func<'TItem, 'TField>>) =
            fun (v: 'TField) ->
                { operation = Eq; field = f; value = v }
                |> Filter.create
        static member gt (f: Expression<Func<'TItem, 'TField>>) =
            fun (v: 'TField) ->
                { operation = Greater; field = f; value = v }
                |> Filter.create
        static member gte (f: Expression<Func<'TItem, 'TField>>) =
            fun (v: 'TField) ->
                { operation = GreaterOrEqual; field = f; value = v }
                |> Filter.create
        static member lt (f: Expression<Func<'TItem, 'TField>>) =
            fun (v: 'TField) ->
                { operation = Less; field = f; value = v }
                |> Filter.create
        static member lte (f: Expression<Func<'TItem, 'TField>>) =
            fun (v: 'TField) ->
                { operation = LessOrEqual; field = f; value = v }
                |> Filter.create
        static member not (f: Expression<Func<'TItem, 'TField>>) =
            fun (v: 'TField) ->
                { operation = Not; field = f; value = v }
                |> Filter.create

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Filter =
        let ``and`` ``new`` previous =
            { previous with definition = previous.definition &&& ``new``.definition }

        let ``or`` ``new`` previous =
            { previous with definition = previous.definition ||| ``new``.definition }

        let empty<'TModel> = { definition = FilterDefinitionBuilder<'TModel>().Empty }