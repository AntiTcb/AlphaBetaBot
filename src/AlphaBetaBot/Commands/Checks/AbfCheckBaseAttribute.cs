using Qmmands;

namespace AlphaBetaBot
{
    public abstract class AbfCheckBaseAttribute : CheckAttribute
    {
        public virtual string Name { get; set; } = "Unknown check.";
        public virtual string Details { get; set; } = ": ";
    }
}
