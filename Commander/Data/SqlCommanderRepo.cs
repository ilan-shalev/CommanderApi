using Commander.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Commander.Data
{
    public class SqlCommanderRepo : ICommanderRepo
    {
        private readonly CommanderContext DbContext;

        public SqlCommanderRepo(CommanderContext context)
        {
            DbContext = context;
        }

        public void CreateCommand(Command command) // ...
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            DbContext.Commands.Add(command);
        }

        public void DeleteCommand(Command command)
        {
            if(command == null)
                throw new ArgumentNullException();

            DbContext.Commands.Remove(command);
        }

        public IEnumerable<Command> GetAllCommands()
        {
            return DbContext.Commands.ToList();
        }

        public Command GetCommandById(int id)
        {
            return DbContext.Commands.SingleOrDefault(x => x.Id == id);
        }

        public bool SaveChanges()
        {
            return (DbContext.SaveChanges() != 0);
        }

        public void UpdateCommand(Command command)
        {
            // empty. can get reference and update the values later.   
        }
    }
}
