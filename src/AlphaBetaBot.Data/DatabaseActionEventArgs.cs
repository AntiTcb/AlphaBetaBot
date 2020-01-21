using System;

namespace AlphaBetaBot.Data
{
    public class DatabaseActionEventArgs
    {
        public ActionType ActionType { get; set; }
        public object Entity { get; set; }
        public Type EntityType => Entity?.GetType();
        public string Path { get; set; }
        public bool IsErrored { get; set; }
        public Exception Exception { get; set; }
    }
    public enum ActionType
    {
        Get,
        GetAll,
        Add,
        Delete,
        DeleteAll,
        Update,
        Save
    }
}