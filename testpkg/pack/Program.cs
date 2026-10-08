// Empaquetador de prueba: convierte una carpeta de homebrew (con eboot.bin)
// en un .pkg de PS5 debug sin licencia (LicenseFree), usando LibProsperoPkg.
//
// Uso:
//   pack <HomebrewFolder> <OutputFolder> [ContentId] [Title] [Passcode]
//
// Si no se pasan ContentId/Title, la libreria los toma de sce_sys/param.json
// (o usa un valor por defecto en el caso de Title). El eBOOT se firma como
// fake-self y la clave de montaje se deriva del content id + passcode, de
// modo que el .pkg instala en una consola en modo debug (la que habilita
// kstuff) sin rif.

using LibProsperoPkg;

if (args.Length < 2)
{
    Console.Error.WriteLine("uso: pack <HomebrewFolder> <OutputFolder> [ContentId] [Title] [Passcode]");
    return 2;
}

var options = new ProsperoHomebrewPackageOptions
{
    HomebrewFolder = args[0],
    OutputFolder = args[1],
};

if (args.Length > 2 && !string.IsNullOrWhiteSpace(args[2]))
    options.ContentId = args[2];
if (args.Length > 3 && !string.IsNullOrWhiteSpace(args[3]))
    options.Title = args[3];
if (args.Length > 4 && !string.IsNullOrWhiteSpace(args[4]))
    options.Passcode = args[4];

try
{
    var result = ProsperoHomebrewPackager.Package(
        options,
        msg => Console.WriteLine($"[pack] {msg}"));

    Console.WriteLine($"PKG: {result.OutputPath}");
    Console.WriteLine($"RequiresRif: {result.DebugLicense.RequiresRif}");
    Console.WriteLine($"LaunchReady: {result.LaunchReadiness?.IsLaunchReady}");

    if (result.Warnings.Count > 0)
    {
        Console.WriteLine("WARNINGS:");
        foreach (var w in result.Warnings)
            Console.WriteLine($"  - {w}");
    }

    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"FALLO: {ex}");
    return 1;
}