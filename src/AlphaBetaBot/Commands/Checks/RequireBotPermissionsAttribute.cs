﻿using System.Threading.Tasks;
using Disqord;
using Qmmands;

namespace AlphaBetaBot
{
    public sealed class RequireBotPermissionsAttribute : AbfCheckBaseAttribute
    {
        public Permission Permissions { get; }

        public override string Name { get; set; } = "Bot permissions";

        public RequireBotPermissionsAttribute(Permission permissions)
        {
            Permissions = permissions;
            Details += Permissions.ToString();
        }

        public override ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            if (!(context is AbfCommandContext ctx))
                return new CheckResult("Invalid command context.");
            
            if (ctx.Guild == null)
                return CheckResult.Successful;
            
            if (ctx.Guild.OwnerId == ctx.Self.Id)
                return CheckResult.Successful;
            
            var perms = ctx.Guild.CurrentMember.GetPermissionsFor(ctx.Channel as CachedTextChannel);

            if (perms.Has(Permission.Administrator))
                return CheckResult.Successful;
            
            if (perms.Has(Permissions))
                return CheckResult.Successful;
            
            return new CheckResult($"I need the following permissions: {Permissions.ToString()}");
        }
    }
}
