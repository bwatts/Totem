using System;
using System.Threading;
using System.Threading.Tasks;
using Dream.Versions;
using Spectre.Console;
using Totem;

namespace Dream.Areas.Home
{
    public class HomeArea : IConsoleArea
    {
        readonly IAnsiConsole _console;
        readonly ITotemClient _totemClient;

        public HomeArea(IAnsiConsole console, ITotemClient totemClient)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _totemClient = totemClient ?? throw new ArgumentNullException(nameof(totemClient));
        }

        public async Task NavigateAsync(CancellationToken cancellationToken)
        {
            var layout = new Table { Border = TableBorder.None, ShowHeaders = false };
            layout.AddColumn(new("") { Alignment = Justify.Left });
            layout.AddColumn(new("") { Alignment = Justify.Right, Padding = new Padding(1) });

            layout.AddRow(
                new FigletText("DREAM").Color(Color.Magenta1),
                new Markup("\nConnected: [red]N[/]\n"));

            layout.Expand();

            _console.Write(layout);
            _console.WriteRule(style: "magenta1");
            _console.WriteLine();
            _console.Markup(" Loading [green]EventStore[/] versions...");

            var versions = await _totemClient.SendAsync(new ListVersions(), cancellationToken);

            _console.MarkupLine($"[green]{versions.Length}[/] found");
            _console.WriteLine();

            if(versions.Length == 0)
            {
                var add = "Add EventStore version [green]20.10.2[/]";
                var addOther = "Add other EventStore version";

                var choice = _console.Prompt(new SelectionPrompt<string>
                {
                    Title = "[magenta1] How would you like to proceed?[/]",
                    HighlightStyle = "orange1"
                }
                .AddChoices(add, addOther));

                if(choice == add)
                {
                    var zipUrl = new Uri("https://github.com/EventStore/Downloads/raw/master/win/EventStore-OSS-Windows-2019-v20.10.2.zip");

                    _console.MarkupLine($"[orange1] Installing EventStore version [green]20.10.2[/] from:[/]");
                    _console.MarkupLine($"[blue] {zipUrl}[/]");

                    await _totemClient.SendAsync(new InstallVersion { ZipUrl = zipUrl }, cancellationToken);
                }
                else
                {
                    var zipUrl = _console.Ask<Uri>("Enter the zip URL for the version");

                    _console.MarkupLine($"[orange1] Installing EventStore version from:[/]");
                    _console.MarkupLine($"[blue] {zipUrl}[/]");

                    await _totemClient.SendAsync(new InstallVersion { ZipUrl = zipUrl }, cancellationToken);
                }

                // TODO: Monitor progress
            }
            else
            {
                var table = new Table { ShowHeaders = false, Border = TableBorder.Ascii };
                table.AddColumns("", "");
                
                foreach(var version in versions)
                {
                    table.AddRow("[green]Version[/]", version.Id.ToCompactString());
                    table.AddRow("[green]Status[/]", version.Status.ToString());

                    if(version.ZipFile != null)
                    {
                        table.AddRow("[green]Path[/]", version.ZipFile.Path.ToString());
                        table.AddRow("[green]Bytes[/]", version.ZipFile.ByteCount.ToString());
                    }

                    if(version.ZipContent != null)
                    {
                        table.AddRow("[green]Files[/]", version.ZipContent.FileCount.ToString());
                        table.AddRow("[green]Bytes[/]", version.ZipContent.ByteCount.ToString());
                        table.AddRow("[green]Exe Path[/]", version.ZipContent.ExePath.ToString());
                    }
                }

                _console.Write(table);
            }
        }
    }
}