$ImageMonikerCS = "$PSScriptRoot/ImageMoniker.cs"
$ImageFactoryCS = "$PSScriptRoot/ImageFactory.cs"

"namespace MKW.GUI.Images
{
    public enum ImageMoniker
    {
        None," | Out-File $ImageMonikerCS -Encoding utf8

"using System.Windows.Controls;

namespace MKW.GUI.Images
{
    public static class ImageFactory
    {
        public static Viewbox? MakeImage(ImageMoniker moniker) => moniker switch
        {
            ImageMoniker.None => null," | Out-File $ImageFactoryCS -Encoding utf8

Get-ChildItem $PSScriptRoot *.xaml | ForEach-Object {
    $name = $_.BaseName

"using System.Windows.Controls;

namespace MKW.GUI.Images
{
    public partial class $name : Viewbox
    {
        public $name()
        {
            InitializeComponent();
        }
    }
}" | Out-File -FilePath "$_.cs" -Encoding utf8

    (Get-Content -Path $_).Replace("<Viewbox Width=", "<Viewbox x:Class=`"MKW.GUI.Images.$name`" Width=") | Out-File -FilePath $_ -Encoding ascii

    "        $name," | Add-Content $ImageMonikerCS -Encoding utf8
    "            ImageMoniker.$name => new $name()," | Add-Content $ImageFactoryCS -Encoding utf8

    svn add "$_.cs" --force
}

"    }
}" | Add-Content $ImageMonikerCS -Encoding utf8

"        };
    }
}" | Add-Content $ImageFactoryCS -Encoding utf8

Write-Host "-----"

svn status $PSScriptRoot
