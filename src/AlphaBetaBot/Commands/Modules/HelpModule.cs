using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disqord;
using Qmmands;

namespace AlphaBetaBot
{
    [Name("Help"), Hidden]
    public sealed class HelpCommands : AbfModuleBase
    {
        private readonly CommandService _commands;
        private readonly AbfConfiguration _configuration;

        public HelpCommands(CommandService commands, AbfConfigurationProvider configuration)
        {
            _commands = commands;
            _configuration = configuration.Configuration;
        }

        private Func<Parameter, string> GetUsage(Command command) 
            => x => $"[{x.Name}{(x.DefaultValue != null ? $"={x.DefaultValue}" : "")}]";

        [Command("Help"), Hidden]
        [Description("Shows the different commands and modules usages.")]
        public async Task HelpAsync()
        {
            var modules = await Task.WhenAll(_commands.GetAllModules().Where(x => !x.Attributes.Any(y => y is HiddenAttribute) && x.Parent is null && x.Aliases.Count > 0).Select(async x =>
            {
                var result = await x.RunChecksAsync(Context);
                return result.IsSuccessful ? x : null;
            }).Where(x => x != null));

            var commands = (await Task.WhenAll(_commands.GetAllCommands().Where(x => !x.Attributes.Any(y => y is HiddenAttribute) && x.Module.Aliases.Count == 0).Select(async x =>
            {
                var result = await x.RunChecksAsync(Context);
                return result.IsSuccessful ? x : null;
            }))).Where(x => x != null && x.Module.Parent is null).DistinctBy(x => x.Name).ToArray();

            var embed = new LocalEmbedBuilder
            {
                Title = "Help",
                Description = "This embed contains a list of every available modules. If you want to see every command of a module, use `!help <module>`.",
                Footer = new LocalEmbedFooterBuilder
                {
                    Text = $"{modules.Length} modules and {commands.Length} commands available in this context."
                }
            };

            embed.AddField("Modules", string.Join(", ", modules.Select(x => $"`{x.Name}`")));
            embed.AddField("Commands", string.Join(", ", commands.Select(x => $"`{x.Name}`")));

            await ReplyAsync(embed: embed.Build());
        }

        [Command("Help"), Hidden]
        [Description("Shows the different commands and modules usages.")]
        public Task HelpAsync([Remainder, Description("Command or module.")] string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return HelpAsync();
            }

            var matchingCommands = _commands.FindCommands(command);
            if (matchingCommands.Count == 0)
            {
                string typoFix = command.Levenshtein(_commands);
                if (!string.IsNullOrWhiteSpace(typoFix))
                {
                    matchingCommands = _commands.FindCommands(typoFix);
                }
            }

            LocalEmbedBuilder embed;
            if (matchingCommands.Count == 0) //Could be a module.
            {
                var matchingModule = _commands.TopLevelModules.FirstOrDefault(x => x.Name.Equals(command, StringComparison.OrdinalIgnoreCase));
                if (matchingModule is null)
                {
                    matchingModule = _commands.TopLevelModules.FirstOrDefault(x => x.Name.Equals(command.Levenshtein(_commands), StringComparison.OrdinalIgnoreCase));
                }

                var modules = _commands.GetAllModules();
                if (matchingModule is null) //Look for nested modules
                {
                    matchingModule = modules.FirstOrDefault(x => x.FullAliases.Any(y => y.Equals(command, StringComparison.OrdinalIgnoreCase)));
                }

                if (matchingModule is null) //Look for nested modules with levenshtein
                {
                    matchingModule = modules.FirstOrDefault(x => x.FullAliases.Any(y => y.Equals(command.Levenshtein(modules.Select(z => z.FullAliases.FirstOrDefault()).ToList()), StringComparison.OrdinalIgnoreCase)));
                }

                if (matchingModule is null) //Look for submodule but without complete module path
                {
                    matchingModule = modules.FirstOrDefault(x => x.Name.Equals(command, StringComparison.OrdinalIgnoreCase));
                }

                if (matchingModule is null) //Look for submodule but without complete module path with levenshtein
                {
                    matchingModule = modules.FirstOrDefault(x => x.Name.Equals(command.Levenshtein(modules.Select(z => z.Name).ToList()), StringComparison.OrdinalIgnoreCase));
                }

                if (matchingModule is null) //Remove last input
                {
                    var cmdArgs = command.Split(' ').ToList();
                    cmdArgs.RemoveAt(cmdArgs.Count - 1);

                    return HelpAsync(string.Join(' ', cmdArgs));
                }

                embed = new LocalEmbedBuilder
                {
                    Title = $"Help - {matchingModule.Name} Module",
                    Description = "This embed contains the list of every command in this module.",
                    Footer = new LocalEmbedFooterBuilder
                    {
                        Text = $"{matchingModule.Commands.Count} commands available in this context."
                    }
                };

                if (matchingModule.Submodules.Count > 0)
                {
                    embed.AddField("Modules", string.Join(", ", matchingModule.Submodules.DistinctBy(x => x.Name).Select(x => $"`{x.Name}`")));
                }

                if (matchingModule.Commands.Count > 0)
                {
                    embed.AddField("Commands", string.Join(", ", matchingModule.Commands.DistinctBy(x => x.Name).Select(x => $"`{x.Name}`")));
                }

                var moduleChecks = CommandUtilities.EnumerateAllChecks(matchingModule).Cast<AbfCheckBaseAttribute>().ToArray();
                if (moduleChecks.Length > 0)
                {
                    embed.AddField("Requirements", string.Join("\n", moduleChecks.Select(x => $"`- {x.Name}{x.Details}`")));
                }

                return ReplyAsync(embed: embed.Build());
            }

            embed = new LocalEmbedBuilder
            {
                Title = "Help"
            };

            var builder = new StringBuilder();
            foreach (var cmd in matchingCommands.Where(c => c.Command.Attributes.All(a => !(a is HiddenAttribute))))
            {
                builder.AppendLine($"**{cmd.Command.Description ?? "Undocumented."}**");
                builder.AppendLine(($"`{Context.Prefix}{cmd.Command.Name} {string.Join(" ", cmd.Command.Parameters.Select(GetUsage(cmd.Command)))}`".ToLowerInvariant()).TrimEnd());
                foreach (var param in cmd.Command.Parameters)
                {
                    builder.AppendLine($"`[{param.Name}]`: {param.Description ?? "Undocumented."}");
                }
                builder.AppendLine($"\n**Example:** `{cmd.Command.Remarks ?? "No example provided."}`\n");
            }

            embed.AddField("Usages", builder.ToString());

            var defaultCmd = matchingCommands.First().Command;

            var checks = CommandUtilities.EnumerateAllChecks(defaultCmd.Module).Cast<AbfCheckBaseAttribute>().ToArray();
            if (checks.Length > 0)
            {
                embed.AddField("Module Requirements", string.Join("\n", checks.Select(x => $"- `{x.Name}{x.Details}`")));
            }

            if (defaultCmd.Checks.Count > 0)
            {
                embed.AddField("Command Requirements", string.Join("\n", defaultCmd.Checks.Cast<AbfCheckBaseAttribute>().Select(x => $"- `{x.Name}{x.Details}`")));
            }

            return ReplyAsync(embed: embed.Build());
        }
    }
}
