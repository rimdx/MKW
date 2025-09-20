$Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False
$files = Get-ChildItem $PSScriptRoot *.xaml

### ImageMoniker.cs
$content = "namespace MKW.GUI.Images
{
    public enum ImageMoniker
    {
        None,
"
$files | ForEach-Object {
    $name = $_.BaseName
    $content += "        $name,`n"
}
$content += "    }
}"
[System.IO.File]::WriteAllLines("$PSScriptRoot/ImageMoniker.cs", $content, $Utf8NoBomEncoding)

### ImageFactory.cs
$content = "using System.Windows.Controls;

namespace MKW.GUI.Images
{
    public static class ImageFactory
    {
        public static Viewbox? MakeImage(ImageMoniker moniker) => moniker switch
        {
            ImageMoniker.None => null,
"
$files | ForEach-Object {
    $name = $_.BaseName
    $content += "            ImageMoniker.$name => new $name(),`n"
}
$content +=
"        };
    }
}"

[System.IO.File]::WriteAllLines("$PSScriptRoot/ImageFactory.cs", $content, $Utf8NoBomEncoding)

$files | ForEach-Object {
    $name = $_.BaseName
    $path = $_.FullName

    ### [Name].xaml.cs
    $content = "using System.Windows.Controls;

namespace MKW.GUI.Images
{
    public partial class $name : Viewbox
    {
        public $name()
        {
            InitializeComponent();
        }
    }
}"
    [System.IO.File]::WriteAllLines("$path.cs", $content, $Utf8NoBomEncoding)

    ### [Name].xaml
    $content = Get-Content -Path $path
    $content = $content.Replace("<Viewbox Width=", "<Viewbox x:Class=`"MKW.GUI.Images.$name`" Width=")
    [System.IO.File]::WriteAllLines("$path", $content, $Utf8NoBomEncoding)

    svn add "$path.cs" --force
}

Write-Host "-----"

svn status $PSScriptRoot
