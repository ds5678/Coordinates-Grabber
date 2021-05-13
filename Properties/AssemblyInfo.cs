using MelonLoader;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(CoordinatesGrabber.BuildInfo.Name)]
[assembly: AssemblyDescription(CoordinatesGrabber.BuildInfo.Description)]
[assembly: AssemblyCompany(CoordinatesGrabber.BuildInfo.Company)]
[assembly: AssemblyProduct(CoordinatesGrabber.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + CoordinatesGrabber.BuildInfo.Author)]
[assembly: AssemblyTrademark(CoordinatesGrabber.BuildInfo.Company)]
[assembly: ComVisible(false)]
[assembly: Guid("47c02bc2-d337-40d2-8062-6c1c416ae2ea")]

[assembly: AssemblyVersion(CoordinatesGrabber.BuildInfo.Version)]
[assembly: AssemblyFileVersion(CoordinatesGrabber.BuildInfo.Version)]
[assembly: MelonInfo(typeof(CoordinatesGrabber.Implementation), CoordinatesGrabber.BuildInfo.Name, CoordinatesGrabber.BuildInfo.Version, CoordinatesGrabber.BuildInfo.Author, CoordinatesGrabber.BuildInfo.DownloadLink)]
[assembly: MelonGame("Hinterland", "TheLongDark")]
