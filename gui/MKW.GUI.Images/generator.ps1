$ImageMonikerCS = "$PSScriptRoot/ImageMoniker.cs"

"namespace MKW.GUI.Images
{
    public enum ImageMoniker
    {
        None," | Out-File $ImageMonikerCS -Encoding utf8

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

    svn add "$_.cs" --force
}

"    }
}" | Add-Content $ImageMonikerCS -Encoding utf8

Write-Host "-----"

svn status $PSScriptRoot
