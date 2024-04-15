using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;

class Db : DataConnection
{
    public Db(DataOptions<Db> options)
        :base(options.Options)
    {
    }

    public ITable<Models.Todo> Todos => this.GetTable<Models.Todo>();
    public ITable<Models.User> Users => this.GetTable<Models.User>();
}